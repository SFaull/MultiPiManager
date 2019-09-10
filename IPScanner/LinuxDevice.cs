using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPScanner
{
    public class LinuxDevice : Device
    {
        public bool ValidCredentials { get { return (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password)); } }
        public string Username { get; set; }
        public string Password { get; set; }

        public LinuxDevice()
        {
            // do nothing, please be sure to set the properties explicitly
        }

        public LinuxDevice(string _IpAddress, string _HostName, string _MacAddress) : base(_IpAddress, _HostName, _MacAddress)
        {

        }

        public LinuxDevice(Device device): base(device){ }

        public void SetCredentials(string _Username, string _Password)
        {
            Username = _Username;
            Password = _Password;
        }

    }
}
