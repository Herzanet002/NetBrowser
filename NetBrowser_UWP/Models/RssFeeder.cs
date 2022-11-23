using System;

namespace NetBrowser_UWP.Models
{
    public class RssFeeder : BaseEntity
    {
        public string Name { get; set; }
        public string ApiUrl { get; set; }
        public Uri Link { get; set; }
        public string FeederImageLink { get; set; }
    }
}
