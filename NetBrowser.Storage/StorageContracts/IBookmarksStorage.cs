using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser.Core.Models;

namespace NetBrowser.Storage.StorageContracts;

public interface IBookmarksStorage
{
    Task SaveBookmarkAsync(BookmarkItem bookmarkItem);

    Task EditBookmarkAsync(BookmarkItem oldBookmark, BookmarkItem newBookmark);

    Task<List<BookmarkItem>> GetBookmarksAsync();

    Task RemoveBookmarkAsync(BookmarkItem bookmarkItem);
}