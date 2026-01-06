using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class OwnedNft
{
    [JsonProperty("acquiredAt")] public AcquiredAt AcquiredAt { get; set; } = new();

    [JsonProperty("animation")] public Animation Animation { get; set; } = new();

    [JsonProperty("balance")]
    public string Balance { get; set; } = string.Empty;

    [JsonProperty("collection")]
    public object Collection { get; set; } = string.Empty;

    [JsonProperty("contract")] public ContractUser Contract { get; set; } = new();

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("image")] public Image Image { get; set; } = new();

    [JsonProperty("mint")] public Mint Mint { get; set; } = new();

    [JsonProperty("name")] public string Name { get; set; } = string.Empty;

    [JsonProperty("owners")]
    public object Owners { get; set; } = string.Empty;

    [JsonProperty("raw")] public Raw Raw { get; set; } = new();

    [JsonProperty("timeLastUpdated")]
    public DateTime TimeLastUpdated { get; set; }

    [JsonProperty("tokenId")]
    public string TokenId { get; set; } = string.Empty;

    [JsonProperty("tokenType")]
    public string TokenType { get; set; } = string.Empty;

    [JsonProperty("tokenUri")]
    public string TokenUri { get; set; } = string.Empty;

}