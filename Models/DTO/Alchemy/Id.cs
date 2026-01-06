using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class Id
{
    [JsonProperty("tokenId")]
    public string TokenId { get; set; } = string.Empty;

    [JsonProperty("tokenMetadata")] public TokenMetadata TokenMetadata { get; set; } = new();
}