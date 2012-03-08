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
using BeCharming.Common;
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

      NotifyIcon icon = new NotifyIcon();
      icon.Visible = true;
      icon.Icon = new System.Drawing.Icon("Icon1.ico");
      icon.ShowBalloonTip(5, "BeCharming", "Running...", ToolTipIcon.Info);

      host = new ServiceHost(typeof(ListenerService), new Uri("http://localhost:10001/becharming"));
      host.AddServiceEndpoint(typeof(IListener), new BasicHttpBinding(), String.Empty);

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
