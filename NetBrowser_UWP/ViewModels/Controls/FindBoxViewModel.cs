using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NetBrowser_UWP.Constants;
using NetBrowser_UWP.Contracts.Services;
using NetBrowser_UWP.Helpers;
using NetBrowser_UWP.Messages;
using NetBrowser_UWP.Models;
using NetBrowser_UWP.Services;
using NetBrowser_UWP.ViewModels.Base;
using Prism.Commands;

namespace NetBrowser_UWP.ViewModels.Controls;

public class FindBoxViewModel : BindableBase, IFindBox
{
    private readonly TabViewService _tabViewService;
    private readonly IDataService _dataService;

    private string _queryText;
    private bool _isBookmarkExists;
    private bool _isFlyoutClosed;
    private bool _isSuggestionPaneOpen;

    private BookmarkItem _bookmarkForSave;
    private ObservableCollection<SearchTermItem> _suggestionsCollection;
    private ObservableCollection<BookmarkItem> _bookmarksList;

    public IAsyncRelayCommand QueryTextChangedCommand { get; private set; }

    public IAsyncRelayCommand QuerySubmittedCommand { get; private set; }

    public IAsyncRelayCommand SaveBookmarkButtonCommand { get; private set; }

    public IAsyncRelayCommand DeleteBookmarkButtonCommand { get; private set; }

    public ICommand AddBookmarkButtonCommand { get; private set; }

    public ICommand CancelSaveBookmarkButtonCommand { get; private set; }

    public FindBoxViewModel(TabViewService tabViewService, IDataService dataService)
    {
        _tabViewService = tabViewService;
        _dataService = dataService;
        QueryTextChangedCommand =
            new AsyncRelayCommand<AutoSuggestBoxTextChangedEventArgs>(OnFindBoxTextChangedCommandExecuted);
        QuerySubmittedCommand =
            new AsyncRelayCommand<AutoSuggestBoxQuerySubmittedEventArgs>(OnFindBoxQuerySubmittedCommandExecuted);
        AddBookmarkButtonCommand = new DelegateCommand(OnAddBookmarkButtonCommandExecuted);
        SaveBookmarkButtonCommand = new AsyncRelayCommand(OnSaveBookmarkCommandExecuted,
            () => !string.IsNullOrWhiteSpace(BookmarkForSave?.Url));
        CancelSaveBookmarkButtonCommand = new DelegateCommand(() => IsFlyoutClosed = true);
        DeleteBookmarkButtonCommand = new AsyncRelayCommand(OnDeleteBookmarkCommandExecuted);
        Messenger.Register<FindBoxViewModel, FindBoxQueryChangedMessage>(this,
            (vm, msg) => vm.QueryText = msg.Value);
        Messenger.Register<FindBoxViewModel, FindBoxNavigateToMessage>(this,
            (vm, msg) => vm.NavigateTo(msg.Value));
        Messenger.Register<FindBoxViewModel, FindBoxSetBookmarkButtonAppearanceMessage>(this,
            async (vm, _) => await vm.SetBookmarkButtonAppearance());
    }

    #region Public properties

    public string QueryText
    {
        get => _queryText;
        set => SetProperty(ref _queryText, value);
    }

    public ObservableCollection<SearchTermItem> SuggestionsCollection
    {
        get => _suggestionsCollection;
        set => SetProperty(ref _suggestionsCollection, value);
    }

    public ObservableCollection<BookmarkItem> BookmarksList
    {
        get => _bookmarksList;
        set => SetProperty(ref _bookmarksList, value);
    }

    public bool IsBookmarkExists
    {
        get => _isBookmarkExists;
        set => SetProperty(ref _isBookmarkExists, value);
    }

    public bool IsSuggestionPaneOpen
    {
        get => _isSuggestionPaneOpen;
        set => SetProperty(ref _isSuggestionPaneOpen, value);
    }

