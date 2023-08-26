namespace NetBrowser_UWP.CommandProcessor.Strategies;

public interface ICommandStrategy
{
    bool TryResolveCommand(Command command, out CommandResult commandResolveResult);
}