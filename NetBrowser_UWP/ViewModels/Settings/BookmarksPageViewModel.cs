using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Constants;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.ViewModels.Base;
using NetBrowser_UWP.Views.UserControls;

namespace NetBrowser_UWP.ViewModels.Settings;

public class BookmarksPageViewModel : BindableBase
{
    private static ObservableCollection<BookmarkItem> _bookmarksList;
    private static string _bookmarkNewTitle;
    private static string _bookmarkNewUrl;
    private static BookmarkItem _selectedBookmark;

    private readonly IDataService _dataService;
    private readonly TabViewService _tabViewService;

    public BookmarksPageViewModel(IDataService dataService, TabViewService tabViewService)
    {
        _dataService = dataService;
        _tabViewService = tabViewService;
        BookmarksPageSettingsLoadedCommand = new AsyncRelayCommand(OnBookmarksPageSettingsLoadedCommandExecuted);
    }

    public IAsyncRelayCommand BookmarksPageSettingsLoadedCommand { get; set; }

    public string BookmarkNewTitle
    {
        get => _bookmarkNewTitle;
        set => SetProperty(ref _bookmarkNewTitle, value);
    }

    public string BookmarkNewUrl
    {
        get => _bookmarkNewUrl;
        set => SetProperty(ref _bookmarkNewUrl, value);
    }

    public BookmarkItem SelectedBookmark
    {
        get => _selectedBookmark;
        set => SetProperty(ref _selectedBookmark, value);
    }

    public ObservableCollection<BookmarkItem> BookmarksList
    {
        get => _bookmarksList;
        set => SetProperty(ref _bookmarksList, value);
    }

    private async Task OnBookmarksPageSettingsLoadedCommandExecuted(CancellationToken ct)
    {
        await GetBookmarksAsync();
    }

    public async Task GetBookmarksAsync()
    {
        BookmarksList = new ObservableCollection<BookmarkItem>(await _dataService.GetBookmarksAsync());
    }

    private async Task OnOpenBookmarkInWebCommandExecuted()
    {
        await _tabViewService.CreateNewWebTab(SelectedBookmark.Url);
        SelectedBookmark = null;
    }

    private async Task OnRemoveBookmarkCommandExecuted()
    {
        await _dataService.RemoveBookmarkAsync(SelectedBookmark);
    }

    private async Task OnDeleteSelectedBookmarkCommandExecuted()
    {
        await new DeleteBookmarkDialog().ShowAsync();
        await GetBookmarksAsync().ConfigureAwait(false);
    }

    private async Task OnEditBookmarkCommandExecuted()
    {
        BookmarkNewTitle = SelectedBookmark.Name;
        BookmarkNewUrl = SelectedBookmark.Url;

        await new EditBookmarkDialog().ShowAsync();
        await GetBookmarksAsync().ConfigureAwait(false);
    }

    private Task OnSaveEditedBookmarkCommandExecuted()
    {
        return _dataService.EditBookmarkAsync(SelectedBookmark, new BookmarkItem
        {
            Name = BookmarkNewTitle,
            Url = BookmarkNewUrl,
            FaviconUrl = ApplicationConstants.FAVICONS_SERVICE + BookmarkNewUrl
        });
    }

    #region Commands

    public IAsyncRelayCommand EditBookmarkCommand => new AsyncRelayCommand(OnEditBookmarkCommandExecuted, () => true);

    public IAsyncRelayCommand SaveEditedBookmarkCommand =>
        new AsyncRelayCommand(OnSaveEditedBookmarkCommandExecuted, () => true);

    public IAsyncRelayCommand OpenBookmarkCommand =>
        new AsyncRelayCommand(OnOpenBookmarkInWebCommandExecuted, () => true);

    public IAsyncRelayCommand DeleteBookmarkCommand =>
        new AsyncRelayCommand(OnDeleteSelectedBookmarkCommandExecuted, () => true);

    public IAsyncRelayCommand RemoveBookmarkCommand =>
        new AsyncRelayCommand(OnRemoveBookmarkCommandExecuted, () => true);

    #endregion Commands
}