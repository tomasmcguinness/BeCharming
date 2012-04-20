using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeCharming.Listener
{
    class UdpState
    {
        public System.Net.IPEndPoint e { get; set; }

        public System.Net.Sockets.UdpClient u { get; set; }
    }
}
