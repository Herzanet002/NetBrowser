using System.Collections.Generic;
using System.Threading.Tasks;
using NetBrowser_UWP.Models;

namespace NetBrowser_UWP.ViewModels.Controls;

public interface IFindBox
{
    void NavigateTo(string address);

    string QueryText { get; set; }

    IList<SearchTermItem> SuggestionsCollection { get; set; }

    Task AutoSuggestListFill();
}