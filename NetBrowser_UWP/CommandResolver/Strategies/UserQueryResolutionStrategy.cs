using System;
using NetBrowser_UWP.Enums;

namespace NetBrowser_UWP.CommandResolver.Strategies;

public class UserQueryResolutionStrategy : ICommandStrategy
{
    public bool TryResolveCommand(Command command, out CommandResult commandResolveResult)
    {
        if (Uri.IsWellFormedUriString(App.CurrentWebEngine.Prefix + command.Query, UriKind.Absolute))
        {
            commandResolveResult =
                new CommandResult(App.CurrentWebEngine.Prefix + command.Query, CommandResultType.Prefixed);
            return true;
        }

        commandResolveResult = null;
        return false;
    }
}