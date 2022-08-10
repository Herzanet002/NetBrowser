using System;

namespace NetBrowser_UWP.Models
{
    [Serializable]
    public class HistoryItemDetails : SiteItem
    {
        public string Time { get; set; }

        public string Date { get; set; }
    }
}
