using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeAutomation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page3 : Page
    {
        public static Page3 current;
        private MainPage rootPage = MainPage.current;
        public Page3()
        {
            this.InitializeComponent();
            current = this;

        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            var applicationData = Windows.Storage.ApplicationData.Current;

            var localSettings = applicationData.LocalSettings;
            localSettings.Values["AutoConnect"] = autoConnectOnStartup_checkbox.IsChecked;
            localSettings.Values["RememberLastDevice"] = rememberLastDevice_checkbox.IsChecked;
        }

        private void Read_btn_Click(object sender, RoutedEventArgs e)
        {
            var applicationData = Windows.Storage.ApplicationData.Current;

            var localSettings = applicationData.LocalSettings;
            label1.Text = localSettings.Values["Stext"].ToString();
        }

        private void SaveCmd_button_Click(object sender, RoutedEventArgs e)
        {
            var applicationData = Windows.Storage.ApplicationData.Current;
            var localSettings = applicationData.LocalSettings;
            if (cmdOn_radio.IsChecked == true && cmdOff_radio.IsChecked == false)
            {
                localSettings.Values["toggleSwicth1"] = textBox1.Text;
                localSettings.Values["toggleSwicth2"] = textBox2.Text;
                localSettings.Values["toggleSwicth3"] = textBox3.Text;
                localSettings.Values["toggleSwicth4"] = textBox4.Text;
                localSettings.Values["toggleSwicth5"] = textBox5.Text;
                localSettings.Values["toggleSwicth6"] = textBox6.Text;
                localSettings.Values["toggleSwicth7"] = textBox7.Text;
                localSettings.Values["toggleSwicth8"] = textBox8.Text;
                localSettings.Values["toggleSwicth9"] = textBox9.Text;
                localSettings.Values["toggleSwicth10"] = textBox10.Text;
                rootPage.StatusBar("Saving Done for On Cmd.", BarStatus.Normal);
            }
            else if (cmdOn_radio.IsChecked == false && cmdOff_radio.IsChecked == true)
            {
                localSettings.Values["toggleSwicth1_off"] = textBox1.Text;
                localSettings.Values["toggleSwicth2_off"] = textBox2.Text;
                localSettings.Values["toggleSwicth3_off"] = textBox3.Text;
                localSettings.Values["toggleSwicth4_off"] = textBox4.Text;
                localSettings.Values["toggleSwicth5_off"] = textBox5.Text;
                localSettings.Values["toggleSwicth6_off"] = textBox6.Text;
                localSettings.Values["toggleSwicth7_off"] = textBox7.Text;
                localSettings.Values["toggleSwicth8_off"] = textBox8.Text;
                localSettings.Values["toggleSwicth9_off"] = textBox9.Text;
                localSettings.Values["toggleSwicth10_off"] = textBox10.Text;
                rootPage.StatusBar("Saving Done for Off Cmd.", BarStatus.Normal);
            }
            else
            {
                rootPage.StatusBar("Please Select 'On' and 'Off' option.", BarStatus.Warnning);
            }



        }


        private void Retrive_button_Click(object sender, RoutedEventArgs e)
        {

            var applicationData = Windows.Storage.ApplicationData.Current;
            var localSettings = applicationData.LocalSettings;

            if (localSettings.Values["toggleSwicth1"] == null || localSettings.Values["toggleSwicth1_off"] == null)
            {
                rootPage.StatusBar("First Save On & OFF command then retrive.", BarStatus.Warnning);
                return;
            }

            if (cmdOn_radio.IsChecked == true && cmdOff_radio.IsChecked == false)
            {
                textBox1.Text = localSettings.Values["toggleSwicth1"].ToString();
                textBox2.Text = localSettings.Values["toggleSwicth2"].ToString();
                textBox3.Text = localSettings.Values["toggleSwicth3"].ToString();
                textBox4.Text = localSettings.Values["toggleSwicth4"].ToString();
                textBox5.Text = localSettings.Values["toggleSwicth5"].ToString();
                textBox6.Text = localSettings.Values["toggleSwicth6"].ToString();
                textBox7.Text = localSettings.Values["toggleSwicth7"].ToString();
                textBox8.Text = localSettings.Values["toggleSwicth8"].ToString();
                textBox9.Text = localSettings.Values["toggleSwicth9"].ToString();
                textBox10.Text = localSettings.Values["toggleSwicth10"].ToString();

                rootPage.StatusBar("Saving Done for On Cmd.", BarStatus.Normal);
            }
            else if (cmdOn_radio.IsChecked == false && cmdOff_radio.IsChecked == true)
            {
                textBox1.Text = localSettings.Values["toggleSwicth1_off"].ToString();
                textBox2.Text = localSettings.Values["toggleSwicth2_off"].ToString();
                textBox3.Text = localSettings.Values["toggleSwicth3_off"].ToString();
                textBox4.Text = localSettings.Values["toggleSwicth4_off"].ToString();
                textBox5.Text = localSettings.Values["toggleSwicth5_off"].ToString();
                textBox6.Text = localSettings.Values["toggleSwicth6_off"].ToString();
                textBox7.Text = localSettings.Values["toggleSwicth7_off"].ToString();
                textBox8.Text = localSettings.Values["toggleSwicth8_off"].ToString();
                textBox9.Text = localSettings.Values["toggleSwicth9_off"].ToString();
                textBox10.Text = localSettings.Values["toggleSwicth10_off"].ToString();

                rootPage.StatusBar("Saving Done for Off Cmd.", BarStatus.Normal);
            }
            else
            {
                rootPage.StatusBar("Please Select 'On' and 'Off' option.", BarStatus.Warnning);

            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Model.BtNotification b = new Model.BtNotification();
            b.tost();

        }
    }
}
