using System.Numerics;
using SepoliaNftMarket.Models;

namespace SepoliaNftMarket.Providers.Moralis;

public interface IMoralisProvider
{
    public Task<NftMetadata> GetNftMetadataAsync(string address, string tokenId);
    public Task<IReadOnlyDictionary<string, NftMetadata>> GetNftsMetadataAsync(List<string> addresses, List<BigInteger> tokenIds);
}