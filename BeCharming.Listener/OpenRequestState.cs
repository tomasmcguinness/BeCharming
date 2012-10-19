using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeCharming.Listener
{
    class OpenRequestState
    {
        public bool IsUrl { get; set; }
        public string UrlToOpen { get; set; }
        public string DocumentName { get; set; }
        public byte[] DocumentBody { get; set; }
    }
}
