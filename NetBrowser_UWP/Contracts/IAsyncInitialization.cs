using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts
{
    public interface IAsyncInitialization
    {
        Task Initialization { get; }
    }
}
