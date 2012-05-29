using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.ServiceModel;

namespace BeCharming.Listener
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ListenerService : IListener
    {
        private NotifyIcon icon;

        public ListenerService(NotifyIcon icon)
        {
            this.icon = icon;
        }

        public string OpenWebPage(string urlToOpen, string pinCode)
        {
            if (!IsPinCodeCorrect(pinCode))
            {
                return "InvalidPin";
            }

            icon.ShowBalloonTip(3, "BeCharming", "Opening " + urlToOpen, ToolTipIcon.Info);            
            Process.Start(urlToOpen);

            return "okay";
        }

        public string OpenDocument(string documentName, byte[] documentBytes, string pinCode)
        {
            if (!IsPinCodeCorrect(pinCode))
            {
                return "InvalidPin";
            }

            // TODO Open in background thread
            //
            icon.ShowBalloonTip(3, "BeCharming", "Opening " + documentName, ToolTipIcon.Info);

            try
            {
                var tempPath = Path.GetTempPath();
                var filePath = string.Format("{0}/{1}", tempPath, Guid.NewGuid());
                var fileName = string.Format("{0}/{1}", filePath, documentName);

                Directory.CreateDirectory(filePath);

                using (var tempFile = File.Create(fileName))
                {
                    tempFile.Write(documentBytes, 0, documentBytes.Length);
                    tempFile.Flush();
                }

                Process.Start(fileName);

                return "okay";
            }
            catch
            {
                return "error";
            }
        }

        private bool IsPinCodeCorrect(string pinCode)
        {
            return pinCode == "1234";
        }
    }
}
