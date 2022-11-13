using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts.Services;

public interface IDataAccessService
{
    public Task InitializeHistoryFile();
    public Task InitializeBookmarksFile();
    public Task InitializeConfigFile();
    public Task InitializeStartPageFile();
}