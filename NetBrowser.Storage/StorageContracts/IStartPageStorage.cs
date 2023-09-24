using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser.Core.Models;

namespace NetBrowser.Storage.StorageContracts;

public interface IStartPageStorage
{
    Task<List<SiteItem>> GetStartPageElementsAsync();

    Task EditStartPageItemAsync(SiteItem oldItem, SiteItem newItem);

    Task AddNewSiteOnStartPageAsync(SiteItem siteItem);

    Task RemoveSiteOnStartPageAsync(SiteItem siteItem);
}