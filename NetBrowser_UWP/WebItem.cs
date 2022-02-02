using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBrowser_UWP
{
    public class WebItem
    {
        public string Title { get; set; }
        public string Url { get; set; }

        public WebItem(string title, string url)
        {
            Title = title;
            Url = url;
        }
    }
}
