using NetBrowser_UWP.Enums;
using System.Text.RegularExpressions;
using System;

namespace NetBrowser_UWP.CommandProcessor.Strategies;

public class HttpsSchemeResolutionStrategy : ICommandStrategy
{
    public bool TryResolveCommand(Command command, out CommandResult commandResolveResult)
    {
        const string addressPattern = @"^(www\.)";
        var query = command.Query.Trim();

        if (Regex.IsMatch(query, addressPattern)
            && Uri.TryCreate("https://" + query, UriKind.Absolute, out var result))
        {
            commandResolveResult = new CommandResult(result.ToString(), CommandResultType.WithHttpsScheme);
            return true;
        }

        commandResolveResult = null;
        return false;
    }
}