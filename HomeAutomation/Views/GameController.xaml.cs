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
using HomeAutomation.EventHandler;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HomeAutomation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameController : Page
    {
        DeviceEventHandler btport = DeviceEventHandler.Current;

        public GameController()
        {
            this.InitializeComponent();
           
            
        }

        private void Ellipse_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            circle_transform.TranslateX += e.Delta.Translation.X;
            circle_transform.TranslateY += e.Delta.Translation.Y;
           
            XposText.Text = e.Delta.Translation.X.ToString("#.##");
            YposText.Text = e.Delta.Translation.Y.ToString("#.##");
            btport.Send_cmd("X;"+XposText.Text + ";" + "Y;"+YposText.Text);
        }

        private void Ellipse_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            
        }

        private void Ellipse_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            circle_transform.TranslateX = 0;
            circle_transform.TranslateY = 0;
            XposText.Text = 0.ToString();
            YposText.Text =0.ToString();
        }
    }
}
