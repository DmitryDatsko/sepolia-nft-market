using System.Numerics;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using Polly;
using SepoliaNftMarket.Configuration;
using SepoliaNftMarket.Models.ContractFunctions;
using SepoliaNftMarket.Models.ContractOutput;

namespace SepoliaNftMarket.Services.Sepolia;

public class SepoliaService : ISepoliaService
{
    private static readonly Random Jitterer = new();
    private readonly Web3 _web3;
    private readonly string _contractAddress;
    private readonly ILogger<SepoliaService> _logger;
    private readonly AsyncPolicy<GetTradeOutput> _genericPolicy;
    public SepoliaService(IOptions<EnvVariables> env,
        ILogger<SepoliaService> logger)
    {
        var sepoliaRpc = env.Value.SepoliaRpcUrl;
        _web3 = new Web3($"{sepoliaRpc}{env.Value.InfuraApiKey}");
        _contractAddress = env.Value.ContractAddress;
        _logger = logger;

        _genericPolicy = Policy<GetTradeOutput>
            .Handle<Nethereum.JsonRpc.Client.RpcClientUnknownException>()
            .Or<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1) + Jitterer.Next(0, 100)),
                onRetry: (outcome, timespan, attempt) =>
                {
                    if (outcome.Exception != null)
                        _logger.LogWarning(outcome.Exception, "Retry {Attempt} due to exception. Waiting {Delay}", attempt, timespan);
                    else
                        _logger.LogWarning("Retry {Attempt} due to unsuccessful result. Waiting {Delay}. Result: {Result}", attempt, timespan, outcome.Result);
                });
    }

    public async Task<GetTradeOutput> GetTradeDataAsync(BigInteger tradeId,
        CancellationToken cancellationToken = default)
    {
        return await _genericPolicy.ExecuteAsync(async ct =>
        {
            ct.ThrowIfCancellationRequested();

            var abi = await File.ReadAllTextAsync("Services/Sepolia/abi.json", ct);
            var contract = _web3.Eth.GetContract(abi, _contractAddress);

            var func = contract.GetFunction<GetTradeFunction>();

            var function = contract.GetFunction("trades");
            var callInput = function.CreateCallInput(tradeId);
            _logger.LogInformation("Trade id from monad service: {id}", tradeId);

            var rawHex = await _web3.Eth.Transactions.Call.SendRequestAsync(callInput);
            _logger.LogDebug("Raw result hex: {RawHex}", rawHex);

            var getTradeMsg = new GetTradeFunction { TradeId = tradeId };

            GetTradeOutput trade;
            try
            {
                trade = await func.CallDeserializingToObjectAsync<GetTradeOutput>(getTradeMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in deserialization of trade data for trade {TradeId}", tradeId);
                throw;
            }

            return trade;
        }, cancellationToken);
    }

    public async Task<string> GetTransactionInitiatorAsync(string txHash)
    {
        var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txHash);

        return transaction.From;
    }
    
    public async Task<string> GetTransactionReciverAsync(string txHash)
    {
        var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txHash);

        return transaction.To;
    }
}