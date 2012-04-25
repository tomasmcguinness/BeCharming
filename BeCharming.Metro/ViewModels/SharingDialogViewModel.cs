using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BeCharming.Metro.ViewModels
{
    public class SharingDialogViewModel : INotifyPropertyChanged
    {
        private Windows.UI.Core.CoreDispatcher Dispatcher;
        private bool isShowingSharingDialog = false;
        private bool isShowingRequiresPassword = false;

        public SharingDialogViewModel(Windows.UI.Core.CoreDispatcher Dispatcher)
        {
            this.Dispatcher = Dispatcher;
            this.CancelCommand = new DelegateCommand(CancelCommandExecute);
        }

        public Boolean IsShowingRequiresPin { get { return isShowingRequiresPassword; } set { isShowingRequiresPassword = value; NotifyPropertyChanged("IsShowingPasswordRequired"); } }
        public Boolean IsShowingSharingDialog { get { return isShowingSharingDialog; } set { isShowingSharingDialog = value; NotifyPropertyChanged("IsShowingSharingDialog"); } }
        public string PinCode { get; set; }
        public ICommand CancelCommand { get; set; }

        public void CancelCommandExecute(object state)
        {
            PinCode = null;
            IsShowingSharingDialog = false;
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
