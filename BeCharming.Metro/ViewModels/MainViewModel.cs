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
        private bool showEditShareTarget = false;
        private ShareTarget selectedShareTarget = null;
        private Models.ShareTargetManager model;
        private bool isAppBarShowing = false;
        private bool isSearchingForPeers = false;
        private Windows.UI.Core.CoreDispatcher Dispatcher;

        public MainViewModel(Windows.UI.Core.CoreDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
            Targets = new ObservableCollection<ShareTarget>();
            model = new Models.ShareTargetManager();
            model.InitShareTargetStorage();
            model.PeerDiscoveryComplete += PeerDiscoveryComplete;
            model.PeerDiscovered += PeerDiscovered;

            AddTarget = new DelegateCommand(AddTargetExecute);
            EditTarget = new DelegateCommand(EditTargetExecute, CanEditTargetExecute);
            DeleteTarget = new DelegateCommand(DeleteTargetExecute, CanDeleteTargetExecute);
            ShowAddTarget = new DelegateCommand(ShowAddTargetExecute);
            CancelAddTarget = new DelegateCommand(CancelAddTargetExecute);
            RefreshTargets = new DelegateCommand(PerformPeerDiscovery);
        }

        void PeerDiscovered(ShareTarget discoveredTarget)
        {
            Dispatcher.Invoke(Windows.UI.Core.CoreDispatcherPriority.Normal, (i, u) =>
            {
                bool alreadyDiscovered = false;

                foreach (ShareTarget target in Targets)
                {
                    if (target.Equals(discoveredTarget))
                    {
                        alreadyDiscovered = true;
                        break;
                    }
                }

                if (!alreadyDiscovered)
                {
                    Targets.Add(discoveredTarget);
                }
            }, this, null);
        }

        private void DeleteTargetExecute(object obj)
        {
            model.DeleteShareTarget(this.SelectedTarget);
        }

        public bool CanDeleteTargetExecute(object obj)
        {
            return SelectedTarget != null;
        }

        private void EditTargetExecute(object obj)
        {
            ShowEditShareTarget = true;
        }

        private bool CanEditTargetExecute(object obj)
        {
            return SelectedTarget != null;
        }

        public ICommand AddTarget { get; set; }
        public ICommand EditTarget { get; set; }
        public ICommand DeleteTarget { get; set; }
        public ICommand ShowAddTarget { get; set; }
        public ICommand CancelAddTarget { get; set; }
        public ICommand RefreshTargets { get; set; }

        public Boolean ShowAddNewShareTarget { get { return showAddNewShareTarget; } set { showAddNewShareTarget = value; NotifyPropertyChanged("ShowAddNewShareTarget"); } }
        public Boolean ShowEditShareTarget { get { return showEditShareTarget; } set { showEditShareTarget = value; NotifyPropertyChanged("ShowEditShareTarget"); } }

        public String TargetIPAddress { get; set; }
        public String TargetName { get; set; }

        public Boolean IsAppBarShowing { get { return isAppBarShowing; } set { isAppBarShowing = value; NotifyPropertyChanged("IsAppBarShowing"); } }
        public Boolean IsSearchingForPeers { get { return isSearchingForPeers; } set { isSearchingForPeers = value; NotifyPropertyChanged("IsSearchingForPeers"); } }
        public ObservableCollection<ShareTarget> Targets { get; set; }
        public ShareTarget SelectedTarget { get { return selectedShareTarget; } set { selectedShareTarget = value; TargetSelected(); } }

        private void TargetSelected()
        {
            IsAppBarShowing = SelectedTarget != null;
        }

        public void AddTargetExecute(object state)
        {
            var target = new ShareTarget() { IPAddress = TargetIPAddress, Name = TargetName };
            model.AddShareTarget(target);
            ShowAddNewShareTarget = false;
        }

        public void ShowAddTargetExecute(object state)
        {
            IsAppBarShowing = false;
            ShowAddNewShareTarget = true;
        }

        public void CancelAddTargetExecute(object state)
        {
            ShowAddNewShareTarget = false;
        }

        public void IncrementShareCount(ShareTarget target)
        {
            model.IncrementShareCount(target);
        }

        public void PerformPeerDiscovery()
        {
            PerformPeerDiscovery(null);
        }

        public void PerformPeerDiscovery(object state)
        {
            IsSearchingForPeers = true;
            model.PerformPeerDiscovery();
        }

        public void PeerDiscoveryComplete(object sender, EventArgs e)
        {
            IsSearchingForPeers = false;
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