using System.Collections.Generic;
using NetBrowser_UWP.CommandProcessor.Strategies;
using NetBrowser_UWP.Enums;

namespace NetBrowser_UWP.CommandProcessor;

public class CommandProcessor : ICommandProcessor
{
    private readonly List<ICommandStrategy> _commandStrategies = new()
    {
        new AbsoluteUriResolutionStrategy(),
        new HttpsSchemeResolutionStrategy(),
        new UserQueryResolutionStrategy()
    };

    public CommandResult ResolveCommand(Command command)
    {
        //normalizing query

        foreach (var commandStrategy in _commandStrategies)
        {
            if (commandStrategy.TryResolveCommand(command, out var commandResolveResult))
            {
                return commandResolveResult;
            }
        }

        return new CommandResult(CommandResultType.Malformed);
    }
}