using Microsoft.EntityFrameworkCore;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Helpers;

public static class EntityExtensions
{
    public static void Clear<T>(this DbSet<T> dbSet) where T : BaseEntity
    {
        dbSet.RemoveRange(dbSet);
    }
}