﻿using System;
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
        }

        public ObservableCollection<ShareTarget> Targets { get; private set; }

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

        public async void TargetSelected(object target, ShareOperation shareOperation)
        {
            ListenerClient client = new ListenerClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri("http://192.168.1.15:22001/BeCharming"));

            string result = null;

            if (shareOperation.Data.AvailableFormats.Contains("UniformResourceLocatorW"))
            {
                shareOperation.ReportSubmittedBackgroundTask();
                result = await client.OpenWebPageAsync(shareOperation.Data.Properties.Description);
            }

            if (shareOperation.Data.AvailableFormats.Contains("FileContents"))
            {
                shareOperation.ReportSubmittedBackgroundTask();
                var contents = await shareOperation.Data.GetDataAsync("FileContents");
                result = await client.OpenDocumentAsync(shareOperation.Data.Properties.Description, (byte[])contents);
            }

            if (result == "okay")
            {
                shareOperation.ReportCompleted();
            }
            else
            {
                shareOperation.ReportError("Failed to share that");
            }
        }
    }
}
