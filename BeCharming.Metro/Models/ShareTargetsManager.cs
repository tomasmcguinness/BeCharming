using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BeCharming.Common.ListenerService;
using BeCharming.Metro.ViewModels;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace BeCharming.Metro.Models
{
    public class ShareTargetsManager
    {
        public event EventHandler ShareStarted;
        public event EventHandler ShareComplete;
        public event EventHandler ShareFailed;
        public event EventHandler PeerDiscoveryComplete;
        public delegate void PeerDiscoveredHandler(ShareTarget shareTarget);
        public event PeerDiscoveredHandler PeerDiscovered;
        private DatagramSocket socket;
        private DispatcherTimer timer;

        public ShareTargetsManager()
        {
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, object e)
        {
            timer.Stop();
            socket.Dispose();
            socket = null;

            if (PeerDiscoveryComplete != null)
            {
                PeerDiscoveryComplete(this, null);
            }
        }

        public async void PerformPeerDiscovery()
        {
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Start();

            if (socket == null)
            {
                socket = new DatagramSocket();
                socket.MessageReceived += socket_MessageReceived;

                try
                {
                    await socket.BindEndpointAsync(null, "22002");
                }
                catch
                {
                    // Swallow any already bound exceptions!
                }
            }

            using (var outputStream = await socket.GetOutputStreamAsync(new HostName("230.0.0.1"), "22003"))
            {
                using (DataWriter wr = new DataWriter(outputStream))
                {
                    wr.WriteString("**BECHARMING DISCOVERY**");
                    await wr.FlushAsync();
                    await wr.StoreAsync();
                }
            }
        }

        void socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            DataReader dr = args.GetDataReader();
            var dataLength = dr.UnconsumedBufferLength;

            string discoveryResult = dr.ReadString(dataLength);
            string[] parts = discoveryResult.Split('|');
            string name = parts[0];
            bool isPinProtected = bool.Parse(parts[1]);
            string uniqueName = parts[3];

            var existingTarget = GetShareTarget(uniqueName);

            var discoveredTarget = new ShareTarget()
            {
                Name = name,
                IPAddress = args.RemoteAddress.DisplayName,
                IsPinCodeRequired = isPinProtected,
                ShareTargetUniqueName = uniqueName
            };

            if (existingTarget != null) discoveredTarget.ShareCount = existingTarget.ShareCount;

            if (PeerDiscovered != null)
            {
                PeerDiscovered(discoveredTarget);
            }
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
        }

        public void InitShareTargetStorage()
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

            var xml = ObjectSerializer<List<ShareTarget>>.ToXml(shareTargets);

            container.Values["ShareTargets"] = xml;
        }

        public ShareTarget GetShareTarget(string uniqueName)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var container = localSettings.CreateContainer("BeCharmingSettings", Windows.Storage.ApplicationDataCreateDisposition.Always);

            List<ShareTarget> shareTargets = null;

            if (container.Values["ShareTargets"] != null)
            {
                shareTargets = ObjectSerializer<List<ShareTarget>>.FromXml(container.Values["ShareTargets"] as string);
            }

            if (shareTargets == null) return null;

            var target = shareTargets.Where(t => t.ShareTargetUniqueName == uniqueName).SingleOrDefault();

            return target;
        }

        public void IncrementShareCount(ShareTarget target)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var container = localSettings.CreateContainer("BeCharmingSettings", Windows.Storage.ApplicationDataCreateDisposition.Always);

            List<ShareTarget> shareTargets = null;

            if (container.Values["ShareTargets"] != null)
            {
                shareTargets = ObjectSerializer<List<ShareTarget>>.FromXml(container.Values["ShareTargets"] as string);
            }
            else
            {
                shareTargets = new List<ShareTarget>();
                container.Values["ShareTargets"] = ObjectSerializer<List<ShareTarget>>.ToXml(shareTargets);
            }

            var targetToUpdate = shareTargets.Where(t => t.ShareTargetUniqueName == target.ShareTargetUniqueName).SingleOrDefault();

            target.ShareCount++;

            if (targetToUpdate == null)
            {
                shareTargets.Add(target);
            }
            else
            {
                targetToUpdate.ShareCount = target.ShareCount;
            }

            var xml = ObjectSerializer<List<ShareTarget>>.ToXml(shareTargets);

            container.Values["ShareTargets"] = xml;
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
        }

        public bool RequiresPin(ShareRequest request)
        {
            return request.Target.IsPinCodeRequired;
        }

        public async void Share(ShareRequest request)
        {
            var serverPath = string.Format("net.tcp://{0}:22001/BeCharming", request.Target.IPAddress);

            ListenerClient client = new ListenerClient();
            ((NetTcpBinding)client.Endpoint.Binding).Security.Mode = SecurityMode.None;
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(serverPath));

            string result = null;

            if (ShareStarted != null)
            {
                ShareStarted(this, null);
            }

            // The PinCode should be forward hashed so it cannot be read??

            if (!string.IsNullOrEmpty(request.Url))
            {
                result = await client.OpenWebPageAsync(request.Url, request.PinCode);
            }
            else
            {
                result = await client.OpenDocumentAsync(request.FileName, request.FileContents, request.PinCode);
            }

            if (result == "okay")
            {
                IncrementShareCount(request.Target);

                if (ShareComplete != null)
                {
                    ShareComplete(this, null);
                }
            }
            else
            {
                if (ShareFailed != null)
                {
                    ShareFailed(this, null);
                }

            }
        }
    }
}
