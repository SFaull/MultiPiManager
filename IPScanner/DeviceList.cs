using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPScanner
{
    public class DeviceList
    {
        public ObservableCollection<Device> Devices;

        public DeviceList()
        {
            Devices = new ObservableCollection<Device>();
        }

        public bool AddDevice(Device device)
        {
            bool containsItem = true;

            try
            {
                containsItem = Devices.Any(item => item.IpAddress == device.IpAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            if (containsItem)
                return false;

            Devices.Add(device);
            return true;
        }


        /// <summary>
        /// Add a linux device, this will replace an existing generic device
        /// </summary>
        /// <param name="linuxdevice"></param>
        /// <returns></returns>
        public bool AddLinuxDevice(LinuxDevice linuxdevice)
        {
            for(int i=0; i< Devices.Count; i++)
            {
                if (Devices[i].MacAddress.Equals(linuxdevice.MacAddress))
                {
                    Devices[i] = linuxdevice;
                    return true;
                }
            }
            return false;
        }


        public Device GetDeviceByIpAddress(string ip)
        {
            foreach(Device dev in Devices)
            {
                if (dev.IpAddress.Equals(ip))
                    return dev;
            }

            return null;
        }
    }
}
