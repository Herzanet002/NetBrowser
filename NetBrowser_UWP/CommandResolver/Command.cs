namespace NetBrowser_UWP.CommandResolver;

public class Command
{
    public Command(string query)
    {
        Query = query;
    }

    public string Query { get; set; }
}