using System.Text.Json.Serialization;

namespace SepoliaNftMarket.Models.DTO.Etherscan;

public class EventLog
{
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
    [JsonPropertyName("result")] public List<EtherscanLog> Result { get; set; } = [];
}