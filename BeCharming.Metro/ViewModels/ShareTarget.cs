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

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