    public BookmarkItem BookmarkForSave
    {
        get => _bookmarkForSave;
        set
        {
            SetProperty(ref _bookmarkForSave, value);
            SaveBookmarkButtonCommand.NotifyCanExecuteChanged();
        }
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

    #endregion

    public async Task FillSuggestionsCollection()
    {
        var searchTerm = await GetSearchTermListAsync();

        var suitableItems = new List<SearchTermItem>(searchTerm.Where(x =>
            x.Query != null && x.Query.Contains(QueryText, StringComparison.OrdinalIgnoreCase)))
        {
            new()
            {
                Query = QueryText,
                IsNewSuggestedSearchQuery = true
            }
        };

        if (QueryText.Length != 0)
        {
            SuggestionsCollection = new ObservableCollection<SearchTermItem>(suitableItems);
            return;
        }

        var recentlySearch = new List<SearchTermItem>();
        var searchTermList = searchTerm.ToList();
        if (searchTermList.Count < 10)
            recentlySearch = searchTermList;
        else
            recentlySearch.AddRange(searchTermList.GetRange(0, 8));

        suitableItems = recentlySearch;
        SuggestionsCollection = new ObservableCollection<SearchTermItem>(suitableItems);
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
                _tabViewService.SelectedWebView2.Source = UriResolver.ResolveUri(address).UriResult;
                break;
        }
    }

    private async Task<IEnumerable<SearchTermItem>> GetSearchTermListAsync()
    {
        var searchTermListTransfer = await _dataService.GetSearchTermsAsync();
        searchTermListTransfer.Reverse();
        return searchTermListTransfer.ToHashSet();
    }

    private async Task OnFindBoxTextChangedCommandExecuted(AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args is null)
            return;
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            await FillSuggestionsCollection();
    }

    private async Task OnFindBoxQuerySubmittedCommandExecuted(AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args is null)
            return;
        var queryForSearch = new SearchTermItem();

        if (args.ChosenSuggestion is SearchTermItem suggestion)
        {
            queryForSearch = suggestion;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(args.QueryText))
            {
                queryForSearch = new SearchTermItem
                {
                    Query = args.QueryText
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
        await _dataService.RemoveBookmarkAsync(BookmarkForSave).ConfigureAwait(false);
        await GetBookmarksAsync();
        await SetBookmarkButtonAppearance();
        IsFlyoutClosed = true;
    }

    private void OnAddBookmarkButtonCommandExecuted()
    {
        var selectedWebView = _tabViewService.GetSelectedWebView();
        if (selectedWebView == null)
            return;

        var existableBookmark = BookmarksList?.FirstOrDefault(bookmark =>
            bookmark.Url == selectedWebView.Source.AbsoluteUri);

        if (existableBookmark != null)
        {
            BookmarkForSave = existableBookmark;
            return;
        }

        BookmarkForSave = new BookmarkItem
        {
            Name = selectedWebView.CoreWebView2.DocumentTitle,
            Url = selectedWebView.Source.AbsoluteUri
        };
    }

    private async Task OnSaveBookmarkCommandExecuted()
    {
        BookmarkForSave.FaviconUrl = ApplicationConstants.FAVICONS_SERVICE + BookmarkForSave.Url;
        await _dataService.SaveBookmarkAsync(BookmarkForSave);

        IsFlyoutClosed = true;
        await SetBookmarkButtonAppearance();
    }

    private async Task SetBookmarkButtonAppearance()
    {
        if (_tabViewService.GetSelectedWebView() == null ||
            _tabViewService.GetSelectedWebView().Source == null)
        {
            IsBookmarkExists = false;
            return;
        }

        await GetBookmarksAsync();
        if (BookmarksList == null)
        {
            return;
        }

        var existableBookmark = BookmarksList?.FirstOrDefault(bookmark =>
            bookmark.Url == _tabViewService.GetSelectedWebView().Source.AbsoluteUri);

        IsBookmarkExists = existableBookmark != null;
    }

    private async Task GetBookmarksAsync()
    {
        var bookmarksListTransfer = await _dataService.GetBookmarksAsync();
        bookmarksListTransfer.Reverse();
        BookmarksList = new ObservableCollection<BookmarkItem>(bookmarksListTransfer);
    }
}