using Newtonsoft.Json;

namespace SepoliaNftMarket.Models.DTO.MoralisResponse;

public class PaymentToken
{
    [JsonProperty("token_name")]
    public string TokenName { get; set; }

    [JsonProperty("token_symbol")]
    public string TokenSymbol { get; set; }

    [JsonProperty("token_logo")]
    public string TokenLogo { get; set; }

    [JsonProperty("token_decimals")]
    public string TokenDecimals { get; set; }

    [JsonProperty("token_address")]
    public string TokenAddress { get; set; }
}