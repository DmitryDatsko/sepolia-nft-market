using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class TokenMetadata
{
    [JsonProperty("tokenType")]
    public string TokenType { get; set; } = string.Empty;
}