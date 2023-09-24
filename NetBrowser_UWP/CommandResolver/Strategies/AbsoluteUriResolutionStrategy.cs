using System;
using NetBrowser.Core.Enums;

namespace NetBrowser_UWP.CommandResolver.Strategies;

public class AbsoluteUriResolutionStrategy : ICommandStrategy
{
    public bool TryResolveCommand(Command command, out CommandResult commandResolveResult)
    {
        if (Uri.TryCreate(command.Query.Trim(), UriKind.Absolute, out var result)
            && (result.Scheme == Uri.UriSchemeHttp
                || result.Scheme == Uri.UriSchemeHttps
                || result.Scheme == Uri.UriSchemeFtp
                || result.Scheme == Uri.UriSchemeFile))
        {
            commandResolveResult = new CommandResult(result.ToString(), CommandResultType.ValidAbsoluteUri);
            return true;
        }

        commandResolveResult = null;
        return false;
    }
}