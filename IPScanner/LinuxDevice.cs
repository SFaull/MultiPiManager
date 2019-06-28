using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPScanner
{
    class LinuxDevice : Device
    {
        public bool ValidCredentials { get { return (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password)); } }
        public string Username { get; set; }

        private string Password { get; set; }

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
