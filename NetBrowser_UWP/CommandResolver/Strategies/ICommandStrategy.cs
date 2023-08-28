namespace NetBrowser_UWP.CommandResolver.Strategies;

public interface ICommandStrategy
{
    bool TryResolveCommand(Command command, out CommandResult commandResolveResult);
}