using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BeCharming.Listener
{
    [ServiceContract]
    public interface IListener
    {
        [OperationContract]
        string OpenWebPage(string urlToOpen);
    }
}
