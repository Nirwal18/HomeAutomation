using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeAutomation.Model;
using HomeAutomation.Views;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace HomeAutomation
{
   public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

   

    public partial class MainPage: Page
    {
        public static string FEATURE_NAME = "Main page.";

        // add the line for list box for navigation in hamburger menu
        List<Scenario> scenarios = new List<Scenario> {

            new Scenario(){Title="\u296E Connect and Pair",ClassType=typeof(Page1)},
            new Scenario() { Title = "\u2339 Control",ClassType = typeof(Page2)},
            new Scenario() { Title ="\u233E Game Controller",ClassType = typeof(GameController)},
            new Scenario() { Title ="\u2699 Setting",ClassType = typeof(Page3)},
            new Scenario(){ Title="Folder", ClassType=typeof(FileExpo)}

        };
    }


    public class ScenarioBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Scenario s = value as Scenario;
            return s.Title;
        }
       

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }

}
