using NetBrowser_UWP.Enums;

namespace NetBrowser_UWP.CommandResolver;

public class CommandResult
{
    public CommandResultType ResultType { get; }

    public string? ResolvedCommandResult { get; }

    public CommandResult(string resolvedCommandResult, CommandResultType resultType)
    {
        ResultType = resultType;
        ResolvedCommandResult = resolvedCommandResult;
    }

    public CommandResult(CommandResultType resultType)
    {
        ResultType = resultType;
    }
}