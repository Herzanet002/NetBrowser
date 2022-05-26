using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetBrowser_UWP.Models
{
    public class SearchEngineItem:INotifyPropertyChanged
    {
        private string prefix;
        private string name;
        private string mode;
        private string homePage;

        public string Prefix
        {
            get => prefix;
            set
            {
                prefix = value;
                OnPropertyChanged(nameof(Prefix));
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Mode
        {
            get => mode;
            set
            {
                mode = value;
                OnPropertyChanged(nameof(Mode));
            }
        }
        public string HomePage
        {
            get => homePage;
            set
            {
                homePage = value;
                OnPropertyChanged(nameof(HomePage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
