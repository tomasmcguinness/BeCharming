using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace BeCharming.TestClient
{
  public class Probe
  {
    public static void SendProbe()
    {
      Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      IPAddress ip = IPAddress.Parse("224.5.6.7");
      s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));

      s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);

      IPEndPoint ipep = new IPEndPoint(ip, 3702);
      s.Connect(ipep);

      byte[] b = new byte[10];
      for (int x = 0; x < b.Length; x++) b[x] = (byte)(x + 65);

      s.Send(b, b.Length, SocketFlags.None);

      s.Close();
    }
  }
}
