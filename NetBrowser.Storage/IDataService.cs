using NetBrowser.Storage.StorageContracts;

namespace NetBrowser.Storage;

public interface IDataService : IHistoryStorage, IBookmarksStorage,
    INewsContentStorage, ISearchEnginesStorage, IStartPageStorage
{
}