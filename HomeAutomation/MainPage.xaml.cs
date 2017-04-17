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
using HomeAutomation.EventHandler;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HomeAutomation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Page3 page3 = Page3.current;
        public static MainPage current;

        public ObservableCollection<Scenario> SResultCollection
        {
            get;
            private set;
        }

        public MainPage()
        {
            this.InitializeComponent();
            SResultCollection = new ObservableCollection<Scenario>();
            current = this;
            this.Loaded += MainPage_Loaded;
            

        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Frame_container.Navigate(typeof(Page1));
            StatusBar_Text.Text = string.Empty;

            // auto connect algorithm
            AutoReconnect();
            
           
        }

        private async void AutoReconnect()
        {
            // Retrive Setting From local Storage
            var applicationData = Windows.Storage.ApplicationData.Current;
            var localSettings = applicationData.LocalSettings;

            if (localSettings.Values["RememberLastDevice"] != null &&
                localSettings.Values["AutoConnect"] != null &&
            localSettings.Values["LastDeviceId"] != null)
            {
                bool autoConnect_setting = (Boolean)localSettings.Values["AutoConnect"];
                bool rememberLastDevice_setting = (Boolean)localSettings.Values["RememberLastDevice"];

                if (autoConnect_setting == true && rememberLastDevice_setting == true)
                {
                    if (await DeviceEventHandler.Current.ConnectAsyncFromId(localSettings.Values["LastDeviceId"].ToString()))
                    {
                        Frame_container.Navigate(typeof(Page2));
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // add page list for hamburger menu in scenario.cs file
            ScenarioControl.ItemsSource = scenarios;

            if (Window.Current.Bounds.Width < 640)
            {
                ScenarioControl.SelectedIndex = -1;
            }
            else
            {
                ScenarioControl.SelectedIndex = 0;
            }
        }




        /// <summary>
        /// for geeting list of scenario menu item in Main page. And accessable.
        /// </summary>
        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }


        public void MainPage_Navigate_Frame(Type page)
        {
            Frame_container.Navigate(page);
        }


        private void Hamburger_menu_click(object sender, RoutedEventArgs e)
        {
            slider.IsPaneOpen = !slider.IsPaneOpen;
           
        }


           


        /// <summary>
        /// it is used to set property to bar.
        /// this function is used to set text and color on Status bar. Example : 
        /// status =BarStatus.Error    red, 
        /// status =BarStatus.warning  Yellow, 
        /// status =BarStatus.sucess   Green 
        /// </summary>
        public void StatusBar(string message,BarStatus status)
        {
            switch (status)
            {
                case BarStatus.Error:
                    StatusBoder.Background = new SolidColorBrush(Windows.UI.Colors.DarkRed);
                    break;
                case BarStatus.Sucess:
                    StatusBoder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case BarStatus.Warnning:
                    StatusBoder.Background = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    break;
                case BarStatus.Normal:
                    StatusBoder.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(240,240,240,240));
                    break;
            }

            StatusBar_Text.Text = message;
               
        }


        

        /// <summary>
        /// This event is tiggered when you click on Hambarger menu list item.
        /// It update header text also.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StatusBar(String.Empty,BarStatus.Normal);
            ListBox scenarioListBox = sender as ListBox;
            Scenario s = scenarioListBox.SelectedItem as Scenario;
            if (s != null)
            {
                Frame_container.Navigate(s.ClassType);
                Header_text.Text = s.Title;
                if (Window.Current.Bounds.Width < 640)
                {
                    slider.IsPaneOpen = false;
                }
            }

        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame_container.Navigate(typeof(Page3));
        }

        private void slider_PaneClosed(SplitView sender, object args)
        {
            if (!slider.IsPaneOpen) { hamburgerMenu_btn.IsChecked = true; }
            else { hamburgerMenu_btn.IsChecked = false; }
        }
    }

    /// <summary>
    /// To show Status bar warnning, sucess and Error state. Used in function StatusBar();
    /// </summary>
    public enum BarStatus { Error, Warnning, Sucess, Normal };



   

}

