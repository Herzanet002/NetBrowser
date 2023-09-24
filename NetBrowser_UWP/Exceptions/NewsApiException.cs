using System;

namespace NetBrowser_UWP.Exceptions;

public class NewsApiException : Exception
{
    public string Response { get; }

    public NewsApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public override string ToString()
        => $"HTTP Response: \n\n{Response}\n\n{base.ToString()}";
}