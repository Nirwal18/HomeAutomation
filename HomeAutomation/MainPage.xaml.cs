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
        public static MainPage current;
        public MainPage()
        {
            this.InitializeComponent();
            current = this;
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Frame_container.Navigate(typeof(Page1));
          //  throw new NotImplementedException();
        }


        private void hamburger_menu_click(object sender, RoutedEventArgs e)
        {
            slider.IsPaneOpen = !slider.IsPaneOpen;
           
        }


        

       


        /// <summary>
        /// it is used to set property to bar.
        /// this function is used to set text and color on Status bar. Example : 
        /// status =barStatus.Error    red, 
        /// status =barStatus.warning  Yellow, 
        /// status =barStatus.sucess   Green 
        /// </summary>
        public void StatusBar(string message,barStatus status)
        {
            switch (status)
            {
                case barStatus.Error:
                    StatusBoder.Background = new SolidColorBrush(Windows.UI.Colors.DarkRed);
                    break;
                case barStatus.Sucess:
                    StatusBoder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case barStatus.Warnning:
                    StatusBoder.Background = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    break;
            }

            StatusBar_Text.Text = message;
               
        }




        private void page2_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame_container.Navigate(typeof(Page2));
           // StatusBar("device Added Event tiggred", barStatus.Sucess);

        }
    }

    /// <summary>
    /// To show Status bar warnning, sucess and Error state. Used in function StatusBar();
    /// </summary>
    public enum barStatus { Error, Warnning, Sucess, Normal };

}

