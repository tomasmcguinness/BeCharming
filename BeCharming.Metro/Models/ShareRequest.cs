using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeCharming.Metro.ViewModels;

namespace BeCharming.Metro.Models
{
    public class ShareRequest
    {
        public ShareTarget Target { get; set; }
        public byte[] FileContents { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}
