using SepoliaNftMarket.Models.DTO;

namespace SepoliaNftMarket.Models.EndpointsCursors;

public record UserTokenCursor(DateTime AcquiredAt, OrderDirection OrderBy);