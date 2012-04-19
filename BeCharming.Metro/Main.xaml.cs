using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeCharming.Common.ListenerService;
using BeCharming.Metro.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

            this.BottomAppBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

            SettingsPane.GetForCurrentView().CommandsRequested += new TypedEventHandler<SettingsPane, SettingsPaneCommandsRequestedEventArgs>(SettingsCommandsRequested);
        }

        private void SettingsCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand generalCommand = new SettingsCommand("generalSettings", "General", new UICommandInvokedHandler(onSettingsCommand));
            args.Request.ApplicationCommands.Add(generalCommand);
            SettingsCommand helpCommand = new SettingsCommand("helpPage", "Help", new UICommandInvokedHandler(onSettingsCommand));
            args.Request.ApplicationCommands.Add(helpCommand);
        }

        public void onSettingsCommand(IUICommand command)
        {
            SettingsCommand settingsCommand = (SettingsCommand)command;
            //rootPage.NotifyUser("You clicked the " + settingsCommand.Label + " settings command", NotifyType.StatusMessage);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShareTarget target = e.ClickedItem as ShareTarget;

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

                var servicePath = string.Format("net.tcp://{0}:22001/BeCharming", target.IPAddress);

                ListenerClient client = new ListenerClient();
                client.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(servicePath));

                await client.OpenDocumentAsync("Test.pdf", fileBytes);
            }
        }

        private void GridView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var selectedBorder = e.OriginalSource as Border;
            var selectedTarget = selectedBorder.DataContext as ShareTarget;

            selectedTarget.IsSelected = !selectedTarget.IsSelected;

            BottomAppBar.Visibility = selectedTarget.IsSelected ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
