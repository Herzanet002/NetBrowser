namespace NetBrowser_UWP.Enums;

public enum CommandResultType
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
    ///     Внутренняя команда браузера
    /// </summary>
    ServiceCommand,

    /// <summary>
    ///     Неверный адрес, не поддающийся преобразованию
    /// </summary>
    Malformed
}