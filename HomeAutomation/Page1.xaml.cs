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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeAutomation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page1 : Page
    {
        private MainPage rootPage = MainPage.current;
        private BluetoothDevice bluetoothDevice;
        private DeviceWatcher deviceWatcher = null;
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
            BtWatcher();     
        }


        private async void Connect_btn_Click(object sender, RoutedEventArgs e)
        {
            if (resultListView.SelectedItem != null)
            {
                rootPage.StatusBar("connecting to Bluetooth device. Please wait...", barStatus.Sucess);
            }
            else
            {
                rootPage.StatusBar("Please Select any item from list to connect.", barStatus.Warnning);
                return;
            }

            Btdevice btSelectedDevice = resultListView.SelectedItem as Btdevice;

            DeviceAccessStatus accessStatus = DeviceAccessInformation.CreateFromId(btSelectedDevice.Id).CurrentStatus;

            if (accessStatus == DeviceAccessStatus.DeniedByUser)
            {
                rootPage.StatusBar("This app does not have access to connect to the remote device (please grant access in Settings > Privacy > Other Devices", barStatus.Error);
                return;
            }

            try
            {
                bluetoothDevice = await BluetoothDevice.FromIdAsync(btSelectedDevice.Id);
            }
            catch( Exception ex)
            {
                rootPage.StatusBar(ex.Message, barStatus.Error);
                    return;
            }


            if (bluetoothDevice == null)
            {
                rootPage.StatusBar("Bluetooth Device returned null. Access Status = "+ accessStatus.ToString() , barStatus.Error);
            }


        }





        public void BtWatcher()
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

    }

}
