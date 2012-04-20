using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;

namespace BeCharming.Listener
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private ServiceHost host = null;
        private System.Windows.Forms.NotifyIcon icon;

        UdpClient client = null;

        public App()
        {
            //// Sending data to remote host.
            //IPAddress addr = IPAddress.Parse("192.168.10.101");
            //IPEndPoint ep2 = new IPEndPoint(addr, 22002);
            //client = new UdpClient();
            //client.Connect(ep2);
            //string s = "**TEST** This is a test of emergency broadcast channel **";
            //int bytesSent = client.Send(Encoding.UTF8.GetBytes(s), Encoding.UTF8.GetBytes(s).Length);

            //IPAddress addr = IPAddress.Parse("192.168.10.100");
            //IPEndPoint ep = new IPEndPoint(addr, 22002);
            //client = new UdpClient(ep);
            //client.EnableBroadcast = true;
            //UdpState s = new UdpState();
            //s.e = ep;
            //s.u = client;

            //IPEndPoint epOut = null;
            //var received = client.Receive(ref epOut);
            //var returned = Encoding.UTF8.GetString(received);

            IPAddress addr = IPAddress.Parse("224.0.0.1");
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 22002);
            client = new UdpClient(ep);
            client.JoinMulticastGroup(addr);

            IPEndPoint epOut = null;
            var received = client.Receive(ref epOut);
            var returned = Encoding.UTF8.GetString(received);

            // Original code.
            //this.icon = CreateIcon();
            //var server = new ListenerService(this.icon);

            //var tcpBaseAddress = new Uri("net.tcp://localhost:22001/becharming");

            //host = new ServiceHost(server, tcpBaseAddress);

            //var binding = new NetTcpBinding();
            //binding.MaxReceivedMessageSize = Int32.MaxValue;

            //host.AddServiceEndpoint(typeof(IListener), binding, String.Empty);

            //ServiceMetadataBehavior metadataBehavior;
            //metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();

            //if (metadataBehavior == null)
            //{
            //    metadataBehavior = new ServiceMetadataBehavior();
            //    host.Description.Behaviors.Add(metadataBehavior);
            //}

            //Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
            //host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, "MEX");

            //ServiceDiscoveryBehavior discoveryBehavior = new ServiceDiscoveryBehavior();
            //discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());

            //host.Description.Behaviors.Add(discoveryBehavior);
            //host.AddServiceEndpoint(new UdpDiscoveryEndpoint());

            //host.Open();

            //ClickOnceHelper.AddShortcutToStartupGroup("Tomas McGuinness", "BeCharming");
        }

        public static void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

            Byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            Console.WriteLine("Received: {0}", receiveString);
        }


        protected override void OnExit(System.Windows.ExitEventArgs e)
        {
            base.OnExit(e);
            icon.Dispose();
            host.Close();
        }

        private System.Windows.Forms.NotifyIcon CreateIcon()
        {
            icon = new System.Windows.Forms.NotifyIcon();
            icon.Visible = true;
            icon.Icon = new System.Drawing.Icon("Icon1.ico");
            icon.ShowBalloonTip(3, "BeCharming", "Ready to receive shared items...", System.Windows.Forms.ToolTipIcon.Info);
            icon.ContextMenu = GetContextMenu();

            return icon;
        }

        private System.Windows.Forms.ContextMenu GetContextMenu()
        {
            System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();
            var exitItem = new System.Windows.Forms.MenuItem() { Text = "Exit" };
            exitItem.Click += exitItem_Click;
            menu.MenuItems.Add(exitItem);

            return menu;
        }

        void exitItem_Click(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
