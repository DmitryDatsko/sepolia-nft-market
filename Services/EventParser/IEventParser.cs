using Nethereum.ABI.FunctionEncoding.Attributes;
using SepoliaNftMarket.Models.DTO.Etherscan;

namespace SepoliaNftMarket.Services.EventParser;

public interface IEventParser
{
    IEventDTO? ParseEvent(EtherscanLog log);
}