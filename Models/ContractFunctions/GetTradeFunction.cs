using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace SepoliaNftMarket.Models.ContractFunctions;

[Function("trades")]
public class GetTradeFunction : FunctionMessage
{
    [Parameter("uint256", "", 1)]
    public BigInteger TradeId { get; set; }
}
