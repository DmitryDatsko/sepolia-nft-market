using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Nethereum.Util;
using Nethereum.Web3;
using SepoliaNftMarket.Context;
using SepoliaNftMarket.Models;
using SepoliaNftMarket.Models.ContractEvents;
using SepoliaNftMarket.Models.DTO;
using SepoliaNftMarket.Providers.Alchemy;
using SepoliaNftMarket.Providers.Etherscan;
using SepoliaNftMarket.Services.EventParser;
using SepoliaNftMarket.Services.Notifications;
using SepoliaNftMarket.Services.Sepolia;

namespace SepoliaNftMarket.Services;

public class RecordChanges : BackgroundService
{
    private readonly ILogger<RecordChanges> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISepoliaService _sepoliaService;
    private readonly IEtherscanProvider _etherscanProvider;
    private readonly IEventParser _eventParser;
    private readonly IAlchemyProvider _alchemyProvider;
    
    public RecordChanges(
        IEtherscanProvider etherscanProvider,
        ILogger<RecordChanges> logger,
        IServiceScopeFactory scopeFactory,
        ISepoliaService sepoliaService,
        IEventParser eventParser,
        IAlchemyProvider alchemyProvider)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _sepoliaService = sepoliaService;
        _etherscanProvider = etherscanProvider;
        _eventParser = eventParser;
        _alchemyProvider = alchemyProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                var notifyService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var nextBlock = await db.Indexer.FirstAsync(i => i.Id == 1, stoppingToken);

                var data = await _etherscanProvider.GetEventLogsAsync(nextBlock.LastProcessedBlock);

