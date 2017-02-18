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
using Windows.Devices.AllJoyn;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HomeAutomation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void hamburger_menu_click(object sender, RoutedEventArgs e)
        {
            slider.IsPaneOpen = !slider.IsPaneOpen;
        }
        /*    private async void button_Click(object sender, RoutedEventArgs e)
             {
                 string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
                 var deviceList = await DeviceInformation.FindAllAsync("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                                 requestedProperties,
                                                                 DeviceInformationKind.AssociationEndpoint);
                 foreach (var deviceInfo in deviceList)
                 {
                     listBox.Items.Add(deviceInfo.Id);
                     listBox.Items.Add(deviceInfo.Name);
                     listBox.Items.Add("----------");
                 }

             }*/
    }

}

