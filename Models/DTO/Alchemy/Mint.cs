using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class Mint
{
    [JsonProperty("blockNumber")]
    public object BlockNumber { get; set; } = string.Empty;

    [JsonProperty("mintAddress")]
    public object MintAddress { get; set; } = string.Empty;

    [JsonProperty("timestamp")]
    public object Timestamp { get; set; } = string.Empty;

    [JsonProperty("transactionHash")] public object TransactionHash { get; set; } = string.Empty;
}