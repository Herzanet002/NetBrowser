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
