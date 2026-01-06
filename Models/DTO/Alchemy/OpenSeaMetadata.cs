using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class OpenSeaMetadata
{
    [JsonProperty("bannerImageUrl")]
    public object BannerImageUrl { get; set; } = string.Empty;

    [JsonProperty("collectionName")]
    public object CollectionName { get; set; } = string.Empty;

    [JsonProperty("collectionSlug")]
    public object CollectionSlug { get; set; } = string.Empty;

    [JsonProperty("description")]
    public object Description { get; set; } = string.Empty;

    [JsonProperty("discordUrl")]
    public object DiscordUrl { get; set; } = string.Empty;

    [JsonProperty("externalUrl")]
    public object ExternalUrl { get; set; } = string.Empty;

    [JsonProperty("floorPrice")]
    public object FloorPrice { get; set; } = string.Empty;

    [JsonProperty("imageUrl")]
    public object ImageUrl { get; set; } = string.Empty;

    [JsonProperty("lastIngestedAt")]
    public object LastIngestedAt { get; set; } = string.Empty;

    [JsonProperty("safelistRequestStatus")]
    public object SafelistRequestStatus { get; set; } = string.Empty;

    [JsonProperty("twitterUsername")]
    public object TwitterUsername { get; set; } = string.Empty;
}