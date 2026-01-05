using System.Numerics;
using Microsoft.EntityFrameworkCore;
using SepoliaNftMarket.Context;
using SepoliaNftMarket.Providers.Moralis;

namespace SepoliaNftMarket.Services.UpdateNftMetadata;

public class UpdateMetadata : IUpdateMetadata
{
    private readonly IMoralisProvider _moralisProvider;
    private readonly ApiDbContext _db;

    public UpdateMetadata(
        IMoralisProvider moralisProvider,
        ApiDbContext db)
    {
        _moralisProvider = moralisProvider;
        _db = db;
    }
    
    public async Task UpdateMetadataAsync(
        List<string> contractAddresses,
        List<BigInteger> tokenIds)
    {
        var metadata = await _moralisProvider
            .GetNftsMetadataAsync(contractAddresses, tokenIds);

        foreach (var data in contractAddresses.Zip(tokenIds))
        {
            var key = MakeKey(data.First, data.Second);
            
            if(!metadata.TryGetValue(key, out var mt))
                continue;
            
            await _db.Listings
                .Where(n => n.NftContractAddress == data.First
                            && n.TokenId == data.Second)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(n => n.NftMetadata.Kind, _ => mt.Kind)
                    .SetProperty(n => n.NftMetadata.Name, _ => mt.Name)
                    .SetProperty(n => n.NftMetadata.ImageOriginal, _ => mt.ImageOriginal)
                    .SetProperty(n => n.NftMetadata.Description, _ => mt.Description)
                    .SetProperty(n => n.NftMetadata.Price, _ => mt.Price ?? 0m)
                    .SetProperty(n => n.NftMetadata.LastUpdated, _ => DateTime.UtcNow));
        }
    }
    
    private static string MakeKey(string contract, BigInteger tokenId)
        => $"{contract.ToLowerInvariant()}:{tokenId}";
}