using System;
using Windows.Foundation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml;
using System.ServiceModel.Dispatcher;
using System.Net.Sockets;
using Windows.Networking.Sockets;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage.Streams;
using HomeAutomation.Model;
using System.Collections.ObjectModel;
using Windows.Devices.Bluetooth.Rfcomm;

namespace HomeAutomation.EventHandler
{
    class DeviceEventHandler
    {
        /// <summary>
        /// allows for singleton DeviceEventHandler 
        /// </summary>
        private static DeviceEventHandler deviceEventHandlor;

        /// <summary>
        /// Used to synchronise threads to avoid Multiple instantiation of DeviceEventHandler.
        /// </summary>
        private static Object singletonCreationLock = new object();

        private String deviceId;
        private DeviceWatcher deviceWatcher;

        private DeviceInformation deviceInformation;
        private DeviceAccessInformation deviceAccessInformation;
        private BluetoothDevice bluetoothDevice;

        private SuspendingEventHandler appSuspendEventHandler;
        private EventHandler<Object> appResumeEventHandler;

        private TypedEventHandler<DeviceEventHandler, DeviceInformation> deviceCloseCallback;
        private TypedEventHandler<DeviceEventHandler, DeviceInformation> deviceConnectedCallback;

        private TypedEventHandler<DeviceWatcher, DeviceInformation> deviceAddedEventHandler;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> deviceUpdateEventHandler;
        private TypedEventHandler<DeviceWatcher, Object> deviceEmulationCompleteEventHandler;
        private TypedEventHandler<DeviceWatcher, Object> deviceStoppedEventHandler;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> deviceRemovedEventHandler;
        private TypedEventHandler<DeviceAccessInformation, DeviceAccessChangedEventArgs> deviceAccessEventHandler;

        private Boolean watcherSuspended;
        private Boolean watcherStarted;
        

        private static StreamSocket _socket;
        private static DataReader _serialReader;
        private static DataWriter _serialWriter;

        /// <summary>
        /// pointer back to the main page to use resource.
        /// </summary>
        private MainPage rootPage = MainPage.current;
        private Page1 page1 = Page1.current;

        /// <summary>
        /// Enforce the Singleton Patter so that there is only one object Handling.
        /// </summary>
        public static DeviceEventHandler Current
        {
            get
            {
                if (deviceEventHandlor == null)
                {
                    lock (singletonCreationLock)
                    {
                        if (deviceEventHandlor == null)
                        {
                            CreateNewDeviceEventHandler();
                        }
                    }
                }
                return deviceEventHandlor;
            }
        }
        /// <summary>
        /// Create a new instance of DeviceEventHandler class/current class;
        /// </summary>
        public static void CreateNewDeviceEventHandler()
        {
            deviceEventHandlor = new DeviceEventHandler();
        }

        public TypedEventHandler<DeviceEventHandler, DeviceInformation> OnDeviceClose
        {
            get
            {
                return deviceCloseCallback;
            }
            set
            {
                deviceCloseCallback = value;
            }
        }

        public TypedEventHandler<DeviceEventHandler, DeviceInformation> OnDeviceConnected
        {
            get
            {
                return deviceConnectedCallback;
            }
            set
            {
                deviceConnectedCallback = value;
            }
        }

        public Boolean IsDeviceConnected
        {
            get
            {
                return (bluetoothDevice != null);
            }
        }

        public BluetoothDevice BluetoothDevice
        {
            get
            {
                return bluetoothDevice;
            }
        }
        
        public DataReader SerialReader
        {
            get
            {
                return _serialReader;
            }
        }

        public DataWriter SerialWriter
        {
            get
            {
                return _serialWriter;
            }
        }
        /// <summary>
        /// This DeviceInformation repersent which device is connected or which device will be reconnected
        /// When the device is plugged in again(if IsEnabledAutoReconnect is true)
        /// </summary>
        public DeviceInformation DeviceIndormation
        {
            get
            {
                return deviceInformation;
            }
        }

        /// <summary>
        /// Return Device AccessInformation for the device that is currently 
        /// connected using this DeviceEventHandler object.
        /// </summary>
        public DeviceAccessInformation DeviceAccessInformation
        {
            get
            {
                return deviceAccessInformation;
            }
        }

       


       
        /// <summary>
        /// Close the device, Stop the Device Watcher, Stops listening for app events, 
        /// reset object state to before a device was ever connected.
        /// </summary>
        public void CloseDevice()
        {
            if (IsDeviceConnected)
            {
                CloseCurrentlyConnectedDevice();
            }



            if (watcherStarted)
            {
                StopDeviceWatcher();
            }
            
            

            if (deviceAccessInformation != null)
            {
                UnregisterFromDeviceAccessStatusChange();
                deviceAccessInformation = null;
            }

            if (appSuspendEventHandler != null || appResumeEventHandler != null)
            {
                UnregisterFromAppEvents();
            }
            deviceInformation = null;

            bluetoothDevice = null;
            _socket.Dispose();
            _socket = null;
            deviceConnectedCallback = null;
            deviceCloseCallback = null;
           
        }

