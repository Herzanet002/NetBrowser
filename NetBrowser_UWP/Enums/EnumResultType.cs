namespace NetBrowser_UWP.Enums;

public enum UriResultType
{
    /// <summary>
    ///     Адрес с добавленным префиксом из App.CurrentWebEngine.Prefix
    /// </summary>
    Prefixed,

    /// <summary>
    ///     Валидный абсолютный URI
    /// </summary>
    ValidAbsoluteUri,

    /// <summary>
    ///      Адрес с добавленной HTTPS-схемой
    /// </summary>
    WithHttpsScheme,

    /// <summary>
    ///     Неверный адрес, не поддающийся преобразованию
    /// </summary>
    Malformed
}