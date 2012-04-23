using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeCharming.Metro.ViewModels
{
    public class ShareTarget : IEquatable<ShareTarget>
    {
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public bool IsSelected { get; set; }
        public int ShareCount { get; set; }

        public int Width { get { return 1 + ShareCount; } }
        public int Height { get { return 1 + ShareCount; } }

        public bool Equals(ShareTarget other)
        {
            if (object.ReferenceEquals(this, other)) return true;

            var areEqual = Name == other.Name && IPAddress == other.IPAddress;

            return areEqual;
        }
    }
}
