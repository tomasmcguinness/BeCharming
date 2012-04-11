using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;

namespace BeCharming.Listener
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private ServiceHost host = null;
        private System.Windows.Forms.NotifyIcon icon;

        public App()
        {
            this.icon = CreateIcon();
            var server = new ListenerService(this.icon);

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

            ServiceDiscoveryBehavior discoveryBehavior = new ServiceDiscoveryBehavior();
            discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());

            host.Description.Behaviors.Add(discoveryBehavior);
            host.AddServiceEndpoint(new UdpDiscoveryEndpoint());

            host.Open();

            ClickOnceHelper.AddShortcutToStartupGroup("Tomas McGuinness", "BeCharming");
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
