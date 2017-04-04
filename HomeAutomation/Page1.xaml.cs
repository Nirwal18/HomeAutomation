using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HomeAutomation.Model;
using HomeAutomation.EventHandler;
using System.Diagnostics;
using Windows.Devices.AllJoyn;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using System.Collections.ObjectModel;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.SerialCommunication;
using System.Threading.Tasks;
using System.Threading;

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

           // DeviceEventHandler.CreateNewDeviceEventHandler();
        }

        private  void Search_btn_Click(object sender, RoutedEventArgs e)
        {
            Search_btn.IsEnabled = false;
            ResultCollection.Clear();
            rootPage.StatusBar("Searching for bluetooth device", BarStatus.Sucess);
            //BtWatcherStart();     
            DeviceEventHandler.Current.StartDeviceWatcher();
        }


        private async void Connect_btn_Click(object sender, RoutedEventArgs e)
        {
            if (resultListView.SelectedItem == null)
            {
                rootPage.StatusBar("please selet an item to connect", BarStatus.Error);
                return;
            }
            // selecting bluetooth device
            Btdevice btSelectedDevice = resultListView.SelectedItem as Btdevice;

            await DeviceEventHandler.Current.ConnectAsyncFromId(btSelectedDevice.Id);
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


        private void Send_btn_Click(object sender, RoutedEventArgs e)
        {
            
        }
 

        private void SetChatUI(string v, string name)
        {
            send_btn.IsEnabled = false;
            resultListView.Visibility = Visibility.Collapsed;
            conversionList.Visibility = Visibility.Visible;
        }

     

        

     



      




        private void Disconnect_btn_Click(object sender, RoutedEventArgs e)
        {
            DeviceEventHandler.Current.CloseDevice();
            rootPage.StatusBar("Disconnect SucessFull",BarStatus.Normal);
            Search_btn.IsEnabled = true;
        }
    }


    
    
}
