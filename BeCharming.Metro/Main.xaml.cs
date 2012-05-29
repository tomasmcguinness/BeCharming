using System;
using BeCharming.Metro.Models;
using BeCharming.Metro.ViewModels;
using Windows.Foundation;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
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
            this.DataContext = new MainViewModel(Dispatcher);
            BottomAppBar.Opened += BottomAppBar_Opened;
            SettingsPane.GetForCurrentView().CommandsRequested += new TypedEventHandler<SettingsPane, SettingsPaneCommandsRequestedEventArgs>(SettingsCommandsRequested);
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((MainViewModel)DataContext).PerformPeerDiscovery();
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShareTarget target = e.ClickedItem as ShareTarget;

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

                DataReader dataReader = new DataReader(fileStream);
                await dataReader.LoadAsync((uint)size);
                byte[] buffer = new byte[(int)size];

                dataReader.ReadBytes(buffer);

                var fileBytes = buffer;

                ShareRequest request = new ShareRequest();
                request.FileContents = fileBytes;
                request.FileName = selectedFile.Name;
                request.Target = target;

                ((MainViewModel)DataContext).StartShare(request);
            }
        }
    }
}
