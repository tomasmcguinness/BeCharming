using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;

namespace BeCharming.Listener
{
    public class SharingManager
    {
        private UdpClient client = null;
        private ServiceHost host = null;

        public void Start(System.Windows.Forms.NotifyIcon icon)
        {
            StartListeningForDiscoveryBroadcasts();

            var server = new ListenerService(icon);

            var tcpBaseAddress = new Uri("net.tcp://localhost:22001/becharming");

            host = new ServiceHost(server, tcpBaseAddress);

            var binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = Int32.MaxValue;

            host.AddServiceEndpoint(typeof(IListener), binding, String.Empty);

            ServiceMetadataBehavior metadataBehavior;
            metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();

            if (metadataBehavior == null)
            {
                metadataBehavior = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(metadataBehavior);
            }

            Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
            host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "MEX");

            //ServiceDiscoveryBehavior discoveryBehavior = new ServiceDiscoveryBehavior();
            //discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());

            //host.Description.Behaviors.Add(discoveryBehavior);

            host.AddServiceEndpoint(new UdpDiscoveryEndpoint());

            host.Open();
        }

        private void StartListeningForDiscoveryBroadcasts()
        {
            IPAddress addr = IPAddress.Parse("230.0.0.1");
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 22002);
            client = new UdpClient(ep);
            client.EnableBroadcast = true;
            client.MulticastLoopback = true;
            client.AllowNatTraversal(true);
            client.JoinMulticastGroup(addr);

            UdpState s = new UdpState();
            s.e = ep;
            s.u = client;

            client.BeginReceive(ReceiveCallback, s);
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

            Byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            var settings = Settings.LoadSettings();

            string s = string.Format("{0}|{1}|{2}", settings.Name, settings.IsPinProtected, 5);
            int bytesSent = u.Send(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes(s).Length, e);
        }

        public void Stop()
        {
            host.Close();
            client.Close();
        }
    }
}
