using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using LiteDB;
using NetBrowser_UWP.Constants;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.Services;

public class DataService : IDataService
{
    private const string DB_FILE_NAME = "global_dat.db";

    private static readonly string FullFilePath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_FILE_NAME);

    #region History

    public Task<List<HistoryItem>> GetHistoryAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<HistoryItem>(ApplicationConstants.HISTORY_COLLECTION_NAME);
        var historyItems = collection.FindAll().ToList();
        return Task.FromResult(historyItems);
    }

    public Task SaveHistoryAsync(HistoryItem historyItem)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<HistoryItem>(ApplicationConstants.HISTORY_COLLECTION_NAME);
        collection.Insert(historyItem);

        collection.EnsureIndex(x => x.Name);
        collection.EnsureIndex(x => x.Url);
        return Task.CompletedTask;
    }

    public Task ClearAllHistoryAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<HistoryItem>(ApplicationConstants.HISTORY_COLLECTION_NAME);
        collection.DeleteAll();
        return Task.CompletedTask;
    }

    public Task RemoveHistoryItemAsync(HistoryItem historyItem)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<HistoryItem>(ApplicationConstants.HISTORY_COLLECTION_NAME);
        collection.Delete(historyItem.Id);
        return Task.CompletedTask;
    }

    #endregion

    #region Search term

    public Task SaveSearchTermAsync(SiteItem siteItem)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<SiteItem>(ApplicationConstants.SEARCHTERMS_COLLECTION_NAME);
        collection.Insert(siteItem);

        collection.EnsureIndex(x => x.Name);
        collection.EnsureIndex(x => x.Url);
        return Task.CompletedTask;
    }

    public Task<List<SiteItem>> GetSearchTermsAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<SiteItem>(ApplicationConstants.SEARCHTERMS_COLLECTION_NAME);
        var siteItems = collection.FindAll().ToList();
        return Task.FromResult(siteItems);
    }

    #endregion

    #region Bookmarks

    public Task SaveBookmarkAsync(BookmarkItem bookmarkItem)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<BookmarkItem>(ApplicationConstants.BOOKMARKS_COLLECTION_NAME);
        collection.Insert(bookmarkItem);

        collection.EnsureIndex(x => x.Name);
        collection.EnsureIndex(x => x.Url);
        return Task.CompletedTask;
    }

    public Task EditBookmarkAsync(BookmarkItem oldBookmark, BookmarkItem newBookmark)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<BookmarkItem>(ApplicationConstants.BOOKMARKS_COLLECTION_NAME);
        collection.Update(oldBookmark.Id, newBookmark);
        return Task.CompletedTask;
    }

    public Task<List<BookmarkItem>> GetBookmarksAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<BookmarkItem>(ApplicationConstants.BOOKMARKS_COLLECTION_NAME);
        var bookmarkItems = collection.FindAll().ToList();
        return Task.FromResult(bookmarkItems);
    }

    public Task RemoveBookmarkAsync(BookmarkItem bookmarkItem)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<BookmarkItem>(ApplicationConstants.BOOKMARKS_COLLECTION_NAME);
        collection.Delete(bookmarkItem.Id);
        return Task.CompletedTask;
    }

    #endregion

    #region News content

    public Task SaveNewsContentToFavoriteAsync(ContentModel contentItem)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<ContentModel>(ApplicationConstants.FAVORITE_NEWS_COLLECTION_NAME);
        collection.Insert(contentItem);

        collection.EnsureIndex(x => x.Title);
        collection.EnsureIndex(x => x.Link);
        return Task.CompletedTask;
    }

    public Task<List<ContentModel>> GetAllFavoriteNewsContentAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<ContentModel>(ApplicationConstants.FAVORITE_NEWS_COLLECTION_NAME);
        var collectionItems = collection.FindAll().ToList();
        return Task.FromResult(collectionItems);
    }

    public Task RemoveNewsContentFromFavoriteAsync(ContentModel contentModel)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<ContentModel>(ApplicationConstants.FAVORITE_NEWS_COLLECTION_NAME);
        collection.Delete(contentModel.Id);
        return Task.CompletedTask;
    }

    public Task<List<RssFeeder>> GetRssFeedersListAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<RssFeeder>(ApplicationConstants.RSS_FEEDERS_COLLECTION_NAME);
        return Task.FromResult(collection.FindAll().ToList());
    }

    public Task AddRssFeedersAsync(IEnumerable<RssFeeder> feeders)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<RssFeeder>(ApplicationConstants.RSS_FEEDERS_COLLECTION_NAME);
        collection.Insert(feeders);
        return Task.CompletedTask;
    }

    public Task AddLikedRssFeedersAsync(IEnumerable<RssFeeder> feeders)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<RssFeeder>(ApplicationConstants.LIKED_RSS_FEEDERS_COLLECTION_NAME);
        collection.Insert(feeders);
        return Task.CompletedTask;
    }

    public Task<List<RssFeeder>> GetLikedRssFeedersAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<RssFeeder>(ApplicationConstants.LIKED_RSS_FEEDERS_COLLECTION_NAME);
        return Task.FromResult(collection.FindAll().ToList());
    }

    public Task ClearAllLikedRssFeedersAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<RssFeeder>(ApplicationConstants.LIKED_RSS_FEEDERS_COLLECTION_NAME);
        collection.DeleteAll();
        return Task.CompletedTask;
    }

    #endregion

    #region Search engine

    public Task SetDefaultSearchEngineAsync(SearchEngineItem searchEngine)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<SearchEngineItem>(ApplicationConstants.SEARCH_ENGINES_COLLECTION_NAME);
        var currentSearchEngine = collection.Find(x => x.IsSelected).First();
        currentSearchEngine.IsSelected = false;
        collection.Update(currentSearchEngine);

        searchEngine.IsSelected = true;
        collection.Update(searchEngine);
        return Task.CompletedTask;
    }

    public Task<List<SearchEngineItem>> GetSearchEngineListAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<SearchEngineItem>(ApplicationConstants.SEARCH_ENGINES_COLLECTION_NAME);
        var searchEngineList = collection.FindAll().ToList();
        return Task.FromResult(searchEngineList);
    }

    public Task<SearchEngineItem> GetCurrentSearchEngineAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<SearchEngineItem>(ApplicationConstants.SEARCH_ENGINES_COLLECTION_NAME);
        var currentSearchEngine = collection.Find(x => x.IsSelected).FirstOrDefault();
        return Task.FromResult(currentSearchEngine);
    }

    public Task AddSearchEngineAsync(SearchEngineItem engineItem)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<SearchEngineItem>(ApplicationConstants.SEARCH_ENGINES_COLLECTION_NAME);
        collection.Insert(engineItem);
        return Task.CompletedTask;
    }

    #endregion

    #region Start page

    public Task<List<SiteItem>> GetStartPageElementsAsync()
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<SiteItem>(ApplicationConstants.STARTPAGE_ITEMS_COLLECTION_NAME);
        var collectionItems = collection.FindAll().ToList();
        return Task.FromResult(collectionItems);
    }

    public Task EditStartPageItemAsync(SiteItem oldItem, SiteItem newItem)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<SiteItem>(ApplicationConstants.STARTPAGE_ITEMS_COLLECTION_NAME);
        collection.Update(oldItem.Id, newItem);
        return Task.CompletedTask;
    }

    public Task AddNewSiteOnStartPageAsync(SiteItem siteItem)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<SiteItem>(ApplicationConstants.STARTPAGE_ITEMS_COLLECTION_NAME);
        collection.Insert(siteItem);

        collection.EnsureIndex(x => x.Name);
        collection.EnsureIndex(x => x.Url);
        return Task.CompletedTask;
    }

    public Task RemoveSiteOnStartPageAsync(SiteItem siteItem)
    {
        using var liteConnection = new LiteDatabase(FullFilePath);
        var collection = liteConnection.GetCollection<SiteItem>(ApplicationConstants.STARTPAGE_ITEMS_COLLECTION_NAME);
        collection.Delete(siteItem.Id);
        return Task.CompletedTask;
    }

    #endregion
}