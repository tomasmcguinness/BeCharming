using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
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
            GenerateAndStoreShareTargetCode();

            StartListeningForDiscoveryBroadcasts();

            var server = new ListenerService(icon);

            var tcpBaseAddress = new Uri("net.tcp://0.0.0.0:22001/becharming");

            host = new ServiceHost(server, tcpBaseAddress);

            var binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
            binding.Security.Mode = SecurityMode.None;

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

            host.Open();
        }

        private void StartListeningForDiscoveryBroadcasts()
        {
            IPAddress addr = IPAddress.Parse("230.0.0.1");
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 22003);
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

            u.BeginReceive(ReceiveCallback, (UdpState)(ar.AsyncState));

            Byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            string s = string.Format("{0}|{1}|{2}|{3}", System.Environment.MachineName, false, 5, GetShareTargetCode());
            int bytesSent = u.Send(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes(s).Length, e);
        }

        public string GetShareTargetCode()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly, null, null);

            using (StreamReader reader = new StreamReader(new IsolatedStorageFileStream("BeCharmingTargetName.txt", FileMode.Open, isoStore)))
            {
                String sb = reader.ReadLine();
                return sb.ToString();
            }
        }

        public void GenerateAndStoreShareTargetCode()
        {
            Guid shareTargetCode = Guid.NewGuid();

            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly, null, null);

            try
            {
                IsolatedStorageFileStream stream = new IsolatedStorageFileStream("BeCharmingTargetName.txt", FileMode.CreateNew, isoStore);

                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(shareTargetCode.ToString());
                }

            }
            catch (IOException)
            {
                // The file could not be created. That's okay. TODO Should be using a File.Exists style test.
            }
        }

        public void Stop()
        {
            if (host != null)
            {
                host.Close();
            }

            if (client != null)
            {
                client.Close();
            }
        }
    }
}
