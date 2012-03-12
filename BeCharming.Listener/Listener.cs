using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeCharming.Common;
using System.Diagnostics;

namespace BeCharming.Listener
{
  public class ListenerService : IListener
  {
    public string Echo(string message)
    {
      Process runCmd = new Process();
      runCmd.StartInfo.FileName = @"C:\Program Files (x86)\Internet Explorer\IExplore.exe";
      runCmd.StartInfo.Arguments = message;
      runCmd.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

      runCmd.StartInfo.UseShellExecute = false;
      runCmd.StartInfo.RedirectStandardOutput = true;
      runCmd.Start();

      return "okay";
    }

    public DiscoveryDetails PerformDiscovery()
    {
      return new DiscoveryDetails()
      {
        Name = "My First Server"
      };
    }
  }
}
