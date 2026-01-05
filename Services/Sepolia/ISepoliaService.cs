using System.Numerics;
using SepoliaNftMarket.Models.ContractOutput;

namespace SepoliaNftMarket.Services.Sepolia;

public interface ISepoliaService
{
    Task<GetTradeOutput> GetTradeDataAsync(BigInteger tradeId, CancellationToken cancellationToken = default);
    Task<string> GetTransactionInitiatorAsync(string txHash);
    Task<string> GetTransactionReciverAsync(string txHash);
}