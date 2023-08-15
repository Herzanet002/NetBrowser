using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using NetBrowser_UWP.Constants;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.ViewModels.Base;
using Prism.Commands;

namespace NetBrowser_UWP.ViewModels.Controls;

public class FindBoxViewModel : BindableBase, IFindBox
{
    private readonly TabViewService _tabViewService;
    private readonly IWebView2Service _webView2Service;
    private readonly IDataService _dataService;

    private string _queryText;
    private string _bookmarkTitleForSave;
    private string _bookmarkUrlForSave;
    private bool _isBookmarksExists;
    private bool _isFlyoutClosed;
    private bool _visibilityDeleteBookmarkButton;
    private IList<SearchTermItem> _suggestionsCollection;
    private ObservableCollection<BookmarkItem> _bookmarksList;

    public IAsyncRelayCommand QueryTextChangedCommand { get; private set; }

    public IAsyncRelayCommand QuerySubmittedCommand { get; private set; }

    public IAsyncRelayCommand SaveBookmarkButtonCommand { get; private set; }

    public IAsyncRelayCommand DeleteBookmarkButtonCommand { get; private set; }

    public ICommand AddBookmarkButtonCommand { get; private set; }

    public ICommand CancelSaveBookmarkButtonCommand { get; private set; }

    public FindBoxViewModel(TabViewService tabViewService,
        IWebView2Service webView2Service,
        IDataService dataService)
    {
        _tabViewService = tabViewService;
        _webView2Service = webView2Service;
        _dataService = dataService;
        QueryTextChangedCommand =
            new AsyncRelayCommand<AutoSuggestBoxTextChangedEventArgs>(OnSearchBoxTextChangedCommandExecuted);
        QuerySubmittedCommand =
            new AsyncRelayCommand<AutoSuggestBoxQuerySubmittedEventArgs>(OnSearchBoxQuerySubmittedCommandExecuted);
        AddBookmarkButtonCommand = new DelegateCommand(OnAddBookmarkButtonCommandExecuted);
        SaveBookmarkButtonCommand = new AsyncRelayCommand(OnSaveBookmarkCommandExecuted);
        CancelSaveBookmarkButtonCommand = new DelegateCommand(OnCancelSaveBookmarkCommandExecuted);
        DeleteBookmarkButtonCommand = new AsyncRelayCommand(OnDeleteBookmarkCommandExecuted);
        Messenger.Register<FindBoxViewModel, FindBoxQueryChangedMessage>(this,
            (vm, msg) => vm.QueryText = msg.Value);
        Messenger.Register<FindBoxViewModel, FindBoxNavigateToMessage>(this,
            (vm, msg) => vm.NavigateTo(msg.Value));
        Messenger.Register<FindBoxViewModel, FindBoxSetBookmarkButtonAppearanceMessage>(this,
            (vm, _) => vm.SetBookmarkButtonAppearance());
    }

    public string QueryText
    {
        get => _queryText;
        set => SetProperty(ref _queryText, value);
    }

    public IList<SearchTermItem> SuggestionsCollection
    {
        get => _suggestionsCollection;
        set => SetProperty(ref _suggestionsCollection, value);
    }

    public ObservableCollection<BookmarkItem> BookmarksList
    {
        get => _bookmarksList;
        set => SetProperty(ref _bookmarksList, value);
    }

    public bool IsBookmarksExists
    {
        get => _isBookmarksExists;
        set => SetProperty(ref _isBookmarksExists, value);
    }

    public bool DeleteBookmarkButtonVisibility
    {
        get => _visibilityDeleteBookmarkButton;
        set => SetProperty(ref _visibilityDeleteBookmarkButton, value);
    }

    public string BookmarkTitleForSave
    {
        get => _bookmarkTitleForSave;
        set => SetProperty(ref _bookmarkTitleForSave, value);
    }

    public string BookmarkUrlForSave
    {
        get => _bookmarkUrlForSave;
        set => SetProperty(ref _bookmarkUrlForSave, value);
    }

    public bool IsFlyoutClosed
    {
        get => _isFlyoutClosed;
        set
        {
            SetProperty(ref _isFlyoutClosed, value);
            if (value)
                IsFlyoutClosed = false;
        }
    }

    public async Task AutoSuggestListFill()
    {
        var searchTerm = await GetSearchTermListAsync();

        var suitableItems = new List<SearchTermItem>(searchTerm.Where(x =>
            x.Query != null && x.Query.Contains(QueryText, StringComparison.OrdinalIgnoreCase)));

        if (!suitableItems.Any())
            suitableItems.Add(new SearchTermItem
            {
                Query = QueryText,
                IsNewSuggestedSearchQuery = true
            });

        if (QueryText.Length != 0)
        {
            SuggestionsCollection = suitableItems;
            return;
        }

        var recentlySearch = new List<SearchTermItem>();
        var searchTermList = searchTerm.ToList();
        if (searchTermList.Count < 10)
            recentlySearch = searchTermList;
        else
            recentlySearch.AddRange(searchTermList.GetRange(0, 8));

        suitableItems = recentlySearch;
        SuggestionsCollection = suitableItems;
    }

