using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class Attribute
{
    [JsonProperty("trait_type")]
    public string TraitType { get; set; } = string.Empty;

    [JsonProperty("value")]
    public object Value { get; set; } = string.Empty;

    [JsonProperty("display_type")]
    public string DisplayType { get; set; } = string.Empty;
}