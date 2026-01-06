using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class ContractNftMetadata
{
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;
}