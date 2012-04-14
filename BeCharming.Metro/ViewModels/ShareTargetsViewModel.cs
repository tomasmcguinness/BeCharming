using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BeCharming.Common.ListenerService;
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

        public ShareTargetsViewModel()
        {
            Targets = new ObservableCollection<ShareTarget>();
            Share = new DelegateCommand(TargetSelected);
        }

        public ObservableCollection<ShareTarget> Targets { get; private set; }
        public ShareTarget SelectedTarget { get { return selectedTarget; } set { selectedTarget = value; UpdateSelectedTarget(); } }
        public ICommand Share { get; set; }
        public bool ShareButtonEnabled { get { return shareButtonEnabled; } set { shareButtonEnabled = value; NotifyPropertyChanged("ShareButtonEnabled"); } }

        private void UpdateSelectedTarget()
        {
            ShareButtonEnabled = (SelectedTarget != null);
        }

        public async Task ActivateAsync(ShareTargetActivatedEventArgs args)
        {
            LoadTargets();
            await ExtractShareData(args);
        }

        private async Task ExtractShareData(ShareTargetActivatedEventArgs args)
        {
            fileBytes = null;
            fileName = null;
            url = null;

            shareOperation = args.ShareOperation;

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
            
        }

        public async void TargetSelected(object target)
        {
            var serverPath = string.Format("net.tcp://{0}:22001/BeCharming", SelectedTarget.IPAddress);

            ListenerClient client = new ListenerClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(serverPath));

            string result = null;

            if (!string.IsNullOrEmpty(url))
            {
                result = await client.OpenWebPageAsync(url);
            }
            else
            {
                result = await client.OpenDocumentAsync(fileName, fileBytes);
            }

            shareOperation.ReportCompleted();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
