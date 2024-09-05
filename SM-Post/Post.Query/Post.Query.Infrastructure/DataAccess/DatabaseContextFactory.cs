using Microsoft.EntityFrameworkCore;

namespace Post.Query.Infrastructure.DataAccess;

public class DatabaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
{
    public DatabaseContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        configureDbContext(optionsBuilder);
        return new DatabaseContext(optionsBuilder.Options);
    }
}