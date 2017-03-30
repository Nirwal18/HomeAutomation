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
using HomeAutomation.Model; //for accessing class in model folder
using HomeAutomation.EventHandler; //for accessing class in EventHandler folder

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeAutomation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page2 : Page
    {
        public static Page2 current;
        private MainPage rootPage = MainPage.current;
        public Page1 page1 = Page1.current;         // for Accessing Page1 Resource

        public Page2()
        {
            this.InitializeComponent();
            current = this;


           
            
        }

        private void ToggleSwitch1_Toggled(object sender, RoutedEventArgs e)
        {
            
            if(this.toggleSwicth1.IsOn)
            {
                page1.Send_cmd(1,3,"ON;");
            }
            else
            {
                page1.Send_cmd(1,3,"OFF;");
            }
           
            
        }

        private void ToggleSwitch2_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.toggleSwicth2.IsOn)
            {
                page1.Send_cmd(1, 4, "ON;");
            }
            else
            {
                page1.Send_cmd(1, 4, "OFF;");
            }

        }
        private void ToggleSwitch3_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.toggleSwicth3.IsOn)
            {
                page1.Send_cmd(1, 5, "ON;");
            }
            else
            {
                page1.Send_cmd(1, 5, "OFF;");
            }

        }
        private void ToggleSwitch4_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.toggleSwicth4.IsOn)
            {
                page1.Send_cmd(1, 6, "ON;");
            }
            else
            {
                page1.Send_cmd(1, 6, "OFF;");
            }

        }
        private void ToggleSwitch5_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.toggleSwicth5.IsOn)
            {
                page1.Send_cmd(1, 7, "ON;");
            }
            else
            {
                page1.Send_cmd(1, 7, "OFF;");
            }

        }
        private void ToggleSwitch6_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.toggleSwicth6.IsOn)
            {
                page1.Send_cmd(1, 8, "ON;");
            }
            else
            {
                page1.Send_cmd(1, 8, "OFF;");
            }

        }
        private void ToggleSwitch7_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.toggleSwicth7.IsOn)
            {
                page1.Send_cmd(1, 9, "ON;");
            }
            else
            {
                page1.Send_cmd(1, 9, "OFF;");
            }

        }
        private void ToggleSwitch8_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.toggleSwicth8.IsOn)
            {
                page1.Send_cmd(1, 10, "ON;");
            }
            else
            {
                page1.Send_cmd(1, 10, "OFF;");
            }

        }
        private void ToggleSwitch9_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.toggleSwicth9.IsOn)
            {
                page1.Send_cmd(1, 11, "ON;");
            }
            else
            {
                page1.Send_cmd(1, 11, "OFF;");
            }

        }
        private void ToggleSwitch10_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.toggleSwicth10.IsOn)
            {
                page1.Send_cmd(1, 12, "ON;");
            }
            else
            {
                page1.Send_cmd(1, 12, "OFF;");
            }

        }
    }
}
