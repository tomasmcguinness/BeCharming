using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BeCharming.Common.ListenerService;
using BeCharming.Metro.Models;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;

namespace BeCharming.Metro.ViewModels
{
    public class ShareTargetsViewModel : INotifyPropertyChanged
    {
        private ShareOperation shareOperation = null;
        private byte[] fileBytes;
        private string fileName;
        private string url;
        private ShareTarget selectedTarget;
        private bool shareButtonEnabled;
        private bool isSharing;
        private bool isSearchingForPeers;
        private ShareTargetsManager manager;
        private Windows.UI.Core.CoreDispatcher Dispatcher;

        public ShareTargetsViewModel(Windows.UI.Core.CoreDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
            Targets = new ObservableCollection<ShareTarget>();
            Share = new DelegateCommand(TargetSelected);
            manager = new ShareTargetsManager();
            manager.ShareComplete += manager_ShareComplete;
            manager.ShareFailed += manager_ShareFailed;
            manager.PeerDiscoveryComplete += model_PeerDiscoveryComplete;
            manager.PeerDiscovered += model_PeerDiscovered;
        }

        void manager_ShareFailed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void manager_ShareComplete(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        async void model_PeerDiscovered(ShareTarget shareTarget)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Targets.Add(shareTarget);
            });
        }

        void model_PeerDiscoveryComplete(object sender, EventArgs e)
        {
            IsSearchingForPeers = false;
        }

        public ObservableCollection<ShareTarget> Targets { get; private set; }
        public ShareTarget SelectedTarget { get { return selectedTarget; } set { selectedTarget = value; UpdateSelectedTarget(); } }
        public ICommand Share { get; set; }
        public bool IsSharing { get { return isSharing; } set { isSharing = value; NotifyPropertyChanged("IsSharing"); } }
        public bool ShareButtonEnabled { get { return shareButtonEnabled; } set { shareButtonEnabled = value; NotifyPropertyChanged("ShareButtonEnabled"); } }
        public bool IsSearchingForPeers { get { return isSearchingForPeers; } set { isSearchingForPeers = value; NotifyPropertyChanged("IsSearchingForPeers"); } }

        private void UpdateSelectedTarget()
        {
            ShareButtonEnabled = (SelectedTarget != null);
        }

        public async Task ActivateAsync(ShareTargetActivatedEventArgs args)
        {
            try
            {
                shareOperation = args.ShareOperation;

                LoadTargets();
                await ExtractShareData(args);
            }
            catch
            {
                shareOperation.ReportError("Could not determine the type of data to share");
            }
        }

        private async Task ExtractShareData(ShareTargetActivatedEventArgs args)
        {
            fileBytes = null;
            fileName = null;
            url = null;

            if (shareOperation.Data.Contains(StandardDataFormats.Uri))
            {
                Uri uri = await shareOperation.Data.GetUriAsync();
                url = uri.ToString();
            }

            if (shareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                var storageItems = await shareOperation.Data.GetStorageItemsAsync();

                StorageFile item = storageItems[0] as StorageFile;
                var properties = await item.GetBasicPropertiesAsync();
                var size = properties.Size;
                var fileStream = await item.OpenReadAsync();

                DataReader dataReader = new DataReader(fileStream);
                await dataReader.LoadAsync((uint)size);
                byte[] buffer = new byte[(int)size];

                dataReader.ReadBytes(buffer);

                fileBytes = buffer;
                fileName = shareOperation.Data.Properties.Description;
            }

            shareOperation.ReportDataRetrieved();
        }

        public void LoadTargets()
        {
            IsSearchingForPeers = true;
            manager.PerformPeerDiscovery();
        }

        public void SetDataToShare(string fileName, byte[] fileContents)
        {
            this.fileName = fileName;
            this.fileBytes = fileContents;
        }

        public void SetDataToShare(string url)
        {
            this.url = url;
        }

        public void TargetSelected(object target)
        {
            IsSharing = true;

            ShareRequest request = new ShareRequest();
            request.Url = this.url;
            request.FileContents = fileBytes;
            request.FileName = fileName;
            request.Target = this.SelectedTarget;

            manager.Share(request);

            IsSharing = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool DoesTargetRequirePin(ShareTarget target)
        {
            return target.IsPinCodeRequired;
        }
    }
}
