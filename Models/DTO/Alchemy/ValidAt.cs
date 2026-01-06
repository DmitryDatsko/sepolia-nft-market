using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class ValidAt
{
    [JsonProperty("blockHash")]
    public string BlockHash { get; set; } = string.Empty;

    [JsonProperty("blockNumber")]
    public int BlockNumber { get; set; }

    [JsonProperty("blockTimestamp")]
    public DateTime BlockTimestamp { get; set; }
}