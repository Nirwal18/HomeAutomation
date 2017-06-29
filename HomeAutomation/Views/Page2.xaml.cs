using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            ToggleSwitch t = (ToggleSwitch)sender;
            var applicationData = Windows.Storage.ApplicationData.Current;

            var localSettings = applicationData.LocalSettings;
 
            if (t.IsOn)
            {
                DeviceEventHandler.Current.Send_cmd(localSettings.Values[t.Name].ToString());
            }
            else
            {
                DeviceEventHandler.Current.Send_cmd(localSettings.Values[t.Name + "_off"].ToString());              
            }
           
            
        }




     
    }
}
