namespace NetBrowser_UWP.CommandProcessor;

public interface ICommandProcessor
{
    CommandResult ResolveCommand(Command command);
}