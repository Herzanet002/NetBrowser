namespace NetBrowser_UWP.EventArguments;

public class SettingChangedEventArgs
{
    public string PropertyName { get; }

    public object NewValue { get; }

    public SettingChangedEventArgs(string propertyName, object newValue)
    {
        PropertyName = propertyName;
        NewValue = newValue;
    }
}