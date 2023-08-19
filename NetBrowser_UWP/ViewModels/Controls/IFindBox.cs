using System.Threading.Tasks;

namespace NetBrowser_UWP.ViewModels.Controls;

public interface IFindBox
{
    Task FillSuggestionsCollection();

    void NavigateTo(string address);

    string QueryText { get; set; }
}