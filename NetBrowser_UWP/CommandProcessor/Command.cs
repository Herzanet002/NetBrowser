namespace NetBrowser_UWP.CommandProcessor;

public class Command
{
    public Command(string query)
    {
        Query = query;
    }

    public string Query { get; set; }
}