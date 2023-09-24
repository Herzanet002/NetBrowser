using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser.Core.Models;

namespace NetBrowser.Storage.StorageContracts;

public interface INewsContentStorage
{
    Task ClearLikedNewsProvidersAsync();

    Task<List<NewsProvider>> GetLikedNewsProvidersAsync();

    Task AddLikedNewsProvidersAsync(IEnumerable<NewsProvider> feeders);

    Task SaveNewsContentToFavoriteAsync(ContentModel content);

    Task<List<ContentModel>> GetAllFavoritesNewsContentAsync();

    Task RemoveNewsContentFromFavoritesAsync(ContentModel content);
}