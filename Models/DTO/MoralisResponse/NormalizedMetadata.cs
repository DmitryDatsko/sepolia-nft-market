using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.MoralisResponse;

public class NormalizedMetadata
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("animation_url")]
    public object AnimationUrl { get; set; }

    [JsonProperty("external_link")]
    public object ExternalLink { get; set; }

    [JsonProperty("external_url")]
    public object ExternalUrl { get; set; }

    [JsonProperty("image")]
    public string Image { get; set; }

    [JsonProperty("attributes")]
    public List<Attribute> Attributes { get; set; }
}