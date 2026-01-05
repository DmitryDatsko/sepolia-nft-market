using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace SepoliaNftMarket.Models.ContractOutput;

[FunctionOutput]
public class GetTradeOutput : IFunctionOutputDTO
{
    [Parameter("tuple", "from", 1)] public PeerOutput From { get; set; } = new();
    [Parameter("tuple", "to", 2)] public PeerOutput To { get; set; } = new();
    [Parameter("bool", "isActive", 3)] public bool IsActive { get; set; }
}