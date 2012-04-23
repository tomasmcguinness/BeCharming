using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeCharming.Listener
{
    public class Settings
    {
        public string Name { get; set; }
        public string PinCode { get; set; }
        public bool IsPinProtected { get; set; }

        public static Settings LoadSettings()
        {
            return null;
        }

        public static void SaveSettings()
        {

        }
    }
}
