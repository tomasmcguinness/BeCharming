using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BeCharming.Common.ListenerService;

namespace BeCharming.Metro.ViewModels
{
    public class ShareTargetsViewModel
    {
        public ShareTargetsViewModel()
        {
            Targets = new ObservableCollection<ShareTarget>();
            ShareTargetTapped = new DelegateCommand(TargetSelected);
        }

        public ObservableCollection<ShareTarget> Targets { get; private set; }
        public ShareTarget SelectedTarget { get; set; }
        public ICommand ShareTargetTapped { get; set; }

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

        public void TargetSelected(object state)
        {
            ListenerClient client = new ListenerClient();
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri("http://192.168.1.15:10001/BeCharming"));
            client.OpenWebPageAsync("http://www.google.com");
        }
    }
}
