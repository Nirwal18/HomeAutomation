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
        private List<Btdevice> device;
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

            BtWatcher();

          /*  foreach (var deviceInfo in deviceList)
            {
                //listBox.Items.Add(deviceInfo.Id);
                //listBox.Items.Add(deviceInfo.Name);
                //listBox.Items.Add("----------");
                resultListView.Items.Add(deviceInfo);
            }
               */       
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

                rootPage.StatusBar("device Added Event tiggred", barStatus.Sucess);
                ResultCollection.Add(new Btdevice(deviceInfo));
            });

           
        }


        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // throw new NotImplementedException();
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            //throw new NotImplementedException();
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            throw new NotImplementedException();
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            throw new NotImplementedException();
        }

    }

}
