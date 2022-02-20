using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetBrowser_UWP.Models
{
    public class SearchEngineItem:INotifyPropertyChanged
    {
        public string prefix;
        public string name;
        public string mode;
        public string homePage;

        public string Prefix
        {
            get { return prefix; }
            set
            {
                prefix = value;
                OnPropertyChanged("Prefix");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                OnPropertyChanged("Mode");
            }
        }
        public string HomePage
        {
            get { return homePage; }
            set
            {
                homePage = value;
                OnPropertyChanged("HomePage");
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
