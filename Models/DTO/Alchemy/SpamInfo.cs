using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class SpamInfo
{
    [JsonProperty("isSpam")]
    public string IsSpam { get; set; } = string.Empty;

    [JsonProperty("classifications")] public List<object> Classifications { get; set; } = [];
}