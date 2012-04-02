using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace WinRT.WSDiscovery
{
    public class DiscoveryClient
    {
        public DiscoveryClient()
        {

        }

        public async void Find()
        {
            DatagramSocket s = new DatagramSocket();
            await s.BindEndpointAsync(new HostName("127.0.0.1:3702"), "BeCharming");

        }
    }
}
