using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.DbContexts;
using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetBrowser_UWP.Services
{
    public class DataTransferDbService : IDataTransferService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DataTransferDbService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task SaveHistoryAsync(HistoryItemDetails historyItemDetail)
        {
            if (historyItemDetail is null) return;
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            await dbContext.HistoryItems.AddAsync(historyItemDetail).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SaveSearchTermAsync(SiteItem searchTermItem)
        {
            if (searchTermItem is null) return;
            var dbContext = GetDbContext();
            await dbContext.SearchTermItems.AddAsync(searchTermItem).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private DataAccessContext GetDbContext()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            return dbContext;
        }

        public async Task<IList<HistoryItemDetails>> GetHistoryAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            return await dbContext.HistoryItems.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public async Task<IList<SiteItem>> GetSearchTermAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            return await dbContext.SearchTermItems.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public async Task SaveBookmarkAsync(BookmarkDetails bookmarkDetails)
        {
            if (bookmarkDetails is null) return;
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            await dbContext.Bookmarks.AddAsync(bookmarkDetails).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IList<BookmarkDetails>> GetBookmarksListAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            return await dbContext.Bookmarks.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public async Task<bool> ExistsItem<T>(T item, CancellationToken ct = default) where T : EntityModel
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            return await dbContext.Set<T>().AnyAsync(i => i.Id == item.Id, ct).ConfigureAwait(false);
        }

        public async Task RemoveBookmarkAsync(BookmarkDetails bookmarkDetails)
        {
            if (bookmarkDetails is null) return;
            if (!await ExistsItem(bookmarkDetails))
                return;
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            dbContext.Bookmarks.Remove(bookmarkDetails);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task SaveNewsContentToFavoriteAsync(ContentModel contentModel, CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }

        public async Task ClearHistoryFileAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            dbContext.HistoryItems.Clear();
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveHistoryItemAsync(HistoryItemDetails historyItemDetails)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            dbContext.HistoryItems.Remove(historyItemDetails);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task EditBookmarkAsync(BookmarkDetails oldBookmark, BookmarkDetails newBookmark)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            var existBookmark = dbContext.Bookmarks.FirstOrDefault(x => x.Name == oldBookmark.Name
                                                                        && x.Url == oldBookmark.Url);
            if (existBookmark is null) return;
            existBookmark.Name = newBookmark.Name;
            existBookmark.FaviconUrl = newBookmark.FaviconUrl;
            existBookmark.Url = newBookmark.Url;
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IList<SearchEngineItem>> GetSearchEngineListAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            return await dbContext.SearchEngines.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }

        public async Task<SearchEngineItem> GetCurrentSearchEngineAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            return dbContext.SearchEngines.AsNoTracking().FirstOrDefault(x => x.IsSelected == "1");
        }

        public async Task<IList<StartPageItem>> GetStartPageElementsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            return (await dbContext.StartPageItems.AsNoTracking().ToListAsync().ConfigureAwait(false));
        }

        public async Task EditStartPageItemAsync(StartPageItem oldItem, StartPageItem newItem)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            var existsItem = dbContext.StartPageItems.FirstOrDefault(x => x.Name == oldItem.Name
                                                                        && x.Url == oldItem.Url);
            if (existsItem is null) return;
            existsItem.Name = newItem.Name;
            existsItem.Url = newItem.Url;
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddNewSiteOnStartPageAsync(StartPageItem siteItem)
        {
            if (siteItem == null) return;
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            await dbContext.StartPageItems.AddAsync(siteItem).ConfigureAwait(false);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveSiteOnStartPageAsync(StartPageItem siteItem)
        {
            if (siteItem == null) return;
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            dbContext.StartPageItems.Remove(siteItem);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ChangeSearchEngineAsync(string newEngine)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            var oldSearchEngine = dbContext.SearchEngines.FirstOrDefault(x => x.IsSelected == "1");
            if (oldSearchEngine != null)
                oldSearchEngine.IsSelected = "0";
            var newSearchEngine = dbContext.SearchEngines.FirstOrDefault(x => x.Name == newEngine);
            if (newSearchEngine != null)
                newSearchEngine.IsSelected = "1";
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

    }
}
