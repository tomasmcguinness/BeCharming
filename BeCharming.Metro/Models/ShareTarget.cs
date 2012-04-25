using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BeCharming.Metro.ViewModels
{
    public class ShareTarget : IEquatable<ShareTarget>, INotifyPropertyChanged
    {
        private int shareCount;

        public string Name { get; set; }
        public string IPAddress { get; set; }
        public bool IsPinCodeRequired { get; set; }
        public string ShareTargetUniqueName { get; set; }
        public int ShareCount { get { return shareCount; } set { shareCount = value; NotifyPropertyChanged("ShareCount"); } }

        public bool IsSelected { get; set; }

        public int Width { get { return 1 + ShareCount; } }
        public int Height { get { return 1 + ShareCount; } }

        public bool Equals(ShareTarget other)
        {
            if (object.ReferenceEquals(this, other)) return true;

            var areEqual = ShareTargetUniqueName == other.ShareTargetUniqueName;

            return areEqual;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
