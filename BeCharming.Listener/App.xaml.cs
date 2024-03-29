﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;

namespace BeCharming.Listener
{
    public partial class App : System.Windows.Application
    {
        private System.Windows.Forms.NotifyIcon icon;
        private SharingManager manager;

        public App()
        {
            try
            {
                icon = CreateIcon();

                manager = new SharingManager();
                manager.Start(icon);

                ClickOnceHelper.AddShortcutToStartupGroup("Tomas McGuinness", "BeCharming");

                this.MainWindow = new MainWindow();
            }
            catch
            {
                // Throw up a popup?
            }
        }

        protected override void OnExit(System.Windows.ExitEventArgs e)
        {
            base.OnExit(e);
            icon.Dispose();
            manager.Stop();
        }

        private System.Windows.Forms.NotifyIcon CreateIcon()
        {
            icon = new System.Windows.Forms.NotifyIcon();
            icon.Visible = true;
            icon.Icon = new System.Drawing.Icon("small-orange-sphere.ico");
            icon.ContextMenu = GetContextMenu();

            icon.ShowBalloonTip(3, "BeCharming", "Ready to receive shared items...", System.Windows.Forms.ToolTipIcon.Info);

            return icon;
        }

        private System.Windows.Forms.ContextMenu GetContextMenu()
        {
            System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();
            
            var exitItem = new System.Windows.Forms.MenuItem() { Text = "Exit" };
            exitItem.Click += exitItem_Click;

            var settingsItem = new System.Windows.Forms.MenuItem() { Text = "Settings" };
            settingsItem.Click += settingsItem_Click;

            menu.MenuItems.Add(settingsItem);
            menu.MenuItems.Add(exitItem);

            return menu;
        }

        void exitItem_Click(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        void settingsItem_Click(object sender, EventArgs e)
        {
          this.MainWindow.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
