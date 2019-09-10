using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IPScanner
{
    [System.Xml.Serialization.XmlInclude(typeof(LinuxDevice))]
    public class Device
    {
        public string FriendlyName { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }
        public string HostName { get; set; }

        public Device()
        {
            // do nothing, please be sure to set the properties explicitly
        }

        public Device(string _IpAddress, string _HostName, string _MacAddress)
        {
            this.IpAddress = _IpAddress;
            this.HostName = _HostName;
            this.MacAddress = _MacAddress;
        }

        public Device(Device device):this(device.IpAddress, device.HostName, device.MacAddress)
        {
            // do nothing
        }
    }
}
