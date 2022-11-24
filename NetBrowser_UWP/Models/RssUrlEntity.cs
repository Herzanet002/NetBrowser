using System.ComponentModel.DataAnnotations.Schema;

namespace NetBrowser_UWP.Models
{
    public class RssUrlEntity : BaseEntity
    {
        public int RssFeederId { get; set; }
        public RssFeeder RssFeeder { get; set; }
        public string Url { get; set; }
    }
}
