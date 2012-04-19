using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeCharming.Metro.ViewModels;

namespace BeCharming.Metro.Models
{
    public class ShareTargets
    {
        public ShareTargets()
        {
            Targets = new ObservableCollection<ShareTarget>(GetShareTargets());
        }

        public ObservableCollection<ShareTarget> Targets { get; set; }

        public List<ShareTarget> GetShareTargets()
        {
            List<ShareTarget> targets = new List<ShareTarget>();

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Containers.ContainsKey("BeCharmingSettings"))
            {
                var container = localSettings.Containers["BeCharmingSettings"];

                List<ShareTarget> shareTargets = null;

                if (container.Values["ShareTargets"] != null)
                {
                    shareTargets = ObjectSerializer<List<ShareTarget>>.FromXml(container.Values["ShareTargets"] as string);
                }

                if (shareTargets != null)
                {
                    foreach (var target in shareTargets)
                    {
                        targets.Add(target);
                    }
                }
            }

            return targets;
        }

        public void AddShareTarget(ShareTarget target)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var container = localSettings.CreateContainer("BeCharmingSettings", Windows.Storage.ApplicationDataCreateDisposition.Always);

            List<ShareTarget> shareTargets = null;

            if (container.Values["ShareTargets"] != null)
            {
                shareTargets = ObjectSerializer<List<ShareTarget>>.FromXml(container.Values["ShareTargets"] as string);
            }

            if (shareTargets == null)
            {
                shareTargets = new List<ShareTarget>();
            }

            shareTargets.Add(target);

            var xml = ObjectSerializer<List<ShareTarget>>.ToXml(shareTargets);

            container.Values["ShareTargets"] = xml;

            Targets.Add(target);
        }

        public void IncrementShareCount(object target)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var container = localSettings.CreateContainer("BeCharmingSettings", Windows.Storage.ApplicationDataCreateDisposition.Always);

            List<ShareTarget> shareTargets = null;

            if (container.Values["ShareTargets"] != null)
            {
                shareTargets = ObjectSerializer<List<ShareTarget>>.FromXml(container.Values["ShareTargets"] as string);
            }

            shareTargets[0].ShareCount++;

            var xml = ObjectSerializer<List<ShareTarget>>.ToXml(shareTargets);

            container.Values["ShareTargets"] = xml;
        }
    }
}
