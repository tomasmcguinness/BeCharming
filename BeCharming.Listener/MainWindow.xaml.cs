using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Windows.Forms;

namespace BeCharming.Listener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceHost host = null;

        public MainWindow()
        {
            InitializeComponent();

            this.Visibility = Visibility.Collapsed;
            this.ShowInTaskbar = false;

            var server = new ListenerService();

            host = new ServiceHost(server, new Uri("http://localhost:22001/becharming"));

            var httpBinding = new BasicHttpBinding();
            httpBinding.MaxReceivedMessageSize = Int32.MaxValue;

            host.AddServiceEndpoint(typeof(IListener), httpBinding, String.Empty);

            ServiceMetadataBehavior metaDataBehavior = new ServiceMetadataBehavior();
            metaDataBehavior.HttpGetEnabled = true;
            host.Description.Behaviors.Add(metaDataBehavior);

            ServiceDiscoveryBehavior discoveryBehavior = new ServiceDiscoveryBehavior();
            discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());

            host.Description.Behaviors.Add(discoveryBehavior);
            host.AddServiceEndpoint(new UdpDiscoveryEndpoint());

            host.Open();
        }
    }
}
