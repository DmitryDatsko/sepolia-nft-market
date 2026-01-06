using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class ContractMetadata
{
    [JsonProperty("name")] public string Name { get; set; } = string.Empty;

    [JsonProperty("symbol")] public string Symbol { get; set; } = string.Empty;

    [JsonProperty("totalSupply")] public string TotalSupply { get; set; } = string.Empty;

    [JsonProperty("tokenType")] public string TokenType { get; set; } = string.Empty;

    [JsonProperty("openSea")] public OpenSea OpenSea { get; set; } = new();
}