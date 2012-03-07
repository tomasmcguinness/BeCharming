using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace BeCharming.Common
{
  [ServiceContract]
  public interface IListener
  {
    [OperationContract]
    string Echo(string message);
  }
}
