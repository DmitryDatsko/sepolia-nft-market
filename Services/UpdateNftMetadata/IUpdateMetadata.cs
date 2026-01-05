using System.Numerics;

namespace SepoliaNftMarket.Services.UpdateNftMetadata;

public interface IUpdateMetadata
{
    Task UpdateMetadataAsync(List<string> contractAddresses, List<BigInteger> tokenIds);
}