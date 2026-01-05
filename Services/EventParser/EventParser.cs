using Microsoft.Extensions.Options;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using SepoliaNftMarket.Configuration;
using SepoliaNftMarket.Models.ContractEvents;
using SepoliaNftMarket.Models.DTO.Etherscan;

namespace SepoliaNftMarket.Services.EventParser;

public class EventParser : IEventParser
{
    private readonly Dictionary<string, Func<FilterLog, IEventDTO>> _decoders;
    private readonly Web3 _web3;
    private readonly string _contractAddress;
    private readonly ILogger<EventParser> _logger;
    
    public EventParser(
        IOptions<EnvVariables> env,
        ILogger<EventParser> logger)
    {
        var envValue = env.Value;
        _contractAddress = envValue.ContractAddress;
        _web3 = new Web3($"{envValue.SepoliaRpcUrl}{envValue.InfuraApiKey}");
        _logger = logger;
        
        _decoders = new Dictionary<string, Func<FilterLog, IEventDTO>>(StringComparer.OrdinalIgnoreCase);
        
        Register<ListingCreatedEvent>();
        Register<ListingRemovedEvent>();
        Register<ListingSoldEvent>();
        Register<TradeAcceptedEvent>();
        Register<TradeCompletedEvent>();
        Register<TradeCreatedEvent>();
        Register<TradeRejectedEvent>();
    }
    
    private void Register<T>() where T : IEventDTO, new()
    {
        var ev = _web3.Eth.GetEvent<T>(_contractAddress);
        var sig = ev.EventABI.Sha3Signature.EnsureHexPrefix();

        var topicDecoder = new EventTopicDecoder();
        
        _decoders[sig] = log => topicDecoder.DecodeTopics<T>(log.Topics, log.Data);
    }

    public IEventDTO? ParseEvent(EtherscanLog log)
    {
        var topics = new List<object?>();
        if(!string.IsNullOrEmpty(log.Topics[0])) topics.Add(log.Topics[0]);
        if(!string.IsNullOrEmpty(log.Topics[1])) topics.Add(log.Topics[1]);
        if(!string.IsNullOrEmpty(log.Topics[2])) topics.Add(log.Topics[2]);
        if(!string.IsNullOrEmpty(log.Topics[3])) topics.Add(log.Topics[2]);
        
        var fl = new FilterLog
        {
            Topics = topics.ToArray(),
            Data = log.Data
        };

        if (string.IsNullOrEmpty(log.Topics[0])) return null;

        if (!_decoders.TryGetValue(log.Topics[0], out var decoder)) return null;
        try
        {
            return decoder(fl);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to decode event {log.Topics[0]}: {ex.Message}");
            return null;
        }
    }
}