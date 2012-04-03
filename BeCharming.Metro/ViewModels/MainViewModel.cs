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
            CancelAddTarget = new DelegateCommand(CancelAddTargetExecute);
        }

        public ICommand AddTarget { get; set; }
        public ICommand CancelAddTarget { get; set; }
        public Boolean ShowAddNewShareTarget { get { return showAddNewShareTarget; } set { showAddNewShareTarget = value; NotifyPropertyChanged("ShowAddNewShareTarget"); } }

        public void AddTargetExecute(object state)
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
