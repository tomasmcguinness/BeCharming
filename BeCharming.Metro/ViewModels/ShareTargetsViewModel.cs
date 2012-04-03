using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BeCharming.Metro.ViewModels
{
    public class ShareTargetsViewModel
    {
        public ShareTargetsViewModel()
        {
            Targets = new ObservableCollection<ShareTarget>();
            TargetSelectedCommand = new DelegateCommand(TargetSelected);
        }

        public ObservableCollection<ShareTarget> Targets { get; private set; }
        public ICommand TargetSelectedCommand { get; set; }

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

                var shareTargets = ObjectSerializer<List<String>>.FromXml(xml);

                foreach (var target in shareTargets)
                {
                    ShareTarget t = new ShareTarget();
                    t.Name = "Test";
                    t.IP = target;
                    Targets.Add(t);
                }
            }
        }

        public void TargetSelected(object state)
        {
            //var service = new ListenerClient();
        }
    }
}
