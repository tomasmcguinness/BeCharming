﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BeCharming.Metro.Models;

namespace BeCharming.Metro.ViewModels
{
    public class SharingDialogViewModel : INotifyPropertyChanged
    {
        private Windows.UI.Core.CoreDispatcher Dispatcher;
        private ShareTargetsManager manager;
        private bool isShowingSharingDialog = false;
        private bool isShowingRequiresPassword = false;
        private bool isSharing = false;
        private string pinCode = null;

        public SharingDialogViewModel(Windows.UI.Core.CoreDispatcher Dispatcher)
        {
            this.Dispatcher = Dispatcher;
            this.manager = new ShareTargetsManager();
            this.manager.ShareComplete += manager_ShareComplete;

            this.CancelCommand = new DelegateCommand(CancelCommandExecute);
            this.ShareCommand = new DelegateCommand(ShareCommandExecute, CanShareCommandExecute);
        }

        void manager_ShareComplete(object sender, EventArgs e)
        {
            this.IsShowingSharingDialog = false;
        }

        public Boolean IsShowingRequiresPin { get { return isShowingRequiresPassword; } set { isShowingRequiresPassword = value; NotifyPropertyChanged("IsShowingPasswordRequired"); } }
        public Boolean IsSharing { get { return isSharing; } set { isSharing = value; NotifyPropertyChanged("IsSharing"); } }
        public Boolean IsShowingSharingDialog { get { return isShowingSharingDialog; } set { isShowingSharingDialog = value; NotifyPropertyChanged("IsShowingSharingDialog"); } }
        public string PinCode { get { return pinCode; } set { pinCode = value; UpdateSendCommand(); NotifyPropertyChanged("PinCode"); } }
        public ICommand CancelCommand { get; set; }
        public ICommand ShareCommand { get; set; }

        public void CancelCommandExecute(object state)
        {
            PinCode = null;
            IsShowingSharingDialog = false;
        }

        public void ShareCommandExecute(object state)
        {
            if (!string.IsNullOrEmpty(PinCode))
            {
                IsSharing = true;
            }
        }

        public bool CanShareCommandExecute(object state)
        {
            return !string.IsNullOrEmpty(PinCode) && PinCode.Length == 4;
        }

        public void UpdateSendCommand()
        {

        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetupForShareRequest(Models.ShareRequest request)
        {
            if (manager.RequiresPin(request))
            {
                IsShowingRequiresPin = true;
                IsSharing = false;
            }
            else
            {
                IsShowingRequiresPin = false;
                IsSharing = true;
                manager.Share(request);
            }
        }
    }
}
