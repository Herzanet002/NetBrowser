using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NetBrowser_UWP.Messages;

public class InnerPageTypeChangedMessage : ValueChangedMessage<Type>
{
    public InnerPageTypeChangedMessage(Type value) : base(value)
    {
    }
}