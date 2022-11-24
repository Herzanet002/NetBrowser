using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetBrowser_UWP.Models
{
    public class RssFeeder : BaseEntity
    {
        public string Name { get; set; }
        public Uri Link { get; set; }
        public string RssUrl { get; set; }
        public string FeederImageLink { get; set; }
    }
}
