using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class ContractUser
{
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("contractDeployer")]
    public object ContractDeployer { get; set; } = string.Empty;

    [JsonProperty("deployedBlockNumber")]
    public object DeployedBlockNumber { get; set; } = string.Empty;

    [JsonProperty("isSpam")]
    public bool IsSpam { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("openSeaMetadata")] public OpenSeaMetadata OpenSeaMetadata { get; set; } = new();

    [JsonProperty("spamClassifications")] public List<object> SpamClassifications { get; set; } = new();

    [JsonProperty("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonProperty("tokenType")]
    public string TokenType { get; set; } = string.Empty;

    [JsonProperty("totalSupply")]
    public string TotalSupply { get; set; } = string.Empty;
}