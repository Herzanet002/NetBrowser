namespace NetBrowser_UWP.CommandResolver;

public interface ICommandResolver
{
    CommandResult ResolveCommand(Command command);
}