                if (data.Result.Count == 0)
                {
                    _logger.LogWarning("Got zero records");
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                nextBlock.LastProcessedBlock = int.Parse(data.Result[0].BlockNumber) + 1;
                nextBlock.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation($"NextBlock: {nextBlock.LastProcessedBlock}");

                var parsedEvents = new List<ParsedEvent>();

                foreach (var dt in data.Result)
                {
                    var evt = _eventParser.ParseEvent(dt);

                    if (evt is null) continue;

                    var priceEth = evt switch
                    {
                        ListingCreatedEvent e => Web3.Convert.FromWei(e.Price),
                        _ => 0m
                    };

                    parsedEvents.Add(new ParsedEvent
                    {
                        Event = evt,
                        BlockNumber = long.Parse(dt.BlockNumber),
                        BlockHash = dt.BlockHash,
                        BlockTimestamp = DateTimeOffset
                            .FromUnixTimeSeconds(Convert.ToInt64(dt.TimeStamp, 16))
                            .UtcDateTime,
                        TransactionHash = dt.TransactionHash,
                        TransactionFrom = await _sepoliaService.GetTransactionInitiatorAsync(dt.TransactionHash),
                        TransactionTo = await _sepoliaService.GetTransactionInitiatorAsync(dt.TransactionHash),
                        Price = priceEth,
                        LogIndex = long.Parse(dt.LogIndex),
                        TransactionIndex = long.Parse(dt.TransactionIndex),
                        LogData = dt.Data,
                        Topic0 = dt.Topics[0],
                        Topic1 = dt.Topics[1],
                        Topic2 = dt.Topics[2],
                        Topic3 = dt.Topics[3]
                    });
                }

                foreach (var pe in parsedEvents)
                {
                    switch (pe.Event)
                    {
                        case ListingCreatedEvent e:
                        {
                            try
                            {
                                if (await db.Listings
                                        .AsNoTracking()
                                        .AnyAsync(l => l.ListingId == e.Id, cancellationToken: stoppingToken))
                                    break;

                                var mtData = await _alchemyProvider
                                    .GetNftMetadataAsync(e.NftContract, e.TokenId.ToString());
                                
                                var lst = new Listing
                                {
                                    ListingId = e.Id,
                                    NftContractAddress = e.NftContract,
                                    Price = Web3.Convert.FromWei(e.Price),
                                    TokenId = e.TokenId,
                                    SellerAddress = e.Seller,
                                    BuyerAddress = string.Empty,
                                    Status = EventStatus.ListingCreated,
                                    NftMetadata = new()
                                    {
                                        TokenId = e.Id,
                                        NftContractAddress = e.NftContract,
                                        Kind = mtData.Kind,
                                        Name = mtData.Name,
                                        ImageOriginal = mtData.ImageOriginal,
                                        Description = mtData.Description,
                                        Price = mtData.Price ?? 0m,
                                        LastUpdated = DateTime.UtcNow
                                    }
                                };
                                
                                var history = new History
                                {
                                    EventMetadata = new()
                                    {
                                        BlockNumber = pe.BlockNumber,
                                        BlockHash = pe.BlockHash,
                                        Timestamp = pe.BlockTimestamp,
                                        TransactionHash = pe.TransactionHash
                                    },
                                    FromAddress = await _sepoliaService.GetTransactionInitiatorAsync(pe.TransactionHash),
                                    ToAddress = string.Empty,
                                    ListingId = e.Id,
                                    TradeId = null,
                                    Listing = lst,
                                    Trade = null,
                                    Status = lst.Status,
                                    CreatedAt = DateTime.UtcNow
                                };
                                
                                await db.Listings.AddAsync(lst, cancellationToken: stoppingToken);
                                await db.History.AddAsync(history, cancellationToken: stoppingToken);
                                await db.SaveChangesAsync(stoppingToken);

                                await notifyService
                                    .NotifyAsync(lst.SellerAddress,
                                        EventStatus.ListingCreated,
                                        "Listing created",
                                        $"You created listing #{lst.ListingId}. Price: {lst.Price} MON"); 

                                _logger.LogInformation($"New listing: {e.Id}");

                                await notifyService.NotifyMarketUpdateAsync();
                            }
                            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pg
                                                                && pg.SqlState == "23505")
                            {
                                _logger.LogWarning("Listing already exists, skipping insert(unique constraint)");
                            }

                            break;
                        }
                        case ListingRemovedEvent e:
                        {
                            var lst = await db.Listings.FirstOrDefaultAsync(l =>
                                l.ListingId == e.Id, cancellationToken: stoppingToken);
                            
                            if(await db.Listings
                                   .AsNoTracking()
                                   .AnyAsync(l => l.ListingId == e.Id 
                                                  && l.Status == EventStatus.ListingRemoved,
                                       cancellationToken: stoppingToken))
                                break;

                            if (lst is not null)
                            {
                                lst.Status = EventStatus.ListingRemoved;
                                
                                var history = new History
                                {
                                    EventMetadata = new()
                                    {
                                        BlockNumber = pe.BlockNumber,
                                        BlockHash = pe.BlockHash,
                                        Timestamp = pe.BlockTimestamp,
                                        TransactionHash = pe.TransactionHash
                                    },
                                    FromAddress = await _sepoliaService.GetTransactionInitiatorAsync(pe.TransactionHash),
                                    ToAddress = string.Empty,
                                    ListingId = lst.ListingId,
                                    TradeId = null,
                                    Listing = lst,
                                    Trade = null,
                                    Status = lst.Status,
                                    CreatedAt = DateTime.UtcNow
                                };
                                
                                await db.History.AddAsync(history, cancellationToken: stoppingToken);
                                await db.SaveChangesAsync(stoppingToken);
                                await notifyService.NotifyMarketUpdateAsync();
                            }
                            break;
                        }
                        case ListingSoldEvent e:
                        {
                            var lst = await db.Listings.FirstOrDefaultAsync(l =>
                                l.ListingId == e.Id, cancellationToken: stoppingToken);

                            if (lst is not null)
                            {
                                if(await db.Listings
                                       .AsNoTracking()
                                       .AnyAsync(l => l.ListingId == e.Id 
                                                      && l.Status == EventStatus.ListingSold,
                                           cancellationToken: stoppingToken))
                                    break;
                                
                                lst.BuyerAddress = e.Buyer;
                                lst.Status = EventStatus.ListingSold;
                                
                                var historySeller = new History
                                {
                                    EventMetadata = new()
                                    {
                                        BlockNumber = pe.BlockNumber,
                                        BlockHash = pe.BlockHash,
                                        Timestamp = pe.BlockTimestamp,
                                        TransactionHash = pe.TransactionHash
                                    },
                                    FromAddress = lst.SellerAddress,
                                    ToAddress = lst.BuyerAddress,
                                    ListingId = lst.ListingId,
                                    TradeId = null,
                                    Listing = lst,
                                    Trade = null,
                                    Status = lst.Status,
                                    CreatedAt = DateTime.UtcNow
                                };
                                
                                var historyBuyer = new History
                                {
                                    EventMetadata = new()
                                    {
                                        BlockNumber = pe.BlockNumber,
                                        BlockHash = pe.BlockHash,
                                        Timestamp = pe.BlockTimestamp,
                                        TransactionHash = pe.TransactionHash
                                    },
                                    FromAddress = lst.BuyerAddress,
                                    ToAddress = lst.SellerAddress,
                                    ListingId = lst.ListingId,
                                    TradeId = null,
                                    Listing = lst,
                                    Trade = null,
                                    Status = EventStatus.ListingBought,
                                    CreatedAt = DateTime.UtcNow
                                };

                                await db.History.AddAsync(historySeller, cancellationToken: stoppingToken);
                                await db.History.AddAsync(historyBuyer, cancellationToken: stoppingToken);
                                await db.SaveChangesAsync(stoppingToken);
                                
                                await notifyService
                                    .NotifyAsync(lst.SellerAddress,
                                        EventStatus.ListingSold,
                                        "Listing sold",
                                        $"Your listing #{lst.ListingId} was bought by {lst.BuyerAddress} for {lst.Price} MON");
                                
                                await notifyService
                                    .NotifyAsync(lst.BuyerAddress,
                                        EventStatus.ListingBought,
                                        "Listing sold",
                                        $"Listing #{lst.ListingId} was bought by you for {lst.Price} MON");
                                
                                await notifyService.NotifyMarketUpdateAsync();
                            }

                            break;
                        }
                        case TradeCreatedEvent e:
                        {
                            if(await db.Trades
                                   .AsNoTracking()
                                   .AnyAsync(t => t.TradeId == e.TradeId, cancellationToken: stoppingToken))
                                break;
                            
                            var tradeData =
                                await _sepoliaService.GetTradeDataAsync(e.TradeId, cancellationToken: stoppingToken);
                            
                            if(tradeData.To.User.Equals(AddressUtil.ZERO_ADDRESS) ||
                               tradeData.From.User.Equals(AddressUtil.ZERO_ADDRESS))
                                break;

                            var listings = await db.Listings
                                .Where(l => e.ListingIds.Contains(l.ListingId))
                                .ToListAsync(cancellationToken: stoppingToken);
                            
                            var trade = new Trade
                            {
                                TradeId = e.TradeId,
                                ListingIds = e.ListingIds,
                                From = new Peer
                                {
                                    Address = tradeData.From.User,
                                    TokenIds = tradeData.From.TokenIds,
                                    NftContracts = tradeData.From.NftContracts
                                },
                                To = new Peer
                                {
                                    Address = tradeData.To.User,
                                    TokenIds = tradeData.To.TokenIds,
                                    NftContracts = tradeData.To.NftContracts
                                },
                                Status = EventStatus.TradeCreated,
                                Listings = listings
                            };
                            
                            var historyFrom = new History
                            {
                                EventMetadata = new()
                                {
                                    BlockNumber = pe.BlockNumber,
                                    BlockHash = pe.BlockHash,
                                    Timestamp = pe.BlockTimestamp,
                                    TransactionHash = pe.TransactionHash
                                },
                                FromAddress = trade.From.Address,
                                ToAddress = trade.To.Address,
                                ListingId = null,
                                Listing = null,
                                Trade = trade, 
                                TradeId = trade.TradeId,
                                Status = trade.Status,
                                CreatedAt = DateTime.UtcNow
                            };
                            
                            var historyTo = new History
                            {
                                EventMetadata = new()
                                {
                                    BlockNumber = pe.BlockNumber,
                                    BlockHash = pe.BlockHash,
                                    Timestamp = pe.BlockTimestamp,
                                    TransactionHash = pe.TransactionHash
                                },
                                FromAddress = trade.To.Address,
                                ToAddress = trade.From.Address,
                                ListingId = null,
                                TradeId = trade.TradeId,
                                Listing = null,
                                Trade = trade, 
                                Status = EventStatus.TradeReceived,
                                CreatedAt = DateTime.UtcNow
                            };

                            await db.Trades.AddAsync(trade, cancellationToken: stoppingToken);
                            await db.History.AddAsync(historyFrom, cancellationToken: stoppingToken);
                            await db.History.AddAsync(historyTo, cancellationToken: stoppingToken);
                            await db.SaveChangesAsync(stoppingToken);
                            
                            await notifyService.NotifyAsync(trade.From.Address.ToLowerInvariant(),
                                EventStatus.TradeCreated,
                                "Trade created",
                                $"You create trade #{trade.TradeId}, second peer of trade - {trade.From.Address.ToLowerInvariant()}");
                            
                            await notifyService.NotifyAsync(trade.To.Address.ToLowerInvariant(),
                                EventStatus.TradeReceived,
                                "Incoming trade",
                                $"You received trade #{trade.TradeId} from {trade.From.Address}");
                            break;
                        }
                        case TradeAcceptedEvent e:
                        {
                            if(await db.Trades
                                   .AsNoTracking()
                                   .AnyAsync(t => t.TradeId == e.TradeId &&
                                       t.Status == EventStatus.TradeAccepted,
                                       cancellationToken: stoppingToken))
                                break;
                            
                            await CloseTradeAsync(e.TradeId, db, notifyService,
                                EventStatus.TradeAccepted,
                                pe,
                                stoppingToken);
                            
                            await notifyService.NotifyMarketUpdateAsync();
                            break;
                        }
                        case TradeCompletedEvent e:
                        {
                            if(await db.Trades
                                   .AsNoTracking()
                                   .AnyAsync(t => t.TradeId == e.TradeId &&
                                                  (int)t.Status == (int)EventStatus.TradeCompleted,
                                       cancellationToken: stoppingToken))
                                break;
                            
                            await CloseTradeAsync(e.TradeId, db, notifyService,
                                EventStatus.TradeCompleted,
                                pe,
                                stoppingToken);
                            
                            break;
                        }
                        case TradeRejectedEvent e:
                        {
                            if(await db.Trades
                                   .AsNoTracking()
                                   .AnyAsync(t => t.TradeId == e.TradeId &&
                                                  (int)t.Status == (int)EventStatus.TradeRejected,
                                       cancellationToken: stoppingToken))
                                break;
                            
                            await CloseTradeAsync(e.TradeId, db, notifyService,
                                EventStatus.TradeRejected,
                                pe,
                                stoppingToken);
                            
                            break;
                        }
                    }
                }
            }

            await Task.Delay(200, cancellationToken: stoppingToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogCritical($"Operation cancelled exception: {ex}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected exception: {ex}");
        }
    }
    private static async Task CloseTradeAsync(
        BigInteger tradeId,
        ApiDbContext db,
        INotificationService notifyService,
        EventStatus status,
        ParsedEvent pe,
        CancellationToken stoppingToken)
    {
        var trade = await db.Trades
            .Include(t => t.Listings)
            .FirstOrDefaultAsync(t => t.TradeId == tradeId,
            cancellationToken: stoppingToken);

        if (trade is null) return;
        
        trade.Status = status;

        await db.History.AddRangeAsync(new List<History>
        {
            new()
            {
                FromAddress = trade.From.Address,
                ToAddress = trade.To.Address,
                EventMetadata = new()
                {
                    BlockNumber = pe.BlockNumber,
                    BlockHash = pe.BlockHash,
                    Timestamp = pe.BlockTimestamp,
                    TransactionHash = pe.TransactionHash
                },
                ListingId = null,
                TradeId = tradeId,
                Listing = null,
                Trade = trade,
                Status = status,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                FromAddress = trade.To.Address,
                ToAddress = trade.From.Address,
                EventMetadata = new()
                {
                    BlockNumber = pe.BlockNumber,
                    BlockHash = pe.BlockHash,
                    Timestamp = pe.BlockTimestamp,
                    TransactionHash = pe.TransactionHash
                },
                ListingId = null,
                TradeId = tradeId,
                Listing = null,
                Trade = trade,
                Status = status,
                CreatedAt = DateTime.UtcNow
            }
        }, stoppingToken);

        await db.SaveChangesAsync(stoppingToken);

        var notifications = status switch
        {
            EventStatus.TradeAccepted => new[]
            {
                (trade.From.Address.ToLowerInvariant(), status, "Trade accepted",
                    $"Your trade #{trade.Id} has been accepted"),
                (trade.To.Address.ToLowerInvariant(), status, "Trade accepted", $"Trade #{trade.Id} has been accepted")
            },
            EventStatus.TradeCompleted => new[]
            {
                (trade.From.Address.ToLowerInvariant(), status, "Trade completed",
                    $"Your trade #{trade.TradeId} final preparations for trade confirmation"),
                (trade.To.Address.ToLowerInvariant(), status, "Trade completed",
                    $"Trade #{trade.TradeId} final preparations for trade confirmation")
            },
            EventStatus.TradeRejected => new[]
            {
                (trade.From.Address.ToLowerInvariant(), status, "Trade rejected",
                    $"Your trade #{trade.TradeId} has been rejected by second side"),
                (trade.To.Address.ToLowerInvariant(), status, "Trade rejected",
                    $"Trade #{trade.TradeId} has been rejected by second side")
            },
            _ => Array.Empty<(string Recipient, EventStatus Status, string Title, string Body)>()
        };

        await Task.WhenAll(notifications.Select(n =>
            notifyService.NotifyAsync(n.Item1, n.Item2, n.Item3, n.Item4)));
    }
}