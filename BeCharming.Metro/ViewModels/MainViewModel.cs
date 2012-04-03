using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BeCharming.Metro.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool showAddNewShareTarget = false;

        public MainViewModel()
        {
            AddTarget = new DelegateCommand(AddTargetExecute);
            ShowAddTarget = new DelegateCommand(ShowAddTargetExecute);
            CancelAddTarget = new DelegateCommand(CancelAddTargetExecute);
        }

        public ICommand AddTarget { get; set; }
        public ICommand ShowAddTarget { get; set; }
        public ICommand CancelAddTarget { get; set; }
        public Boolean ShowAddNewShareTarget { get { return showAddNewShareTarget; } set { showAddNewShareTarget = value; NotifyPropertyChanged("ShowAddNewShareTarget"); } }
        public String IPAddress { get; set; }

        public void AddTargetExecute(object state)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var container = localSettings.CreateContainer("BeCharmingSettings", Windows.Storage.ApplicationDataCreateDisposition.Always);

            List<string> shareTargets = container.Values["ShareTargets"] as List<string>;

            if (shareTargets == null)
            {
                shareTargets = new List<string>();
            }

            shareTargets.Add(IPAddress);

            var xml = ObjectSerializer<List<String>>.ToXml(shareTargets);

            container.Values["ShareTargets"] = xml;

            ShowAddNewShareTarget = false;
        }

        public void ShowAddTargetExecute(object state)
        {
            ShowAddNewShareTarget = true;
        }

        public void CancelAddTargetExecute(object state)
        {
            ShowAddNewShareTarget = false;
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
