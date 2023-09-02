using System;

namespace NetBrowser_UWP.Contracts;

public interface ITrackingContract
{
    void NotifyPropertyChanged(Type propertyType);
}