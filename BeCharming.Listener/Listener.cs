using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace BeCharming.Listener
{
    public class ListenerService : IListener
    {
        public string OpenWebPage(string urlToOpen)
        {
            Process runCmd = new Process();
            runCmd.StartInfo.FileName = @"C:\Program Files (x86)\Internet Explorer\IExplore.exe";
            runCmd.StartInfo.Arguments = urlToOpen;
            runCmd.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

            runCmd.StartInfo.UseShellExecute = false;
            runCmd.StartInfo.RedirectStandardOutput = true;
            runCmd.Start();

            return "okay";
        }

        public string OpenDocument(string documentName, byte[] documentBytes)
        {
            var tempPath = Path.GetTempPath();
            var fileName = string.Format("{0}/{1}/{2}", tempPath, Guid.NewGuid(), documentName);

            var tempFile = File.Create(fileName);
            tempFile.Write(documentBytes, 0, documentBytes.Length);

            // Store the file in a temp folder.
            Process runCmd = new Process();
            runCmd.StartInfo.FileName = fileName;// @"C:\Program Files (x86)\Internet Explorer\IExplore.exe";
            runCmd.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

            runCmd.StartInfo.UseShellExecute = false;
            runCmd.StartInfo.RedirectStandardOutput = true;
            runCmd.Start();

            return "okay";
        }
    }
}
