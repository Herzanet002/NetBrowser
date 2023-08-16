using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NetBrowser_UWP.Messages;

public sealed class FindBoxQueryChangedMessage : ValueChangedMessage<string>
{
    public FindBoxQueryChangedMessage(string value) : base(value)
    {
    }
}