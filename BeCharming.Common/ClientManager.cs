using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT.WSDiscovery;

namespace BeCharming.Common
{
    public class ClientManager
    {
        public delegate void ListenerDiscoveredHandler(string serviceName);
        public event ListenerDiscoveredHandler ListenerDiscovered;

        public void DiscoverServices()
        {
            DiscoveryClient c = new DiscoveryClient();
            c.Find();

            //DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            //FindResponse discoveryResponse = discoveryClient.Find(new FindCriteria(typeof(IListener)));

            //foreach (var endpoint in discoveryResponse.Endpoints)
            //{
            //    EndpointAddress address = endpoint.Address;

            //ListenerClient service = new ListenerClient(new BasicHttpBinding(), address);
            //    //ervice.FetchDetails();

            //    if (ListenerDiscovered != null)
            //    {
            //        ListenerDiscovered("Hello");
            //    }
            //}
        }

        public void OpenUrl(string url)
        {




        }
    }
}
