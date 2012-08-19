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

            // TODO Open in background thread
            //
            icon.ShowBalloonTip(3, "BeCharming", "Opening " + urlToOpen, ToolTipIcon.Info);

            Process runCmd = new Process();
            runCmd.StartInfo.FileName = @"C:\Program Files (x86)\Internet Explorer\IExplore.exe";
            runCmd.StartInfo.Arguments = urlToOpen;
            runCmd.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

            runCmd.StartInfo.UseShellExecute = false;
            runCmd.StartInfo.RedirectStandardOutput = true;
            runCmd.Start();

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

                // Store the file in a temp folder.
                Process runCmd = new Process();
                runCmd.StartInfo.FileName = fileName;// @"C:\Program Files (x86)\Internet Explorer\IExplore.exe";
                runCmd.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

                runCmd.StartInfo.UseShellExecute = true;
                runCmd.StartInfo.RedirectStandardOutput = false;
                runCmd.Start();

                return "okay";
            }
            catch
            {
                return "error";
            }
        }

        private bool IsPinCodeCorrect(string pinCode)
        {
            return true;
            //return pinCode == "1234";
        }
    }
}
