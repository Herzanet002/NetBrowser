using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts.Services;

public interface IDataInitializeService
{
    public Task InitializeHistoryStorageAsync();
    public Task InitializeBookmarksStorageAsync();
    public Task InitializeConfigStorageAsync();
    public Task InitializeStartPageStorageAsync();
    public Task InitializeNewsContentStorageAsync();
}