using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class UserNftMetadata
{
    [JsonProperty("ownedNfts")] public List<OwnedNft> OwnedNfts { get; set; } = [];

    [JsonProperty("pageKey")]
    public object PageKey { get; set; } = string.Empty;

    [JsonProperty("totalCount")]
    public int TotalCount { get; set; }

    [JsonProperty("validAt")] public ValidAt ValidAt { get; set; } = new();
}