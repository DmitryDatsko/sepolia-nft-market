using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class AlchemyNftMetadata
{
    [JsonProperty("contract")] public ContractNftMetadata ContractNftMetadata { get; set; } = new();

    [JsonProperty("id")] public Id Id { get; set; } = new();

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonConverter(typeof(TokenUriConverter))]
    [JsonProperty("tokenUri")] 
    public TokenUri TokenUri { get; set; } = new();

    [JsonProperty("media")] public List<Medium> Media { get; set; } = [];

    [JsonProperty("metadata")] public MetadataNftMetadata MetadataNftMetadata { get; set; } = new();

    [JsonProperty("timeLastUpdated")]
    public DateTime TimeLastUpdated { get; set; }

    [JsonProperty("contractMetadata")] public ContractMetadata ContractMetadata { get; set; } = new();

    [JsonProperty("spamInfo")] public SpamInfo SpamInfo { get; set; } = new();
}
