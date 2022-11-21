namespace NetBrowser_UWP.Models;

public class SearchEngineItem : EntityModel
{
    public string Prefix { get; set; }
    public string Name { get; set; }
    public string IsSelected { get; set; }
    public string HomePage { get; set; }
}