﻿using System;
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
               
        private DeviceWatcher deviceWatcher = null;
        private RfcommDeviceService chatService = null;
       
        private StreamSocket _socket;
        private DataReader serialReader;
        private DataWriter serialWriter;
        
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

            DeviceEventHandler.CreateNewDeviceEventHandler();
        }

        private  void Search_btn_Click(object sender, RoutedEventArgs e)
        {
            Search_btn.IsEnabled = false;
            ResultCollection.Clear();
            rootPage.StatusBar("Searching for bluetooth device", BarStatus.Sucess);
            //BtWatcherStart();     
            DeviceEventHandler.Current.StartDeviceWatcher();
        }


        private void Connect_btn_Click(object sender, RoutedEventArgs e)
        {        
            Connect2(); 
        }



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
            rootPage.StatusBar("Watcher Stoped", BarStatus.Warnning);
        }


        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // for updating UI from non UI thread.
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {

                ResultCollection.Add(new Btdevice(deviceInfo));
                rootPage.StatusBar("Searching for Bluetooth devices...", BarStatus.Sucess);
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

                rootPage.StatusBar(String.Format("Enumeration completed. {0} device found. ", ResultCollection.Count), BarStatus.Normal);
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
                        BarStatus.Sucess);

            });
           
        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            // for updating UI from non UI thread.
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                rootPage.StatusBar(String.Format("{0} device Found, Watcher {1}", ResultCollection.Count, DeviceWatcherStatus.Aborted == sender.Status ? "Aborted" : "Stopped"),
                    BarStatus.Error);
                ResultCollection.Clear();
            });
          
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
            SendMessage();
        }
 

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

                
                uint stringLength = serialReader.ReadUInt32();
               uint actualStringLength = await serialReader.LoadAsync(stringLength);

                if (actualStringLength != stringLength)
                {
                    // The underlying socket was closed before we were able to read the whole data
                    return;
                }

                conversionList.Items.Add("Received: " + serialReader.ReadString(8));

               ReceiveStringLoop(serialReader);
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
                    BarStatus.Warnning);
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

            rootPage.StatusBar(disconnectReason, BarStatus.Warnning);
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


        public async void Connect2()
        {
           // checking if device is selected or not.
            if (resultListView.SelectedItem == null)
            {
                rootPage.StatusBar("please selet an item to connect", BarStatus.Error);
                return;
            }
            // selecting bluetooth device
            Btdevice btSelectedDevice = resultListView.SelectedItem as Btdevice;

            await DeviceEventHandler.Current.ConnectAsyncFromId(btSelectedDevice.Id);
            return;
            //initialize bluetooth device
            var btDevice = await BluetoothDevice.FromIdAsync(btSelectedDevice.Id);

            // checking device initiallized or not.
            if (btDevice == null)
            {
                rootPage.StatusBar("Connection device error", BarStatus.Error);
                return;
            }
            // getting service result from device regarding to serial port.
            var _rfcomService = await btDevice.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort,BluetoothCacheMode.Uncached);
                if (_rfcomService.Services.Count == 0)
                {
                    rootPage.StatusBar("Serial service not found on device.", BarStatus.Error);
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
                rootPage.StatusBar("connection to device done sucessfully", BarStatus.Sucess);


                SetChatUI(chatService.ServiceId.ToString(), btDevice.Name);

                // reader and writer assingment.
                serialWriter = new DataWriter(_socket.OutputStream);
                serialReader = new DataReader(_socket.InputStream);

                ResetUI();





               /* byte b;//size of buffer protocol
                string result;
                while (true)
                {
                   await serialReader.LoadAsync(sizeof(uint));


                    b = serialReader.ReadByte();
                    result=serialReader.ReadString(Convert.ToUInt32(b));
                    conversionList.Items.Add("Sent: " +  b );
                   
                    conversionList.Items.Add("Sent: " + result);

                    rootPage.StatusBar("bytes read successfully!", BarStatus.Warnning);

                    
                }*/






                


               //ReceiveStringLoop(serialReader);
            }
            catch(Exception ex)
            { rootPage.StatusBar("Error : "+ex.ToString(), BarStatus.Error); }
        }


        // under development function
        public string StatusRecieved()
        {
            uint size;
            Task t1 = new Task(async () => {
                size = await serialReader.LoadAsync(sizeof(uint));
            });

            t1.Start();
            t1.Wait();

            byte b = serialReader.ReadByte();
            uint c = Convert.ToUInt32(b);
            return serialReader.ReadString(c);
            

        }


        public async void Send_cmd(int mode, int pin,string cmd)
        {
            DeviceEventHandler.Current.SerialWriter.WriteInt32(mode);
            DeviceEventHandler.Current.SerialWriter.WriteInt32(pin);
            DeviceEventHandler.Current.SerialWriter.WriteString(cmd);
            await DeviceEventHandler.Current.SerialWriter.StoreAsync();
            //serialWriter.WriteInt32(mode);
            //serialWriter.WriteInt32(pin);
            //serialWriter.WriteString(cmd);
            //await serialWriter.StoreAsync();
            
                      
        }

        private void Disconnect_btn_Click(object sender, RoutedEventArgs e)
        {
            if(_socket != null) _socket.Dispose(); 
            if (serialReader != null) serialReader.Dispose();
            if (serialWriter != null) serialWriter.Dispose();
            rootPage.StatusBar("Socket closed by User",BarStatus.Warnning);
        }
    }


    
    
}
