namespace NetBrowser.Core.Models;

public class HistoryItem : SiteItem
{
    public HistoryItem(string time, string date)
    {
        Time = time;
        Date = date;
    }

    public HistoryItem()
    {
    }

    public string Time { get; set; }

    public string Date { get; set; }
}