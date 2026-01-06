using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class Animation
{
    [JsonProperty("cachedUrl")]
    public object CachedUrl { get; set; } = string.Empty;

    [JsonProperty("contentType")]
    public object ContentType { get; set; } = string.Empty;

    [JsonProperty("originalUrl")]
    public object OriginalUrl { get; set; } = string.Empty;

    [JsonProperty("size")]
    public object Size { get; set; } = string.Empty;
}