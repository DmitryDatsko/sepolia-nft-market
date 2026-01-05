using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.MoralisResponse;

public class LastSale
{
    [JsonProperty("transaction_hash")]
    public string TransactionHash { get; set; }

    [JsonProperty("block_timestamp")]
    public DateTime? BlockTimestamp { get; set; }

    [JsonProperty("buyer_address")]
    public string BuyerAddress { get; set; }

    [JsonProperty("seller_address")]
    public string SellerAddress { get; set; }

    [JsonProperty("price")]
    public string Price { get; set; }

    [JsonProperty("price_formatted")]
    public string PriceFormatted { get; set; }

    [JsonProperty("usd_price_at_sale")]
    public string UsdPriceAtSale { get; set; }

    [JsonProperty("current_usd_value")]
    public string CurrentUsdValue { get; set; }

    [JsonProperty("token_address")]
    public string TokenAddress { get; set; }

    [JsonProperty("token_id")]
    public string TokenId { get; set; }

    [JsonProperty("payment_token")]
    public PaymentToken PaymentToken { get; set; }
}