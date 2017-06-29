using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HomeAutomation.Model;
using HomeAutomation.EventHandler;
using Windows.Devices.Enumeration;
using System.Collections.ObjectModel;
using Windows.Devices.SerialCommunication;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeAutomation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page1 : Page
    {
        private MainPage rootPage = MainPage.current;

        public static Page1 current;

        //  private GattDeviceService service;

        //   public IAsyncOperation<DevicePairingResult> ParingResult;
        public ObservableCollection<Btdevice> ResultCollection
        {
            get;
            private set;
        }

        public Page1()
        {
            this.InitializeComponent();
            ResultCollection = new ObservableCollection<Btdevice>();
            current = this;
            this.Loaded += Page1_Loaded;
            DeviceEventHandler.CreateNewDeviceEventHandler();
        }

        private void Page1_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceEventHandler.Current.BluetoothDevice != null) 
            {
                Connect_btn.IsEnabled = false;
                disconnect_btn.IsEnabled = true;
                disconnect_btn.Visibility = Visibility.Visible;
            }
            
        }

       

        private void Search_btn_Click(object sender, RoutedEventArgs e)
        {
            Search_btn.IsEnabled = false;
            ResultCollection.Clear();
            rootPage.StatusBar("Searching for bluetooth device", BarStatus.Sucess);
            //BtWatcherStart();     
            DeviceEventHandler.Current.StartDeviceWatcher();
            if (DeviceEventHandler.Current.BluetoothDevice == null) { Connect_btn.IsEnabled = true; }
            
        }


        private async void Connect_btn_Click(object sender, RoutedEventArgs e)
        {
            Connect_btn.IsEnabled = false;

            if (resultListView.SelectedItem == null)
            {
                rootPage.StatusBar("please selet an item to connect", BarStatus.Error);
                return;
            }
            disconnect_btn.Visibility = Visibility.Visible;
            // selecting bluetooth device
            Btdevice btSelectedDevice = resultListView.SelectedItem as Btdevice;

            // saving device in Local Setting For Later Use in Auto Reconnect
            var applicationData = Windows.Storage.ApplicationData.Current;
            var localSettings = applicationData.LocalSettings;
            localSettings.Values["LastDeviceId"] = btSelectedDevice.Id;

            try
            {
                if(await DeviceEventHandler.Current.ConnectAsyncFromId(btSelectedDevice.Id))
                {
                    rootPage.MainPage_Navigate_Frame(typeof(Page2));
                }
            }
            catch (Exception ex)
            {
                ContentDialog cDialog = new ContentDialog()
                {
                    Title = "Bluetooth",
                    Content = "Please enable Bluetooth first.",
                    IsPrimaryButtonEnabled = true,
                    PrimaryButtonText = "Exit",
                    

                };
                // if bluetooth is ni=ot enabled then msg popup
                if(await cDialog.ShowAsync() == ContentDialogResult.Primary) { cDialog.Hide(); }
                disconnect_btn.Visibility = Visibility.Collapsed;
                Connect_btn.IsEnabled = true;
            }

        }


        // serial port connect optional not woking with bluetooth.
        public async void Connect()
        {
            Btdevice btdevice = resultListView.SelectedItem as Btdevice;

            var serialPort = await SerialDevice.FromIdAsync(btdevice.Id);

            serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.BaudRate = 9600;
            serialPort.Parity = SerialParity.None;
            serialPort.StopBits = SerialStopBitCount.One;
            serialPort.DataBits = 8;
        }








        private async void BtPair()
        {
            rootPage.StatusBar("Paring Started Please Wait...", BarStatus.Warnning);

            Btdevice deviceInfoDisp = resultListView.SelectedItem as Btdevice;
            DevicePairingResult dpr = await deviceInfoDisp.DeviceInformation.Pairing.PairAsync();

            rootPage.StatusBar("Paring Result" + dpr.Status.ToString(), dpr.Status == DevicePairingResultStatus.Paired ? BarStatus.Sucess : BarStatus.Error);
        }
        private async void BtUnpair()
        {
            rootPage.StatusBar("Paring Started Please Wait...", BarStatus.Warnning);

            Btdevice deviceInfoDisp = resultListView.SelectedItem as Btdevice;
            DeviceUnpairingResult dupr = await deviceInfoDisp.DeviceInformation.Pairing.UnpairAsync();

            rootPage.StatusBar("Unparing Result" + dupr.Status.ToString(), dupr.Status == DeviceUnpairingResultStatus.Unpaired ? BarStatus.Sucess : BarStatus.Error);

        }

        private void PairUnpair_btn_Click(object sender, RoutedEventArgs e)
        {
            resultListView.IsEnabled = false;

            Btdevice deviceInfoDisp = resultListView.SelectedItem as Btdevice;
            if (deviceInfoDisp == null)
            {
                rootPage.StatusBar("Please Select the device", BarStatus.Error);
                resultListView.IsEnabled = true;
                return;
            }
            if (deviceInfoDisp.IsPared)
            {
                BtUnpair();
            }
            else
            {
                BtPair();
            }
            resultListView.IsEnabled = true;
        }


       















        private void Disconnect_btn_Click(object sender, RoutedEventArgs e)
        {
            DeviceEventHandler.Current.CloseDevice();
            rootPage.StatusBar("Disconnect SucessFull", BarStatus.Normal);
            Search_btn.IsEnabled = true;
        }
    }


}
    

