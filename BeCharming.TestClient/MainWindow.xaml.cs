using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeCharming.TestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //Sender serviceSender = new Sender();
            //serviceSender.ListenerDiscovered += new Sender.ListenerDiscoveredHandler(serviceSender_ListenerDiscovered);
            //serviceSender.DiscoverServices();
            Probe.SendProbe();
        }

        void serviceSender_ListenerDiscovered(string serviceName)
        {
            listBox1.Items.Add(serviceName);
        }
    }
}
