using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser.Core.Models;

namespace NetBrowser.Storage.StorageContracts;

public interface ISearchEnginesStorage
{
    Task SetDefaultSearchEngineAsync(SearchEngineItem searchEngine);

    Task<List<SearchEngineItem>> GetSearchEngineListAsync();

    Task<SearchEngineItem> GetCurrentSearchEngineAsync();

    Task AddSearchEngineAsync(SearchEngineItem engineItem);
}