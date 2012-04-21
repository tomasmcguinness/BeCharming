using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using BeCharming.Common.ListenerService;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BeCharming.Metro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Main : Page
    {
        DatagramSocket socket = new DatagramSocket();

        public Main()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel(Dispatcher);
            BottomAppBar.Opened += BottomAppBar_Opened;
            SettingsPane.GetForCurrentView().CommandsRequested += new TypedEventHandler<SettingsPane, SettingsPaneCommandsRequestedEventArgs>(SettingsCommandsRequested);

            socket.MessageReceived += socket_MessageReceived;
        }

        void BottomAppBar_Opened(object sender, object e)
        {
            //CommandManager .InvalidateRequerySuggested();
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await socket.BindEndpointAsync(null, "22002");

            //socket.JoinMulticastGroup(new HostName("224.0.0.1"));
            //EndpointPair p = new EndpointPair(new HostName("127.0.0.1"), "22002", new HostName("127.0.0.1"), "22003");

            //await socket.ConnectAsync(new HostName("192.168.10.100"), "22002");

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

                try
                {
                    await client.OpenDocumentAsync("Test.pdf", fileBytes);
                    ((MainViewModel)DataContext).IncrementShareCount(target);
                }
                catch (EndpointNotFoundException)
                {
                    // TODO Handle this exception.
                }
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var outputStream = await socket.GetOutputStreamAsync(new HostName("230.0.0.1"), "22002");
            DataWriter wr = new DataWriter(outputStream);
            wr.WriteString("**TEST - This is a test of the emergency broadcast system **");
            await wr.FlushAsync();
            await wr.StoreAsync();
        }

        void socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {

        }
    }
}
