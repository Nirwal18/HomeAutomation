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
using Windows.UI.Popups;

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
            ToggleSwitch t = sender as ToggleSwitch;
           
            int x = Convert.ToInt32(t.Name.Substring(12));
                
            if (t.IsOn)
            {
                DeviceEventHandler.Current.Send_cmd(1,x,"ON");
            }
            else
            {
                DeviceEventHandler.Current.Send_cmd(1, x, "OFF");              
            }
           
            
        }

     
    }
}
