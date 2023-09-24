using NetBrowser.Core.Enums;
using NetBrowser_UWP.Contracts.Services;

namespace NetBrowser_UWP.CommandResolver.Strategies;

public class PredefinedSchemeResolutionStrategy : ICommandStrategy
{
    private readonly IPageService _pageService;

    public PredefinedSchemeResolutionStrategy(IPageService pageService)
    {
        _pageService = pageService;
    }

    public bool TryResolveCommand(Command command, out CommandResult commandResolveResult)
    {
        var pageInfo = _pageService.GetPageInfo(command.Query);
        if (pageInfo != null)
        {
            commandResolveResult = new CommandResult(pageInfo.Path, CommandResultType.PredefinedCommand);
            return true;
        }

        commandResolveResult = null;
        return false;
    }
}