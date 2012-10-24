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
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Windows.Forms;

namespace BeCharming.Listener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Visibility = Visibility.Collapsed;
            this.ShowInTaskbar = false;
            this.DataContext = this;
        }

        public bool OpenItemsAutomatically
        {
            get
            {
                return Settings.OpenItemsAutomatically;
            }
            set
            {
                Settings.OpenItemsAutomatically = value;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            e.Cancel = true;
        }

    }
}
