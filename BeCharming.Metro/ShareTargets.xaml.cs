using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeCharming.Metro.ViewModels;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BeCharming.Metro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShareTargets : UserControl
    {
        public ShareTargets()
        {
            this.InitializeComponent();
            this.DataContext = new ShareTargetsViewModel(Dispatcher);
        }

        public async Task ActivateAsync(ShareTargetActivatedEventArgs args)
        {
            await ((ShareTargetsViewModel)DataContext).ActivateAsync(args);
        }
    }
}
