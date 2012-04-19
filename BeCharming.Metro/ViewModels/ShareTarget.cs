using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeCharming.Metro.ViewModels
{
    public class ShareTarget
    {
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public bool IsSelected { get; set; }
        public int ShareCount { get; set; }

        public int Width { get { return 1 + ShareCount; } }
        public int Height { get { return 1 + ShareCount; } }
    }
}
