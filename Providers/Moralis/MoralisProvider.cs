using System.Numerics;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SepoliaNftMarket.Configuration;
using SepoliaNftMarket.Models;
using SepoliaNftMarket.Models.DTO.MoralisResponse;

namespace SepoliaNftMarket.Providers.Moralis;

public class MoralisProvider : IMoralisProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public MoralisProvider(
        IHttpClientFactory factory,
        IOptions<EnvVariables> env)
    {
        _httpClient = factory.CreateClient();
        var envValue = env.Value;
        
        _apiKey = envValue.MoralisApiKey;
        _baseUrl = envValue.MoralisUrl;
    }
    
    public async Task<NftMetadata> GetNftMetadataAsync(string address, string tokenId)
    {
        var query = new Dictionary<string, string>
        {
            ["chain"] = "sepolia",
            ["format"] = "decimal",
            ["include_prices"] = "true",
        };
        
        var requestUrl = QueryHelpers.AddQueryString($"{_baseUrl}{address}/{tokenId}", query);
        
        using var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        req.Headers.Add("X-API-Key", _apiKey);
        
        using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        resp.EnsureSuccessStatusCode();
        
        var data = JsonSerializer.Deserialize<MoralisNftMetadata>(await resp.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive  = true });

        if (data != null)
        {
            var nftMetadate = new NftMetadata
            {
                TokenId = BigInteger.Parse(tokenId),
                NftContractAddress = address,
                Kind = data.ContractType,
                Name = data.Name,
                ImageOriginal = data.NormalizedMetadata.Image,
                Description = data.NormalizedMetadata.Description,
                Price = decimal.Parse(data.FloorPriceUsd),
            };
        }

        return new();
    }

    public async Task<IReadOnlyDictionary<string, NftMetadata>> GetNftsMetadataAsync(List<string> addresses, List<BigInteger> tokenIds)
    {
        if (addresses.Count != tokenIds.Count) return new Dictionary<string, NftMetadata>();
        
        var dict = new Dictionary<string, NftMetadata>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < addresses.Count; i++)
        {
            var metadata = await GetNftMetadataAsync(addresses[i], tokenIds[i].ToString());
            dict[GetKey(addresses[i], tokenIds[i].ToString())] = metadata;
            await Task.Delay(100);
        }
            
        return dict;
    }
    
    private static string GetKey(string contract, string tokenId) =>
        $"{contract.ToLowerInvariant()}:{tokenId}";
}