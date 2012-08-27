using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using BeCharming.Common.ListenerService;
using BeCharming.Metro.Models;
using BeCharming.Metro.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BeCharming.Metro.Common;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BeCharming.Metro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Main : LayoutAwarePage
    {
        public Main()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel(Dispatcher);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((MainViewModel)DataContext).PerformPeerDiscovery();
        }

        internal bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());

            if (!unsnapped)
            {
                //NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage);
            }

            return unsnapped;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShareTarget target = e.ClickedItem as ShareTarget;
            Share(target);
        }

        private void TargetList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShareTarget target = e.ClickedItem as ShareTarget;
            Share(target);
        }

        private async void Share(ShareTarget target)
        {
            if (this.EnsureUnsnapped())
            {
                var filePicker = new FileOpenPicker();
                filePicker.ViewMode = PickerViewMode.Thumbnail;
                filePicker.CommitButtonText = "Share File";
                filePicker.SettingsIdentifier = "becharming";
                filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                filePicker.FileTypeFilter.Add("*");

                var selectedFile = await filePicker.PickSingleFileAsync();

                if (selectedFile != null)
                {
                    var properties = await selectedFile.GetBasicPropertiesAsync();
                    var size = properties.Size;
                    var fileStream = await selectedFile.OpenReadAsync();

                    using (DataReader dataReader = new DataReader(fileStream))
                    {
                        await dataReader.LoadAsync((uint)size);
                        byte[] buffer = new byte[(int)size];

                        dataReader.ReadBytes(buffer);

                        var fileBytes = buffer;

                        ShareRequest request = new ShareRequest();
                        request.FileContents = fileBytes;
                        request.FileName = selectedFile.Name;
                        request.Target = target;

                        ((MainViewModel)DataContext).Share(request);
                    }
                }
            }
        }

        //public void NotifyUser(string strMessage, NotifyType type)
        //{
        //    switch (type)
        //    {
        //        // Use the status message style.
        //        case NotifyType.StatusMessage:
        //            StatusBlock.Style = Resources["StatusStyle"] as Style;
        //            break;
        //        // Use the error message style.
        //        case NotifyType.ErrorMessage:
        //            StatusBlock.Style = Resources["ErrorStyle"] as Style;
        //            break;
        //    }

        //    StatusBlock.Text = strMessage;

        //    // Collapse the StatusBlock if it has no text to conserve real estate.
        //    if (StatusBlock.Text != String.Empty)
        //    {
        //        StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //    }
        //    else
        //    {
        //        StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //    }
        //}
    }
}
