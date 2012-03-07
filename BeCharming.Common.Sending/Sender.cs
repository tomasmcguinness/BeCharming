using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ServiceModel.Discovery;
using System.ServiceModel;
using BeCharming.Common.Sending.BeCharming.Common.Service;

namespace BeCharming.Common.Sending
{
  public class Sender
  {
    public void DiscoverServices()
    {
      DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
      FindResponse discoveryResponse = discoveryClient.Find(new FindCriteria(typeof(IListener)));
      EndpointAddress address = discoveryResponse.Endpoints[0].Address;

      ListenerClient service = new ListenerClient(new BasicHttpBinding(), address);
      service.Echo("WS-Discovery test");
    }
  }
}
