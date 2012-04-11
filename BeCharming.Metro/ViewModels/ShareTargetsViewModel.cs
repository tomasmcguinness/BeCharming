using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ShareTargetsViewModel
    {
        public ShareTargetsViewModel()
        {
            Targets = new ObservableCollection<ShareTarget>();
            Share = new DelegateCommand(TargetSelected);
        }

        public ObservableCollection<ShareTarget> Targets { get; private set; }
        public ICommand Share { get; set; }

        private ShareOperation shareOperation;
        private byte[] fileBytes;
        private string fileName;
        private string url;

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

            ShareOperation operation = args.ShareOperation;

            if (operation.Data.Contains(StandardDataFormats.Uri))
            {
                Uri uri = await operation.Data.GetUriAsync();
                url = uri.ToString();
            }

            if (operation.Data.Contains(StandardDataFormats.StorageItems))
            {
                var storageItems = await operation.Data.GetStorageItemsAsync();

                StorageFile item = storageItems[0] as StorageFile;
                var properties = await item.GetBasicPropertiesAsync();
                var size = properties.Size;
                var fileStream = await item.OpenReadAsync();

                DataReader dataReader = new DataReader(fileStream);
                await dataReader.LoadAsync((uint)size);
                byte[] buffer = new byte[(int)size];

                dataReader.ReadBytes(buffer);

                fileBytes = buffer;
                fileName = operation.Data.Properties.Description;
            }
        }

        public void LoadTargets()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Containers.ContainsKey("BeCharmingSettings"))
            {
                var container = localSettings.Containers["BeCharmingSettings"];

                var xml = container.Values["ShareTargets"] as String;

                if (xml == null)
                {
                    return;
                }

                var shareTargets = ObjectSerializer<List<ShareTarget>>.FromXml(xml);

                Targets.Clear();

                foreach (var target in shareTargets)
                {
                    Targets.Add(target);
                }
            }
        }

        public async void TargetSelected(object target)
        {
            ShareTarget shareTarget = target as ShareTarget;

            var serverPath = string.Format("net.tcp://{0}:22001/BeCharming", shareTarget.IPAddress);

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
    }
}
