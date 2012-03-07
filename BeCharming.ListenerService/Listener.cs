using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeCharming.Common;

namespace BeCharming.ListenerService
{
  public class Listener : IListener
  {
    public string Echo(string message)
    {
      return message;
    }
  }
}
