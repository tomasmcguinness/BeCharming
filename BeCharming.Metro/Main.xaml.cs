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
}
