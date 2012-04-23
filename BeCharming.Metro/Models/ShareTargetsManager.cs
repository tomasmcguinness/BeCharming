using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeCharming.Metro.ViewModels;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace BeCharming.Metro.Models
{
    public class ShareTargetManager
    {
        public event EventHandler TargetsUpdated;
        public event EventHandler PeerDiscoveryComplete;
        private DatagramSocket socket;
        private DispatcherTimer timer;

        public ShareTargetManager()
        {
            socket = new DatagramSocket();
            socket.MessageReceived += socket_MessageReceived;

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;

            Targets = new List<ShareTarget>();
            UpdateTargets();
        }

        void timer_Tick(object sender, object e)
        {
            timer.Stop();


            if (PeerDiscoveryComplete != null)
            {
                PeerDiscoveryComplete(this, null);
            }
        }

        private void UpdateTargets()
        {
            Targets.Clear();

            foreach (var target in GetShareTargets())
            {
                Targets.Add(target);
            }

            if (TargetsUpdated != null)
            {
                TargetsUpdated(this, null);
            }
        }

        public List<ShareTarget> Targets { get; set; }

        public async void PerformPeerDiscovery()
        {
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Start();

            await socket.BindEndpointAsync(null, "22002");

            var outputStream = await socket.GetOutputStreamAsync(new HostName("230.0.0.1"), "22002");
            DataWriter wr = new DataWriter(outputStream);
            wr.WriteString("**BECHARMING DISCOVERY**");
            await wr.FlushAsync();
            await wr.StoreAsync();
        }

        void socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            DataReader dr = args.GetDataReader();

            Targets.Add(new ShareTarget() { Name = "Discovered" });

            TargetsUpdated(this, null);
        }

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

            UpdateTargets();
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

            UpdateTargets();
        }

        public void DeleteShareTarget(ShareTarget shareTarget)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var container = localSettings.CreateContainer("BeCharmingSettings", Windows.Storage.ApplicationDataCreateDisposition.Always);

            List<ShareTarget> shareTargets = null;

            if (container.Values["ShareTargets"] != null)
            {
                shareTargets = ObjectSerializer<List<ShareTarget>>.FromXml(container.Values["ShareTargets"] as string);
            }

            shareTargets.Remove(shareTarget);

            var xml = ObjectSerializer<List<ShareTarget>>.ToXml(shareTargets);

            container.Values["ShareTargets"] = xml;

            UpdateTargets();
        }
    }
}