    public void NavigateTo(string address)
    {
        if (_tabViewService.GetSelectedWebView() == null)
            return;

        switch (address)
        {
            case ApplicationConstants.SETTINGS_ADDRESS:
                _tabViewService.CreateSettingsTab();
                break;

            case ApplicationConstants.STARTPAGE_ADDRESS:
                _tabViewService.CreateStartPageTab();
                break;

            case ApplicationConstants.NEWS_ADDRESS:
                _tabViewService.CreateNewsTab();
                break;

            default:
                _tabViewService.SelectedWebView2.Source = _webView2Service.ResolveUri(address);
                break;
        }
    }

    private async Task<IEnumerable<SearchTermItem>> GetSearchTermListAsync()
    {
        var searchTermListTransfer = await _dataService.GetSearchTermsAsync();
        searchTermListTransfer.Reverse();
        return searchTermListTransfer.ToHashSet();
    }

    private async Task OnSearchBoxTextChangedCommandExecuted(AutoSuggestBoxTextChangedEventArgs obj)
    {
        if (obj is null)
            return;
        if (obj.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            await AutoSuggestListFill();
    }

    private async Task OnSearchBoxQuerySubmittedCommandExecuted(AutoSuggestBoxQuerySubmittedEventArgs obj)
    {
        if (obj is null)
            return;
        var queryForSearch = new SearchTermItem();

        if (obj.ChosenSuggestion is SearchTermItem suggestion)
        {
            queryForSearch = suggestion;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(obj.QueryText))
            {
                queryForSearch = new SearchTermItem
                {
                    Query = obj.QueryText
                };
            }
        }

        if (_tabViewService.GetSelectedWebView() == null)
            return;

        NavigateTo(queryForSearch.Query);
        await _dataService.AddOrReplaceSearchTermAsync(new SearchTermItem
        {
            Query = queryForSearch.Query,
            LastTimeAccess = DateTime.Now
        }).ConfigureAwait(false);
    }

    private async Task OnDeleteBookmarkCommandExecuted()
    {
        await _dataService.RemoveBookmarkAsync(new BookmarkItem
        {
            Name = BookmarkTitleForSave,
            Url = BookmarkUrlForSave
        });
        await GetBookmarksAsync();
        SetBookmarkButtonAppearance();
        IsFlyoutClosed = true;
    }

    private void OnAddBookmarkButtonCommandExecuted()
    {
        if (_tabViewService.GetSelectedWebView() == null)
            return;
        BookmarkTitleForSave = _tabViewService.GetSelectedWebView().CoreWebView2.DocumentTitle;
        BookmarkUrlForSave = _tabViewService.GetSelectedWebView().Source.AbsoluteUri;
    }

    private async Task OnSaveBookmarkCommandExecuted()
    {
        if (!(string.IsNullOrWhiteSpace(BookmarkTitleForSave) ||
              string.IsNullOrWhiteSpace(BookmarkUrlForSave)) &&
            Uri.IsWellFormedUriString(BookmarkUrlForSave, UriKind.Absolute))
        {
            await _dataService.SaveBookmarkAsync(
                new BookmarkItem
                {
                    Name = BookmarkTitleForSave,
                    Url = BookmarkUrlForSave,
                    FaviconUrl = Constants.ApplicationConstants.FAVICONS_SERVICE + BookmarkUrlForSave
                });
            await GetBookmarksAsync();
            IsFlyoutClosed = true;
            SetBookmarkButtonAppearance();
        }
        else
        {
            var dialogError = new ContentDialog
            {
                Title = "Неверные данные",
                Content = "Проверьте правильность адреса",
                CloseButtonText = "Закрыть"
            };

            await dialogError.ShowAsync();
        }
    }

    private void OnCancelSaveBookmarkCommandExecuted()
    {
        IsFlyoutClosed = true;
    }

    private void SetBookmarkIconState(bool isAccessable)
    {
        IsBookmarksExists = isAccessable;
        DeleteBookmarkButtonVisibility = isAccessable;
    }

    private void SetBookmarkButtonAppearance()
    {
        if (_tabViewService.GetSelectedWebView() == null ||
            _tabViewService.GetSelectedWebView().Source == null)
        {
            SetBookmarkIconState(false);
            return;
        }

        if (BookmarksList == null)
            return;

        var existableBookmark = BookmarksList.FirstOrDefault(bookmark =>
            bookmark.Url == _tabViewService.GetSelectedWebView().Source.AbsoluteUri);

        SetBookmarkIconState(existableBookmark != null);
    }

    private async Task GetBookmarksAsync()
    {
        var bookmarksListTransfer = await _dataService.GetBookmarksAsync();
        bookmarksListTransfer.Reverse();
        BookmarksList = new ObservableCollection<BookmarkItem>(bookmarksListTransfer);
    }
}

public sealed class FindBoxQueryChangedMessage : ValueChangedMessage<string>
{
    public FindBoxQueryChangedMessage(string value) : base(value)
    {
    }
}

public sealed class FindBoxNavigateToMessage : ValueChangedMessage<string>
{
    public FindBoxNavigateToMessage(string value) : base(value)
    {
    }
}

public sealed class FindBoxSetBookmarkButtonAppearanceMessage
{
}