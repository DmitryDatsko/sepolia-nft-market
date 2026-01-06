using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class MetadataUser
{
    [JsonProperty("attributes")] public List<Attribute> Attributes { get; set; } = new();

    [JsonProperty("background_color")]
    public string BackgroundColor { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("external_url")]
    public string ExternalUrl { get; set; } = string.Empty;

    [JsonProperty("image_data")]
    public string ImageData { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("image")]
    public string Image { get; set; } = string.Empty;
}