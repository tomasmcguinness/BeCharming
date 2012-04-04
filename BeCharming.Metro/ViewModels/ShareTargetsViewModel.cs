using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BeCharming.Common.ListenerService;
using Windows.ApplicationModel.DataTransfer.ShareTarget;

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
        public ShareOperation ShareOperation { get; set; }

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

            ListenerClient client = new ListenerClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri("http://192.168.10.101:22001/BeCharming"));

            string result = null;

            ShareOperation.ReportStarted();

            if (ShareOperation.Data.AvailableFormats.Contains("UniformResourceLocatorW"))
            {
                ShareOperation.ReportSubmittedBackgroundTask();
                result = await client.OpenWebPageAsync(ShareOperation.Data.Properties.Description);
            }

            if (ShareOperation.Data.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
            {
                //var contents = await ShareOperation.Data.GetDataAsync("FileContents");

                var storageItems = await ShareOperation.Data.GetStorageItemsAsync();

                //result = await client.OpenDocumentAsync(ShareOperation.Data.Properties.Description, (byte[])contents);
            }

            ShareOperation.ReportDataRetrieved();

            if (result == "okay")
            {
                ShareOperation.ReportCompleted();
            }
            else
            {
                ShareOperation.ReportError("Failed to share that");
            }
        }
    }
}
