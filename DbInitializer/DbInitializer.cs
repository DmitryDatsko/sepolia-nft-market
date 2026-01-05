using Microsoft.EntityFrameworkCore;
using SepoliaNftMarket.Context;

namespace SepoliaNftMarket.DbInitializer;

public class DbInitializer(IDbContextFactory<ApiDbContext> context) : IDbInitializer
{
    public void Initialize()
    {
        using var db = context.CreateDbContext();
        db.Database.Migrate();
    }
}