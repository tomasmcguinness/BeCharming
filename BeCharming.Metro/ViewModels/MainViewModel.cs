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
        }

        public ICommand AddTarget { get; set; }
        public Boolean ShowAddNewShareTarget { get { return showAddNewShareTarget; } set { showAddNewShareTarget = value; NotifyPropertyChanged("ShowAddNewShareTarget"); } }

        public void AddTargetExecute(object state)
        {
            ShowAddNewShareTarget = true;
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
