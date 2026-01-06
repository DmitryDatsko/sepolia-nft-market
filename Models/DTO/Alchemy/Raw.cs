using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class Raw
{
    [JsonProperty("error")]
    public object Error { get; set; } = string.Empty;

    [JsonProperty("metadata")] public Metadata Metadata { get; set; } = new();

    [JsonProperty("tokenUri")]
    public string TokenUri { get; set; } = string.Empty;
}