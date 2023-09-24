using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser.Core.Models;

namespace NetBrowser.Storage.StorageContracts;

public interface IHistoryStorage
{
    Task SaveHistoryAsync(HistoryItem historyItem);

    Task<List<HistoryItem>> GetHistoryAsync();

    Task ClearAllHistoryAsync();

    Task RemoveHistoryItemAsync(HistoryItem historyItem);

    Task AddOrReplaceSearchTermAsync(SearchTermItem searchTermItem);

    Task<List<SearchTermItem>> GetSearchTermsAsync();
}