        /// <summary>
        /// Contructor for current class.
        /// </summary>
        private DeviceEventHandler()
        {
            watcherStarted = false;
            watcherSuspended = false;
        
            RegisterForDeviceWatcherEvents();
        }

        private async void CloseCurrentlyConnectedDevice()
        {
            if (bluetoothDevice != null)
            {
                // notify collback that we are about to close the device
                if (deviceCloseCallback != null)
                {
                    deviceCloseCallback(this, deviceInformation);
                }

                bluetoothDevice.Dispose();
                bluetoothDevice = null;

                String btdeviceId = deviceId;

                await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
                {

                    rootPage.StatusBar(btdeviceId + " is closed", BarStatus.Warnning);

                }));


            }
        }

        private void RegisterForAppEvents()
        {
            appSuspendEventHandler = new SuspendingEventHandler(DeviceEventHandler.Current.OnAppSuspension);
            appResumeEventHandler = new EventHandler<Object>(DeviceEventHandler.Current.OnAppResume);
            App.Current.Suspending += appSuspendEventHandler;
            App.Current.Resuming += appResumeEventHandler;
        }

        private void UnregisterFromAppEvents()
        {
            App.Current.Suspending -= appSuspendEventHandler;
            appSuspendEventHandler = null;

            App.Current.Resuming -= appResumeEventHandler;
            appResumeEventHandler = null;
        }

        private void RegisterForDeviceWatcherEvents()
        {
            // device requesting property
            string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            // watcher created for bluetooth device discovery
            deviceWatcher = DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                            requestedProperties,
                                                            DeviceInformationKind.AssociationEndpoint);

            deviceAddedEventHandler = new TypedEventHandler<DeviceWatcher, DeviceInformation>(this.DeviceWatcher_Added);
            deviceUpdateEventHandler = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(this.DeviceWatcher_Updated);
            deviceEmulationCompleteEventHandler = new TypedEventHandler<DeviceWatcher, Object>(this.DeviceWatcher_EnumerationCompleted);
            deviceStoppedEventHandler = new TypedEventHandler<DeviceWatcher, Object>(this.DeviceWatcher_Stopped);
            deviceRemovedEventHandler = new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(this.DeviceWatcher_Removed);

            deviceWatcher.Added += deviceAddedEventHandler;
            deviceWatcher.Updated += deviceUpdateEventHandler;
            deviceWatcher.EnumerationCompleted += deviceEmulationCompleteEventHandler;
            deviceWatcher.Stopped += deviceStoppedEventHandler;
            deviceWatcher.Removed += deviceRemovedEventHandler;
        }

        private void UnregisterFromDeviceWatcherEvents()
        {
            deviceWatcher.Added -= deviceAddedEventHandler;
            deviceWatcher.Removed -= deviceRemovedEventHandler;
            deviceWatcher.Updated -= deviceUpdateEventHandler;
            deviceWatcher.EnumerationCompleted -= deviceEmulationCompleteEventHandler;
            deviceWatcher.Stopped -= deviceStoppedEventHandler;

            deviceAddedEventHandler = null;
            deviceRemovedEventHandler = null;
            deviceUpdateEventHandler = null;
            deviceEmulationCompleteEventHandler = null;
            deviceStoppedEventHandler = null;
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {

                page1.ResultCollection.Add(new Btdevice(deviceInfo));
                rootPage.StatusBar("Searching for Bluetooth devices...", BarStatus.Sucess);
            });
        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            // for updating UI from non UI thread.
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                rootPage.StatusBar(String.Format("{0} device Found, Watcher {1}", page1.ResultCollection.Count, DeviceWatcherStatus.Aborted == sender.Status ? "Aborted" : "Stopped"),
                    BarStatus.Error);
                page1.ResultCollection.Clear();
            });
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            // for updating UI from non UI thread.
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {

                rootPage.StatusBar(String.Format("Enumeration completed. {0} device found. ", page1.ResultCollection.Count), BarStatus.Normal);
            });
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceUpdate)
        {
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {

                foreach (Btdevice disp in page1.ResultCollection)
                {
                    if (disp.Id == deviceUpdate.Id)
                    {
                        disp.Update(deviceUpdate);
                        break;
                    }
                }
            });
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceUpdate)
        {
            // for updating UI from non UI thread.
            await rootPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {

                foreach (Btdevice disp in page1.ResultCollection)
                {
                    if (disp.Id == deviceUpdate.Id)
                    {
                        page1.ResultCollection.Remove(disp);
                        break;
                    }
                }

                rootPage.StatusBar(
                        String.Format("{0} devices found.", page1.ResultCollection.Count),
                        BarStatus.Sucess);

            });


        }

        public void StartDeviceWatcher()
        {
            watcherStarted = true;

            if ((deviceWatcher.Status != DeviceWatcherStatus.Started)
                && (deviceWatcher.Status != DeviceWatcherStatus.EnumerationCompleted))
            {
                RegisterForDeviceWatcherEvents();
                deviceWatcher.Start();
            }
        }

        public void StopDeviceWatcher()
        {
            if ((deviceWatcher.Status == DeviceWatcherStatus.Started)
                || (deviceWatcher.Status == DeviceWatcherStatus.EnumerationCompleted))
            {
                UnregisterFromDeviceWatcherEvents();
                deviceWatcher.Stop();
            }
        }

        private void RegisterForDeviceAccessStatusChange()
        {
            // Enable the following registration ONLY if the Serial device under test is non-internal.
            //
            
            deviceAccessInformation = DeviceAccessInformation.CreateFromId(deviceId);
            deviceAccessEventHandler = new TypedEventHandler<DeviceAccessInformation, DeviceAccessChangedEventArgs>(this.OnDeviceAccessChanged);
            deviceAccessInformation.AccessChanged += deviceAccessEventHandler;
        }

        private void UnregisterFromDeviceAccessStatusChange()
        {
            deviceAccessInformation.AccessChanged -= deviceAccessEventHandler;
            deviceAccessEventHandler = null;
        }

        private void OnAppSuspension(Object sender, SuspendingEventArgs args)
        {
            if (watcherStarted)
            {
                watcherSuspended = true;
                StopDeviceWatcher();
            }
            else
            {
                watcherSuspended = false;
            }

            CloseCurrentlyConnectedDevice();
        }

        private void OnAppResume(Object sender, Object args)
        {
            if (watcherSuspended)
            {
                watcherSuspended = false;
                StartDeviceWatcher();
            }
        }

        private async void OnDeviceAccessChanged(DeviceAccessInformation sender, DeviceAccessChangedEventArgs eventArgs)
        {
            if ((eventArgs.Status == DeviceAccessStatus.DeniedBySystem)
                || (eventArgs.Status == DeviceAccessStatus.DeniedByUser))
            {
                CloseCurrentlyConnectedDevice();
            }
            else if ((eventArgs.Status == DeviceAccessStatus.Allowed)
                && (deviceInformation != null))
            {
                await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(async () =>
                {
                    if (deviceId != null) { await ConnectAsyncFromId(deviceId); }
                    
                   // await OpenDeviceAsync(deviceInformation, deviceSelector);
                }));
            }
        }

       
        /// <summary>
        /// This is a async task to connect to device and create _socket, _dataReader and _dataWriter object.
        /// </summary>
        /// <param name="BtdeviceId"></param>
        /// <returns> boolean</returns>
        public async Task<Boolean> ConnectAsyncFromId(string BtdeviceId)
        {
            deviceId = BtdeviceId;
            StopDeviceWatcher();

            bluetoothDevice = await BluetoothDevice.FromIdAsync(BtdeviceId);

            if (bluetoothDevice != null)
            {
                if (deviceConnectedCallback != null)
                {
                    deviceConnectedCallback(this, deviceInformation);
                }

                if (appSuspendEventHandler == null || appResumeEventHandler == null)
                {
                    RegisterForAppEvents();
                }

                if (deviceAccessEventHandler == null)
                {
                    RegisterForDeviceAccessStatusChange();
                }


                var _rfcomService = await bluetoothDevice.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort, BluetoothCacheMode.Uncached);
                if (_rfcomService.Services.Count <= 0)
                {
                    rootPage.StatusBar("Serial service not found on "+ bluetoothDevice.Name, BarStatus.Error);
                    return false;
                }

                var serialService = _rfcomService.Services[0];

                lock (this) { _socket = new StreamSocket(); } // for marking this as crtical section.

                try
                {
                    // socket initializesd
                    await _socket.ConnectAsync(serialService.ConnectionHostName, serialService.ConnectionServiceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                    rootPage.StatusBar("connection to device done sucessfully", BarStatus.Sucess);

                    // reader and writer assingment.
                    _serialWriter = new DataWriter(_socket.OutputStream);
                    _serialReader = new DataReader(_socket.InputStream);

                }
                catch(Exception e)
                {
                    rootPage.StatusBar(e.ToString(),BarStatus.Error);
                }

            }

            return (bluetoothDevice != null);
        }

        public async void Send_cmd(int mode, int pin, string cmd)
        {
            if (_serialWriter != null && _socket != null) 
            {
                _serialWriter.WriteInt32(mode);
                _serialWriter.WriteInt32(pin);
                _serialWriter.WriteString(cmd);
                await _serialWriter.StoreAsync();
            }
            else
            {
                rootPage.StatusBar("Error, Please Connect and Try again...",BarStatus.Error);
            } 
        }
    }
}
