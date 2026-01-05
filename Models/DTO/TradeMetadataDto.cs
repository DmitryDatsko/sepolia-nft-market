using System.Numerics;

namespace SepoliaNftMarket.Models.DTO;

public class TradeMetadataDto
{
    public BigInteger ListingId { get; set; }
    public required string Kind { get; set; }
    public string NftContractAddress { get; set; } = string.Empty;
    public BigInteger TokenId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string ImageOriginal { get; set; }
    public decimal Price { get; set; }
}