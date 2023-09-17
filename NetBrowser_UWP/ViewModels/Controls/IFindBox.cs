using System.Threading.Tasks;
using NetBrowser_UWP.CommandResolver;

namespace NetBrowser_UWP.ViewModels.Controls;

public interface IFindBox
{
    Task FillSuggestionsCollection();

    void NavigateTo(CommandResult commandResult);

    string QueryText { get; set; }
}