using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Windows;

namespace BeCharming.Listener
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceHost host = null;

        public App()
        {
            var server = new ListenerService();

            host = new ServiceHost(server, new Uri("net.tcp://localhost:22001/becharming"));

            var binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = Int32.MaxValue;

            host.AddServiceEndpoint(typeof(IListener), binding, String.Empty);
            host.AddServiceEndpoint(typeof(IMetadataExchange), binding, "MEX");

            ServiceMetadataBehavior metadataBehavior;
            metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();

            if (metadataBehavior == null)
            {
                metadataBehavior = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(metadataBehavior);
            }

            //ServiceMetadataBehavior metaDataBehavior = new ServiceMetadataBehavior();
            //metaDataBehavior.HttpGetEnabled = true;
            //host.Description.Behaviors.Add(metaDataBehavior);

            ServiceDiscoveryBehavior discoveryBehavior = new ServiceDiscoveryBehavior();
            discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());

            host.Description.Behaviors.Add(discoveryBehavior);
            host.AddServiceEndpoint(new UdpDiscoveryEndpoint());

            host.Open();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            host.Close();
        }
    }
}
