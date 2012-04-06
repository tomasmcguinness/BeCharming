using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeCharming.Common.ListenerService;
using BeCharming.Metro.ViewModels;
//using BeCharming.Metro.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
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
    public sealed partial class Main : Page
    {
        public Main()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
            ((MainViewModel)this.DataContext).TargetIPAddress = "127.0.0.1";
            ((MainViewModel)this.DataContext).TargetName = "Localhost";
            ((MainViewModel)this.DataContext).AddTargetExecute(null);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void GridView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".pdf");
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.SettingsIdentifier = "picker1";
            filePicker.CommitButtonText = "Open File to Send";

            var item = await filePicker.PickSingleFileAsync();

            if (item != null)
            {
                var properties = await item.GetBasicPropertiesAsync();
                var size = properties.Size;
                var fileStream = await item.OpenReadAsync();

                DataReader dataReader = new DataReader(fileStream);
                await dataReader.LoadAsync((uint)size);
                byte[] buffer = new byte[(int)size];

                dataReader.ReadBytes(buffer);

                var fileBytes = buffer;

                ListenerClient client = new ListenerClient();
                client.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri("http://192.168.1.12:22001/BeCharming"));

                await client.OpenDocumentAsync("Test.pdf", fileBytes);
            }
        }
    }
}
