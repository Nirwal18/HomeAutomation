using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.Model
{
   public class Btdevice
    {
        public string Name { get; set; }
        public int id { get; set; }
    }

    public class DeviceManager
    {
       public static List<Btdevice> GetDevice()
        {
            var device = new List<Btdevice>();
            device.Add(new Btdevice { Name = "phone 1", id = 0001 });
            device.Add(new Btdevice { Name = "phone 2", id = 0002 });
            device.Add(new Btdevice { Name = "phone 3", id = 0003 });
            device.Add(new Btdevice { Name = "phone 4", id = 0004 });
            device.Add(new Btdevice { Name = "phone 5", id = 0005 });
            device.Add(new Btdevice { Name = "phone 6", id = 0006 });
            return device;
        }

    }

}
