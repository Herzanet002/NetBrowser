using CommunityToolkit.Mvvm.Messaging.Messages;
using NetBrowser_UWP.CommandResolver;

namespace NetBrowser_UWP.Messages;

public sealed class FindBoxNavigateToMessage : ValueChangedMessage<CommandResult>
{
    public FindBoxNavigateToMessage(CommandResult value) : base(value)
    {
    }
}