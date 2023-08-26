using NetBrowser_UWP.Enums;
using System;

namespace NetBrowser_UWP.CommandProcessor.Strategies;

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