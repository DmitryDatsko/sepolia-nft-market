using System.Numerics;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SepoliaNftMarket.Configuration;
using SepoliaNftMarket.Models;
using SepoliaNftMarket.Models.DTO;
using SepoliaNftMarket.Models.DTO.Alchemy;

namespace SepoliaNftMarket.Providers.Alchemy;

public class AlchemyProvider : IAlchemyProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AlchemyProvider> _logger;
    private readonly string _alchemyUrl;
    private readonly string _alchemyApiKey;

    public AlchemyProvider(
        IHttpClientFactory factory,
        IOptions<EnvVariables> env,
        ILogger<AlchemyProvider> logger,
        IMemoryCache cache)
    {
        var envValue = env.Value;
        
        _httpClient = factory.CreateClient();
        _cache = cache;
        _logger = logger;
        _alchemyUrl = envValue.AlchemyUrl;
        _alchemyApiKey = envValue.AlchemyApiKey;
    }
    
    public async Task<NftMetadata> GetNftMetadataAsync(string address, string tokenId)
    {
        var query = new Dictionary<string, string>
        {
            ["contractAddress"] = address,
            ["tokenId"] = tokenId,
            ["refreshCache"] = "false"
        };
    
        var requestUrl = QueryHelpers.AddQueryString($"{_alchemyUrl}{_alchemyApiKey}/getNFTMetadata", query);
        using var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
    
        _logger.LogInformation($"Metadata request: {requestUrl}");
    
        resp.EnsureSuccessStatusCode();
        var content = await resp.Content.ReadAsStringAsync();
    
        _logger.LogInformation($"Metadata response: {content}");

        var json = Newtonsoft.Json.Linq.JObject.Parse(content);

        var name = json["name"]?.ToString() ?? string.Empty;
        var description = json["description"]?.ToString() ?? string.Empty;
        var tokenType = json["tokenType"]?.ToString() ?? string.Empty;
        var imageUrl = json["image"]?["cachedUrl"]?.ToString() 
                       ?? json["image"]?["originalUrl"]?.ToString() 
                       ?? string.Empty;

        return new NftMetadata
        {
            TokenId = BigInteger.Parse(tokenId),
            NftContractAddress = address,
            Kind = tokenType,
            Name = name,
            ImageOriginal = GetImageUrl(imageUrl),
            Description = description
        };
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

    public async Task<List<UserToken>> GetUserTokensAsync(string address, bool sortByDesc)
    {
        _logger.LogInformation($"Getting user tokens for {address}");
        var cacheKey = $"{nameof(GetUserTokensAsync)}_{address}_{sortByDesc}";

        if (_cache.TryGetValue(cacheKey, out List<UserToken>? cached))
        {
            if (cached != null)
                return cached;
        }
        
        var query = new Dictionary<string, string>
        {
            ["owner"] = address,
            ["withMetadata"] = "true",
            ["refreshCache"] = "false"
        };

        var baseUrl = $"{_alchemyUrl}{_alchemyApiKey}/getNFTsForOwner";
        var requestUrl = QueryHelpers.AddQueryString(baseUrl, query);
        using var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        
        resp.EnsureSuccessStatusCode();
        
        var content = await resp.Content.ReadAsStringAsync();

        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        var data = JsonConvert.DeserializeObject<UserNftMetadata>(content, settings);

        if (data == null) return new();
        
        var result = new List<UserToken>();

        foreach (var dt in data.OwnedNfts)
        {
            var metadata = await GetNftMetadataAsync(dt.Contract.Address, dt.TokenId);
            var ts = ParseTimestamp(dt.Mint?.Timestamp);
            
            result.Add(new UserToken
            {
                ContractAddress = dt.Contract.Address,
                TokenId = dt.TokenId,
                Kind = dt.TokenType,
                Name = dt.Name,
                ImageOriginal = metadata.ImageOriginal,
                Description = metadata.Description,
                Price = metadata.Price,
                AcquiredAt = ts > 0 ? 
                    DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime :
                    DateTime.MinValue
            });
            
            await Task.Delay(100);
        }
        
        _cache.Set(
            cacheKey,
            result,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15),
                SlidingExpiration = TimeSpan.FromSeconds(5),
                Priority = CacheItemPriority.Normal
            });
        
        return result;
    }

    private static string GetKey(string contract, string tokenId) =>
        $"{contract.ToLowerInvariant()}:{tokenId}";
    
    private static string GetImageUrl(string url) =>
        url.Replace("ipfs://", "https://ipfs.io/ipfs/");

    private static long ParseTimestamp(object? timestampObj)
    {
        if (timestampObj == null) return 0;

        if (timestampObj is JsonElement je)
        {
            if(je.ValueKind == JsonValueKind.Number && je.TryGetInt64(out var n)) return n;
            if (je.ValueKind == JsonValueKind.String && long.TryParse(je.GetString(), out var s)) return s;
            return 0;
        }

        if (timestampObj is long l) return l;
        if (timestampObj is int i) return i; 
        if (timestampObj is string str && long.TryParse(str, out var parsed)) return parsed;
        
        return 0;
    }
}