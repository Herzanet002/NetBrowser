using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts.Services;

public interface IFirstRunAppInitializerService
{
    Task InitializeAppStorageAsync();
}