using NetBrowser_UWP.Models;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts.Services;

public interface IDataTransferService
{
    Task SaveHistoryAsync(HistoryItemDetails historyItemDetail);
    Task SaveSearchTermAsync(SearchTermItem siteItem);
    Task<IList<HistoryItemDetails>> GetHistoryAsync();
    Task<IList<SearchTermItem>> GetSearchTermAsync();
    Task SaveBookmarkAsync(BookmarkDetails bookmarkDetails);
    Task<IList<BookmarkDetails>> GetBookmarksListAsync();
    Task RemoveBookmarkAsync(BookmarkDetails bookmarkDetails);
    Task SaveNewsContentToFavoriteAsync(ContentModel contentModel);
    Task<IList<ContentModel>> GetAllFavoritesNewsContentAsync();
    Task<ContentModel> HasNewsContentInFavorite(ContentModel contentModel);
    Task RemoveNewsContentFromFavorite(ContentModel contentModel);
    Task<IList<RssFeeder>> GetRssFeedersListAsync();
    Task AddRecommendationSyndicationCategoryAsync(ICollection<SyndicationCategoryModel> category);
    Task RemoveRecommendationSyndicationCategoryAsync(ICollection<SyndicationCategoryModel> category);
    Task ClearHistoryFileAsync();
    Task RemoveHistoryItemAsync(HistoryItemDetails historyItemDetails);
    Task EditBookmarkAsync(BookmarkDetails oldBookmark, BookmarkDetails newBookmark);
    Task<IList<SearchEngineItem>> GetSearchEngineListAsync();
    Task<SearchEngineItem> GetCurrentSearchEngineAsync();
    Task<IList<StartPageItem>> GetStartPageElementsAsync();
    Task EditStartPageItemAsync(StartPageItem oldItem, StartPageItem newItem);
    Task AddNewSiteOnStartPageAsync(StartPageItem siteItem);
    Task RemoveSiteOnStartPageAsync(StartPageItem siteItem);
    Task ChangeSearchEngineAsync(string newEngine);
}