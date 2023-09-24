using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using NetBrowser.Core.Enums;
using NetBrowser_UWP.CommandResolver.Strategies;

namespace NetBrowser_UWP.CommandResolver;

public class CommandResolver : ObservableRecipient, ICommandResolver
{
    public IEnumerable<ICommandStrategy> CommandStrategies { get; }

    public CommandResolver(IEnumerable<ICommandStrategy> commandStrategies)
        => CommandStrategies = commandStrategies;

    public CommandResult ResolveCommand(Command command)
    {
        command.Query = command.Query.ToLower().Trim();

        foreach (var commandStrategy in CommandStrategies)
        {
            if (commandStrategy.TryResolveCommand(command, out var commandResolveResult))
            {
                return commandResolveResult;
            }
        }

        return new CommandResult(CommandResultType.Malformed);
    }
}