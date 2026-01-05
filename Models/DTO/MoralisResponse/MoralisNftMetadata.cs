using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.MoralisResponse;

public class MoralisNftMetadata
{
    [JsonProperty("amount")] public string Amount { get; set; }

    [JsonProperty("token_id")] public string TokenId { get; set; }

    [JsonProperty("token_address")] public string TokenAddress { get; set; }

    [JsonProperty("contract_type")] public string ContractType { get; set; }

    [JsonProperty("owner_of")] public string OwnerOf { get; set; }

    [JsonProperty("last_metadata_sync")] public DateTime? LastMetadataSync { get; set; }

    [JsonProperty("last_token_uri_sync")] public DateTime? LastTokenUriSync { get; set; }

    [JsonProperty("metadata")] public string Metadata { get; set; }

    [JsonProperty("block_number")] public string BlockNumber { get; set; }

    [JsonProperty("block_number_minted")] public object BlockNumberMinted { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("symbol")] public string Symbol { get; set; }

    [JsonProperty("token_hash")] public string TokenHash { get; set; }

    [JsonProperty("token_uri")] public string TokenUri { get; set; }

    [JsonProperty("minter_address")] public object MinterAddress { get; set; }

    [JsonProperty("rarity_rank")] public int? RarityRank { get; set; }

    [JsonProperty("rarity_percentage")] public double? RarityPercentage { get; set; }

    [JsonProperty("rarity_label")] public string RarityLabel { get; set; }

    [JsonProperty("verified_collection")] public bool? VerifiedCollection { get; set; }

    [JsonProperty("possible_spam")] public bool? PossibleSpam { get; set; }

    [JsonProperty("last_sale")] public LastSale LastSale { get; set; }

    [JsonProperty("normalized_metadata")] public NormalizedMetadata NormalizedMetadata { get; set; }

    [JsonProperty("collection_logo")] public string CollectionLogo { get; set; }

    [JsonProperty("collection_banner_image")]
    public string CollectionBannerImage { get; set; }

    [JsonProperty("collection_category")] public string CollectionCategory { get; set; }

    [JsonProperty("project_url")] public string ProjectUrl { get; set; }

    [JsonProperty("wiki_url")] public string WikiUrl { get; set; }

    [JsonProperty("discord_url")] public string DiscordUrl { get; set; }

    [JsonProperty("telegram_url")] public string TelegramUrl { get; set; }

    [JsonProperty("twitter_username")] public string TwitterUsername { get; set; }

    [JsonProperty("instagram_username")] public string InstagramUsername { get; set; }

    [JsonProperty("list_price")] public ListPrice ListPrice { get; set; }

    [JsonProperty("floor_price")] public string FloorPrice { get; set; }

    [JsonProperty("floor_price_usd")] public string FloorPriceUsd { get; set; }

    [JsonProperty("floor_price_currency")] public string FloorPriceCurrency { get; set; }
}