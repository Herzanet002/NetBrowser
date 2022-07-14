using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBrowser_UWP.Contracts.Services
{
    public interface IDataAccessService
    {
        public void InitializeHistoryFile();
        public void InitializeBookmarksFile();
        public void InitializeConfigFile();
        public void InitializeStartPageFile();
    }
}
