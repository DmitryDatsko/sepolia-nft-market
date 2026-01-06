using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class AcquiredAt
{
    [JsonProperty("blockNumber")] public object BlockNumber { get; set; } = string.Empty;

    [JsonProperty("blockTimestamp")]
    public object BlockTimestamp { get; set; } = string.Empty;
}