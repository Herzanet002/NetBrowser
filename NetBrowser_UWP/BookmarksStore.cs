using System.Collections.ObjectModel;
using NetBrowser_UWP.Services;

namespace NetBrowser_UWP
{
    public class BookmarksStore
    {
        private readonly DataTransferService _dataTransferService;
        private ObservableCollection<BookmarksStore> _bookmarks;

        public BookmarksStore(DataTransferService dataTransferService)
        {
            _dataTransferService = dataTransferService;
            _bookmarks = new ObservableCollection<BookmarksStore>();
        }

    }
}
