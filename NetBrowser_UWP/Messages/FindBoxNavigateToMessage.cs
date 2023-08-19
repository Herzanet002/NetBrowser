using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NetBrowser_UWP.Messages;

public sealed class FindBoxNavigateToMessage : ValueChangedMessage<string>
{
    public FindBoxNavigateToMessage(string value) : base(value)
    {
    }
}