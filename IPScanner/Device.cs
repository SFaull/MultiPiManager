using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPScanner
{
    class Device
    {
        public string IpAddress { get; private set; }
        public string MacAddress { get; private set; }
        public string HostName { get; private set; }

        public Device(string _IpAddress, string _HostName, string _MacAddress)
        {
            this.IpAddress = _IpAddress;
            this.HostName = _HostName;
            this.MacAddress = _MacAddress;
        }
    }
}
