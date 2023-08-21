using System;

namespace NetBrowser_UWP.Services.Settings;

public class SettingHolder<T>
{
    private readonly Func<T> _onGetAction;
    private readonly Action<T> _onSetAction;

    public SettingHolder(Func<T> onGetAction, Action<T> onSetAction)
    {
        _onGetAction = onGetAction;
        _onSetAction = onSetAction;
    }

    public void SetSetting(T value) => _onSetAction(value);

    public T GetSetting() => _onGetAction();
}