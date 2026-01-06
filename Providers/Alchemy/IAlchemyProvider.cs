using System.Numerics;
using SepoliaNftMarket.Models;
using SepoliaNftMarket.Models.DTO;

namespace SepoliaNftMarket.Providers.Alchemy;

public interface IAlchemyProvider
{
    public Task<NftMetadata> GetNftMetadataAsync(string address, string tokenId);
    public Task<IReadOnlyDictionary<string, NftMetadata>> GetNftsMetadataAsync(List<string> addresses, List<BigInteger> tokenIds);
    public Task<List<UserToken>> GetUserTokensAsync(string address, bool sortByDesc);
}