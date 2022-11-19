using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.Views.UserControls;

namespace NetBrowser_UWP.ViewModels.Settings;

public class BookmarksPageViewModel : ObservableObject
{
    private static ObservableCollection<BookmarkDetails> _bookmarksList;
    private static string _bookmarkNewTitle;
    private static string _bookmarkNewUrl;
    private static BookmarkDetails _selectedBookmark;

    private readonly IDataTransferService _dataTransferService;
    private readonly TabViewService _tabViewService;

    public BookmarksPageViewModel(IDataTransferService dataTransferService, TabViewService tabViewService)
    {
        _dataTransferService = dataTransferService;
        _tabViewService = tabViewService;
        GetBookmarksAsync();
    }

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

    public BookmarkDetails SelectedBookmark
    {
        get => _selectedBookmark;
        set => SetProperty(ref _selectedBookmark, value);
    }

    public ObservableCollection<BookmarkDetails> BookmarksList
    {
        get => _bookmarksList;
        set => SetProperty(ref _bookmarksList, value);
    }

    public async Task GetBookmarksAsync()
    {
        BookmarksList = new ObservableCollection<BookmarkDetails>(await _dataTransferService.GetBookmarksListAsync());
    }

    private async Task OnOpenBookmarkInWebCommandExecuted()
    {
        await _tabViewService.CreateNewWebTab(SelectedBookmark.Url);
        SelectedBookmark = null;
    }

    private async Task OnRemoveBookmarkCommandExecuted()
    {
        await _dataTransferService.RemoveBookmarkAsync(SelectedBookmark);
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
        return _dataTransferService.EditBookmarkAsync(SelectedBookmark, new BookmarkDetails
        {
            Name = BookmarkNewTitle,
            Url = BookmarkNewUrl,
            FaviconUrl = Constants.Constants.FAVICONS_SERVICE + BookmarkNewUrl
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