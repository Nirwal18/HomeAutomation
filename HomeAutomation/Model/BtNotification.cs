using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Microsoft.Toolkit.Uwp.Notifications;

namespace HomeAutomation.Model
{
    public class BtNotification
    {
        XmlDocument xmlDoc = new XmlDocument();
        StringBuilder template1 = new StringBuilder();
        void Toast(string text)
        {
            XmlDocument xmlDoc = new XmlDocument();
            StringBuilder template1 = new StringBuilder();
            template1.Append("<toast><visual version='2'><binding template='ToastText01'>");
            template1.AppendFormat("<text id='1'>{0}</text>", text);
            template1.Append("</binding></visual></toast>");

            xmlDoc.LoadXml(template1.ToString());
            ToastNotification notfn = new ToastNotification(xmlDoc);
            ToastNotificationManager.CreateToastNotifier().Show(notfn);
        }

       

       



        public void Toast(string title, string content,string imgUri)
        {

            // template for the toast
            ToastContent toastContent = new ToastContent()
            {
                Launch = "app-defined-string",

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            // text element for toast
                                    new AdaptiveText()
                                    {
                                    Text = title,
                                    HintMaxLines = 1
                                    },

                                    new AdaptiveText()
                                    {
                                    Text = content
                                    },
                                    //image for toast
                                    new AdaptiveImage()
                                    {
                                    Source =imgUri,
                                    HintAlign=AdaptiveImageAlign.Left,
                                    },


                        }
                    }

                }
            };

            //  Actions = new ToastActionsCustom() {  },

            //Audio = new ToastAudio() {  }
            ToastNotification notfn = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(notfn);
        }





    }        
        
}

