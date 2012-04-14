using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            Targets = new ObservableCollection<ShareTarget>();
            AddTarget = new DelegateCommand(AddTargetExecute);
            ShowAddTarget = new DelegateCommand(ShowAddTargetExecute);
            CancelAddTarget = new DelegateCommand(CancelAddTargetExecute);
        }

        public ICommand AddTarget { get; set; }
        public ICommand ShowAddTarget { get; set; }
        public ICommand CancelAddTarget { get; set; }
        public Boolean ShowAddNewShareTarget { get { return showAddNewShareTarget; } set { showAddNewShareTarget = value; NotifyPropertyChanged("ShowAddNewShareTarget"); } }
        public String TargetIPAddress { get; set; }
        public String TargetName { get; set; }
        public ObservableCollection<ShareTarget> Targets { get; set; }

        public void AddTargetExecute(object state)
        {
            var target = new ShareTarget() { IPAddress = TargetIPAddress, Name = TargetName };

            Models.ShareTargets model = new Models.ShareTargets();
            model.AddShareTarget(target);

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
