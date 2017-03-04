using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace HomeAutomation.Model
{
    public class Btdevice : INotifyPropertyChanged
    {
        private DeviceInformation deviceInfo;

        // class constructor
        public Btdevice(DeviceInformation deviceInfonIn)
        {
            deviceInfo = deviceInfonIn;
        }

        public string Name
        {
            get
            {
                return deviceInfo.Name;
            }
        }
        public string Id
        {
            get
            {
                return deviceInfo.Id;
            }
        }

        public bool IsPared
        {
            get
            {
                return deviceInfo.Pairing.IsPaired;
            }
        }

        public void Update(DeviceInformationUpdate updateDevice)
        {
            deviceInfo.Update(updateDevice);
        }


        // interface implementation

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }

}  