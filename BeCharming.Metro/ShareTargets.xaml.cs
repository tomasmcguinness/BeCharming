using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeCharming.Metro.ViewModels;
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
    public sealed partial class ShareTargets : Page
    {
        public ShareTargets()
        {
            this.InitializeComponent();
            this.DataContext = new ShareTargetsViewModel();
            ((ShareTargetsViewModel)DataContext).LoadTargets();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public Windows.ApplicationModel.DataTransfer.ShareTarget.ShareOperation ShareOperation { get; set; }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ((ShareTargetsViewModel)DataContext).TargetSelected(e.OriginalSource, this.ShareOperation);
        }

        internal void LoadShareOperation(Windows.ApplicationModel.DataTransfer.ShareTarget.ShareOperation shareOperation)
        {
            this.ShareOperation = shareOperation;
        }
    }
}
