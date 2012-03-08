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
    public delegate void ListenerDiscoveredHandler(string serviceName);
    public event ListenerDiscoveredHandler ListenerDiscovered;

    public void DiscoverServices()
    {
      DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
      FindResponse discoveryResponse = discoveryClient.Find(new FindCriteria(typeof(IListener)));

      foreach (var endpoint in discoveryResponse.Endpoints)
      {
        EndpointAddress address = endpoint.Address;

        //ListenerClient service = new ListenerClient(new BasicHttpBinding(), address);
        //ervice.FetchDetails();

        if (ListenerDiscovered != null)
        {
          ListenerDiscovered("Hello");
        }
      }
    }
  }
}
