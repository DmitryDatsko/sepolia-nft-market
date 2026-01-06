using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class Image
{
    [JsonProperty("cachedUrl")]
    public string CachedUrl { get; set; } = string.Empty;

    [JsonProperty("contentType")]
    public object ContentType { get; set; } = string.Empty;

    [JsonProperty("originalUrl")]
    public string OriginalUrl { get; set; } = string.Empty;

    [JsonProperty("pngUrl")]
    public object PngUrl { get; set; } = string.Empty;

    [JsonProperty("size")]
    public object Size { get; set; } = string.Empty;

    [JsonProperty("thumbnailUrl")]
    public object ThumbnailUrl { get; set; } = string.Empty;
}