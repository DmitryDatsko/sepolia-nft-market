using System.Numerics;
using SepoliaNftMarket.Models.DTO.Etherscan;

namespace SepoliaNftMarket.Providers.Etherscan;

public interface IEtherscanProvider
{
    public Task<EventLog> GetEventLogsAsync(BigInteger fromBlock);
    public Task<DateTime> GetBlockMintTimeAsync(BigInteger blockNumber);
}