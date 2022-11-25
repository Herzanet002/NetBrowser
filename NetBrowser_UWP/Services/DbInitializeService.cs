using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetBrowser_UWP.DbContexts;

namespace NetBrowser_UWP.Services;

public class DbInitializeService
{
    private readonly DataAccessContext _dbContext;

    public DbInitializeService(DataAccessContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitializeAsync()
    {
        //await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);
        await _dbContext.Database.MigrateAsync().ConfigureAwait(false);
    }
}