using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts.Services;

public interface IDataAccessService
{
    public Task InitializeHistoryFileAsync();
    public Task InitializeBookmarksFileAsync();
    public Task InitializeConfigFileAsync();
    public Task InitializeStartPageFileAsync();
    public Task InitializeNewsContentFileAsync();
}