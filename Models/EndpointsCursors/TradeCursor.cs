using SepoliaNftMarket.Models.DTO;

namespace SepoliaNftMarket.Models.EndpointsCursors;

public record TradeCursor(Guid LastId, TradeDirection Direction);