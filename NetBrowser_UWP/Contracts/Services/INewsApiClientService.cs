using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser.Core.Models;

namespace NetBrowser_UWP.Contracts.Services;

public interface INewsApiClientService
{
    Task<IEnumerable<ContentModel>> GetNewsAsync(int pageSize, int pageIndex);

    Task<IEnumerable<ContentModel>> GetNewsByProvidersAsync(IEnumerable<NewsProvider> providers, int pageSize, int pageIndex);
    
    Task<IEnumerable<NewsProvider>> GetNewsProvidersAsync();
}