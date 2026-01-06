using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class TokenUri
{
    [JsonProperty("gateway")]
    public string Gateway { get; set; } = string.Empty;

    [JsonProperty("raw")]
    public string Raw { get; set; } = string.Empty;
}