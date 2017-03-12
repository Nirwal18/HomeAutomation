using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HomeAutomation.Model;
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
        private BluetoothDevice bluetoothDevice = null;
        private SerialDevice serialPort = null;
        private DeviceWatcher deviceWatcher = null;
        private RfcommDeviceService chatService = null;
       
        private StreamSocket _socket;
        private DataReader serialReader;
        private DataWriter serialWriter;
        private CancellationTokenSource ReadCancellationTokenSource;

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
            // device = DeviceManager.GetDevice();
        }

        private  void Search_btn_Click(object sender, RoutedEventArgs e)
        {
            Search_btn.IsEnabled = false;
            ResultCollection.Clear();
            rootPage.StatusBar("Searching for bluetooth device", barStatus.Sucess);
            BtWatcherStart();     
        }


        private void Connect_btn_Click(object sender, RoutedEventArgs e)
        {
            ReadCancellationTokenSource = new CancellationTokenSource();

            //connect();
            connect2();
            
        }



     public async void connect()
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

   
       
        private  void rcvdText_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ...


            // ...	
        }

      



        public void BtWatcherStart()
        {
            // device requesting property
            string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            // watcher created for bluetooth device discovery
            deviceWatcher=DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                            requestedProperties,
                                                            DeviceInformationKind.AssociationEndpoint);
            // watcher event call initiallize
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;
            // now watcher start working
            deviceWatcher.Start();

        }

        public void BtWatcherStop()
        {
            deviceWatcher.Added -= DeviceWatcher_Added;
            deviceWatcher.Updated -= DeviceWatcher_Updated;
            deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Removed -= DeviceWatcher_Removed;
            deviceWatcher.Stopped -= DeviceWatcher_Stopped;

            if (DeviceWatcherStatus.Started == deviceWatcher.Status ||
                DeviceWatcherStatus.EnumerationCompleted == deviceWatcher.Status)
            {
                deviceWatcher.Stop();
            }
            // update ui and enable btn
            Search_btn.IsEnabled = true;
            rootPage.StatusBar("Watcher Stoped", barStatus.Warnning);
        }


        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // for updating UI from non UI thread.
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {

                ResultCollection.Add(new Btdevice(deviceInfo));
                rootPage.StatusBar("Searching for Bluetooth devices...", barStatus.Sucess);
            });

           
        }


        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceUpdate)
        {
            // for updating UI from non UI thread.
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {

                foreach (Btdevice disp in ResultCollection)
                {
                    if(disp.Id == deviceUpdate.Id)
                    {
                        disp.Update(deviceUpdate);
                        break;
                    }
                }
            });
           
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            // for updating UI from non UI thread.
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {

                rootPage.StatusBar(String.Format("Enumeration completed. {0} device found. ", ResultCollection.Count), barStatus.Normal);
            });
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceUpdate)
        {
            // for updating UI from non UI thread.
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {

                foreach (Btdevice disp in ResultCollection)
                {
                    if (disp.Id == deviceUpdate.Id) 
                    {
                        ResultCollection.Remove(disp);
                        break;
                    }
                }

                rootPage.StatusBar(
                        String.Format("{0} devices found.", ResultCollection.Count),
                        barStatus.Sucess);

            });
           
        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            // for updating UI from non UI thread.
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                rootPage.StatusBar(String.Format("{0} device Found, Watcher {1}", ResultCollection.Count, DeviceWatcherStatus.Aborted == sender.Status ? "Aborted" : "Stopped"),
                    barStatus.Error);
                ResultCollection.Clear();
            });
          
        }

       
        private async void BtPair()
        {                       
            rootPage.StatusBar("Paring Started Please Wait...", barStatus.Warnning);

            Btdevice deviceInfoDisp = resultListView.SelectedItem as Btdevice;
            DevicePairingResult dpr = await deviceInfoDisp.DeviceInformation.Pairing.PairAsync();

            rootPage.StatusBar("Paring Result" + dpr.Status.ToString(), dpr.Status == DevicePairingResultStatus.Paired ? barStatus.Sucess : barStatus.Error);
        }
        private async void BtUnpair()
        {
            rootPage.StatusBar("Paring Started Please Wait...", barStatus.Warnning);

            Btdevice deviceInfoDisp = resultListView.SelectedItem as Btdevice;
            DeviceUnpairingResult dupr = await deviceInfoDisp.DeviceInformation.Pairing.UnpairAsync();

            rootPage.StatusBar("Unparing Result" + dupr.Status.ToString(), dupr.Status == DeviceUnpairingResultStatus.Unpaired ? barStatus.Sucess : barStatus.Error);
                        
        }

        private void PairUnpair_btn_Click(object sender, RoutedEventArgs e)
        {
            resultListView.IsEnabled = false;
           
            Btdevice deviceInfoDisp = resultListView.SelectedItem as Btdevice;
            if (deviceInfoDisp == null)
            {
                rootPage.StatusBar("Please Select the device", barStatus.Error);
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

        private void send_btn_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }
        /*
        public async void connect()
        {

            // Make sure user has selected a device first
            if (resultListView.SelectedItem != null)
            {
                rootPage.StatusBar("Connecting to remote device. Please wait...", barStatus.Sucess);
            }
            else
            {
                rootPage.StatusBar("Please select an item to connect to", barStatus.Error);
                return;
            }

            Btdevice deviceInfoDisp = resultListView.SelectedItem as Btdevice;

            // Perform device access checks before trying to get the device.
            // First, we check if consent has been explicitly denied by the user.
            DeviceAccessStatus accessStatus = DeviceAccessInformation.CreateFromId(deviceInfoDisp.Id).CurrentStatus;
            if (accessStatus == DeviceAccessStatus.DeniedByUser)
            {
                rootPage.StatusBar("This app does not have access to connect to the remote device (please grant access in Settings > Privacy > Other Devices", barStatus.Error);
                return;
            }

            // If not, try to get the Bluetooth device
            try
            {
                bluetoothDevice = await BluetoothDevice.FromIdAsync(deviceInfoDisp.Id);
            }
            catch (Exception ex)
            {
                rootPage.StatusBar(ex.Message, barStatus.Error);
                return;
            }

            // If we were unable to get a valid Bluetooth device object,
            // it's most likely because the user has specified that all unpaired devices
            // should not be interacted with.
            if (bluetoothDevice == null)
            {
                rootPage.StatusBar("Bluetooth Device returned null. Access Status = " + accessStatus.ToString(), barStatus.Error);
            }

            // This should return a list of uncached Bluetooth services (so if the server was not active when paired, it will still be detected by this call
            var rfcommServices = await bluetoothDevice.GetRfcommServicesAsync(BluetoothCacheMode.Uncached);

            if (rfcommServices.Services.Count > 0)
            {
                chatService = rfcommServices.Services[0];
            }
            else
            {
                rootPage.StatusBar(
                   "Could not discover the chat service on the remote device",
                   barStatus.Error);
                return;
            }

            // Do various checks of the SDP record to make sure you are talking to a device that actually supports the Bluetooth Rfcomm Chat Service
            var attributes = await chatService.GetSdpRawAttributesAsync();
            if (!attributes.ContainsKey(Constants.SdpServiceNameAttributeId))
            {
                rootPage.StatusBar(
                    "The Chat service is not advertising the Service Name attribute (attribute id=0x100). " +
                    "Please verify that you are running the BluetoothRfcommChat server.",
                    barStatus.Error);
                Search_btn.IsEnabled = true;
                return;
            }

            var attributeReader = DataReader.FromBuffer(attributes[Constants.SdpServiceNameAttributeId]);
            var attributeType = attributeReader.ReadByte();
            if (attributeType != Constants.SdpServiceNameAttributeType)
            {
                rootPage.StatusBar(
                    "The Chat service is using an unexpected format for the Service Name attribute. " +
                    "Please verify that you are running the BluetoothRfcommChat server.",
                    barStatus.Error);
                Search_btn.IsEnabled = true;
                return;
            }

            var serviceNameLength = attributeReader.ReadByte();

            // The Service Name attribute requires UTF-8 encoding.
            attributeReader.UnicodeEncoding = UnicodeEncoding.Utf8;

            deviceWatcher.Stop();

            lock (this)
            {
                chatSocket = new StreamSocket();
            }
            try
            {
                await chatSocket.ConnectAsync(chatService.ConnectionHostName, chatService.ConnectionServiceName);

                SetChatUI(attributeReader.ReadString(serviceNameLength), bluetoothDevice.Name);
                chatWriter = new DataWriter(chatSocket.OutputStream);

                DataReader chatReader = new DataReader(chatSocket.InputStream);
                ResetUI();
              //  ReceiveStringLoop(chatReader);
            }
            catch (Exception ex)
            {
                switch ((uint)ex.HResult)
                {
                    case (0x80070490): // ERROR_ELEMENT_NOT_FOUND
                        rootPage.StatusBar("Please verify that you are running the BluetoothRfcommChat server.", barStatus.Error);
                        Search_btn.IsEnabled = true;
                        break;
                    default:
                        throw;
                }
            }
        }
        */
        private void SetChatUI(string v, string name)
        {
            send_btn.IsEnabled = false;
            resultListView.Visibility = Visibility.Collapsed;
            conversionList.Visibility = Visibility.Visible;
        }

        private async void ReceiveStringLoop(DataReader chatReader)
        {
          try
            {
                uint size = await chatReader.LoadAsync(sizeof(uint));
                if (size < sizeof(uint))
                {
                    Disconnect("Remote device terminated connection - make sure only one instance of server is running on remote device");
                    return;
                }

                
                uint stringLength = chatReader.ReadUInt32();
               uint actualStringLength = await chatReader.LoadAsync(stringLength);

                if (actualStringLength != stringLength)
                {
                    // The underlying socket was closed before we were able to read the whole data
                    return;
                }

                conversionList.Items.Add("Received: " + chatReader.ReadString(8));

               ReceiveStringLoop(chatReader);
            }
            catch (Exception ex)
            {
                lock (this)
                {
                    if (_socket == null)
                    {
                        // Do not print anything here -  the user closed the socket.
                        // HResult = 0x80072745 - catch this (remote device disconnect) ex = {"An established connection was aborted by the software in your host machine. (Exception from HRESULT: 0x80072745)"}
                    }
                    else
                    {
                        Disconnect("Read stream failed with error: " + ex.Message);
                    }
                }
            }
        }

        private async void SendMessage()
        {
            try
            {
                if (message_box.Text.Length != 0)
                {
                    uint inputelement = serialWriter.MeasureString(message_box.Text);
                    serialWriter.WriteUInt32(inputelement);
                    serialWriter.WriteString(message_box.Text);

                    conversionList.Items.Add("Sent: " + message_box.Text);
                    message_box.Text = "";
                    await serialWriter.StoreAsync();
                     

                }
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80072745)
            {
                // The remote device has disconnected the connection
                rootPage.StatusBar("Remote side disconnect: " + ex.HResult.ToString() + " - " + ex.Message,
                    barStatus.Warnning);
            }
        }

        private void Disconnect(string disconnectReason)
        {
            //throw new NotImplementedException();
            if (serialWriter != null)
            {
              serialWriter.DetachStream();
                serialWriter = null;
            }


            if (chatService != null)
            {
                chatService.Dispose();
                chatService = null;
            }
            lock (this)
            {
                if (_socket != null)
                {
                    _socket.Dispose();
                    _socket = null;
                }
            }

            rootPage.StatusBar(disconnectReason, barStatus.Warnning);
            ResetUI();
        }

        private void ResetUI()
        {
            //throw new NotImplementedException();
            send_btn.IsEnabled = true;
            Search_btn.IsEnabled = true;
            resultListView.Visibility = Visibility.Visible;
            conversionList.Visibility = Visibility.Visible;
        }


        public async void connect2()
        {
           // checking if device is selected or not.
            if (resultListView.SelectedItem == null)
            {
                rootPage.StatusBar("please selet an item to connect", barStatus.Error);
                return;
            }
            // selecting bluetooth device
            Btdevice btSelectedDevice = resultListView.SelectedItem as Btdevice;

            //initialize bluetooth device
            var btDevice = await BluetoothDevice.FromIdAsync(btSelectedDevice.Id);

            // checking device initiallized or not.
            if (btDevice == null)
            {
                rootPage.StatusBar("Connection device error", barStatus.Error);
                return;
            }
            // getting service result from device regarding to serial port.
            var _rfcomService = await btDevice.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort,BluetoothCacheMode.Uncached);
                if (_rfcomService.Services.Count == 0)
                {
                    rootPage.StatusBar("Serial service not found on device.", barStatus.Error);
                    return;
                }
                

            message_box.Text = _rfcomService.Services.Count().ToString();
            // chat service storing from result arrey
            chatService = _rfcomService.Services[0];

            BtWatcherStop();

            
            // socket assingment
          lock(this) { _socket = new StreamSocket(); } // for marking this as crtical section.

            message_box.Text = chatService.ServiceId.ToString();

            try
            {
                // socket initializesd
                await _socket.ConnectAsync(chatService.ConnectionHostName, chatService.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                rootPage.StatusBar("connection to device done sucessfully", barStatus.Sucess);


                SetChatUI(chatService.ServiceId.ToString(), btDevice.Name);

                // reader and writer assingment.
                 serialWriter = new DataWriter(_socket.OutputStream);
                DataReader serialReader = new DataReader(_socket.InputStream);

                ResetUI();





                byte b;

                while (true)
                {
                   await serialReader.LoadAsync(sizeof(uint));


                    b = serialReader.ReadByte();
                       conversionList.Items.Add("Sent: " +  b );
                   
                    conversionList.Items.Add("Sent: " + Convert.ToChar(b));

                    rootPage.StatusBar("bytes read successfully!", barStatus.Warnning);

                    
                }






                


               //ReceiveStringLoop(serialReader);
            }
            catch(Exception ex)
            { rootPage.StatusBar("Error : "+ex.ToString(), barStatus.Error); }
        }

        
    }


    
    
}
