using Microsoft.EntityFrameworkCore;
using NetBrowser_UWP.Models;
using System.Collections.Generic;

namespace NetBrowser_UWP.DbContexts;

public class DataAccessContext : DbContext
{
    public DataAccessContext(DbContextOptions<DataAccessContext> options) : base(options)
    {
    }

    public DbSet<HistoryItemDetails> HistoryItems { get; set; }
    public DbSet<BookmarkDetails> Bookmarks { get; set; }
    public DbSet<SearchEngineItem> SearchEngines { get; set; }
    public DbSet<StartPageItem> StartPageItems { get; set; }
    public DbSet<SiteItem> SearchTermItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StartPageItem>().HasData(InitializeStartPageItems());
        modelBuilder.Entity<SearchEngineItem>().HasData(InitializeSearchEngines());
        base.OnModelCreating(modelBuilder);
    }

    public List<SearchEngineItem> InitializeSearchEngines()
    {
        return new List<SearchEngineItem>
            {
                new SearchEngineItem
                {
                    Id = 1,
                    HomePage = "https://www.google.ru/",
                    Name = "Google",
                    IsSelected = "1",
                    Prefix = "https://google.com/search?q="
                },
                new SearchEngineItem
                {
                    Id = 2,
                    HomePage = "https://yandex.ru/",
                    Name = "Yandex",
                    IsSelected = "0",
                    Prefix = "https://yandex.ru/search/?text="
                },
                new SearchEngineItem
                {
                    Id = 3,
                    HomePage = "https://www.bing.ru/",
                    Name = "Bing",
                    IsSelected = "0",
                    Prefix = "https://bing.com/search?q="
                }
            };
    }

    public List<StartPageItem> InitializeStartPageItems()
    {
        return new List<StartPageItem>
            {
                new StartPageItem
                {
                    Id = 1,
                    Name = "Google",
                    Url = "www.google.com"
                },
                new StartPageItem
                {
                    Id = 2,
                    Name = "GitHub",
                    Url = "www.github.com"
                },
                new StartPageItem
                {
                    Id = 3,
                    Name = "Yandex",
                    Url = "www.yandex.com"
                },
                new StartPageItem
                {
                    Id = 4,
                    Name = "YouTube",
                    Url = "www.youtube.com"
                }
            };
    }
}