using System.Numerics;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using SepoliaNftMarket.Configuration;
using SepoliaNftMarket.Models.DTO.Etherscan;

namespace SepoliaNftMarket.Providers.Etherscan;

public class EtherscanProvider : IEtherscanProvider
{
    private readonly HttpClient _httpClient;
    private readonly Web3 _web3;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly int _confirmationBlocks;
    private readonly string _contractAddress;
    
    public EtherscanProvider(
        IHttpClientFactory factory,
        IOptions<EnvVariables> env)
    {
        _httpClient = factory.CreateClient();
        var envValue = env.Value;
        _web3 = new Web3($"{envValue.SepoliaRpcUrl}{envValue.InfuraApiKey}");
        
        _apiKey = envValue.EtherscanApiKey;
        _baseUrl = envValue.EtherscanUrl;
        _confirmationBlocks = envValue.BlocksForConfirmation;
        _contractAddress = envValue.ContractAddress;
    }
    
    public async Task<EventLog> GetEventLogsAsync(BigInteger fromBlock)
    {
        var latestBlock = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

        var query = new Dictionary<string, string>
        {
            ["fromBlock"] = fromBlock.ToString(),
            ["toBlock"] = (latestBlock.Value - _confirmationBlocks).ToString(),
            ["address"] = _contractAddress,
            ["apikey"] = _apiKey
        };
        
        var requestUrl = QueryHelpers.AddQueryString(_baseUrl, query);

        using var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);

        resp.EnsureSuccessStatusCode();
        
        var data = JsonSerializer.Deserialize<EventLog>(await resp.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive  = true });
        
        return data ??  new();
    }

    public async Task<DateTime> GetBlockMintTimeAsync(BigInteger blockNumber)
    {
        var blockTx = await _web3.Eth.Blocks.
            GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(blockNumber));

        var unixTime = (long)blockTx.Timestamp.Value;
        var utcTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
        
        return utcTime;
    }
}