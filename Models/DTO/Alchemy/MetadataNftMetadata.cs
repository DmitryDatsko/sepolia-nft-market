using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class MetadataNftMetadata
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("image")]
    public string Image { get; set; } = string.Empty;
}