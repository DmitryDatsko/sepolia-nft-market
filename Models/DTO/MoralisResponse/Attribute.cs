using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.MoralisResponse;

public class Attribute
{
    [JsonProperty("trait_type")]
    public string TraitType { get; set; } = string.Empty;

    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;

    [JsonProperty("display_type")]
    public object DisplayType { get; set; } = string.Empty;

    [JsonProperty("max_value")]
    public object MaxValue { get; set; } = string.Empty;

    [JsonProperty("trait_count")]
    public int TraitCount { get; set; } 

    [JsonProperty("order")]
    public object Order { get; set; } = string.Empty;

    [JsonProperty("rarity_label")]
    public string RarityLabel { get; set; } = string.Empty;

    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("percentage")]
    public double Percentage { get; set; }
}