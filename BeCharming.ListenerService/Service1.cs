using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Network.Bonjour;
using Network.ZeroConf;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using BeCharming.Common;
using System.ServiceModel.Description;

namespace BeCharming.ListenerService
{
  public partial class Service1 : ServiceBase
  {
    private ServiceHost host;

    public Service1()
    {
      InitializeComponent();
    }

    protected override void OnStart(string[] args)
    {
      host = new ServiceHost(typeof(Listener), new Uri("http://localhost:10001/becharming"));
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

    protected override void OnStop()
    {
      host.Close();
    }
  }
}
