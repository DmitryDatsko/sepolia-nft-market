using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.MoralisResponse;

public class ListPrice
{
    [JsonProperty("listed")]
    public bool? Listed { get; set; }

    [JsonProperty("price")]
    public string Price { get; set; }

    [JsonProperty("price_currency")]
    public string PriceCurrency { get; set; }

    [JsonProperty("price_usd")]
    public string PriceUsd { get; set; }

    [JsonProperty("marketplace")]
    public string Marketplace { get; set; }
}