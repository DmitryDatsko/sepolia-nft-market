using System.Text.Json.Serialization;

namespace SepoliaNftMarket.Models.DTO.Etherscan;

public class EtherscanLog
{
    [JsonPropertyName("address")] public string Address { get; set; } = string.Empty;
    [JsonPropertyName("topics")] public List<string> Topics { get; set; } = [];
    [JsonPropertyName("data")] public string Data { get; set; } = string.Empty;
    [JsonPropertyName("blockNumber")] public string BlockNumber { get; set; } = string.Empty;
    [JsonPropertyName("blockHash")] public string BlockHash { get; set; } = string.Empty;
    [JsonPropertyName("timeStamp")] public string TimeStamp { get; set; } = string.Empty;
    [JsonPropertyName("gasPrice")] public string GasPrice { get; set; } = string.Empty;
    [JsonPropertyName("gasUsed")] public string GasUsed { get; set; } = string.Empty;
    [JsonPropertyName("logIndex")] public string LogIndex { get; set; } = string.Empty;
    [JsonPropertyName("transactionHash")] public string TransactionHash { get; set; } = string.Empty;
    [JsonPropertyName("transactionIndex")] public string TransactionIndex { get; set; } = string.Empty;
}