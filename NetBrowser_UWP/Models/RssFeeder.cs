using System;

namespace NetBrowser_UWP.Models
{
    public class RssFeeder : BaseEntity
    {
        public string Name { get; set; }
        public Uri Link { get; set; }
        public string RssUrl { get; set; }
        public string FeederImageLink { get; set; }
        public bool IsCategorized { get; set; }
        public bool RecommendedCategory { get; set; }
    }
}
