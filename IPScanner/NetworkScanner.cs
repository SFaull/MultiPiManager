using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPScanner
{
    class NetworkScanner
    {
        #region Properties

        private DeviceList deviceList;

        #endregion

        #region Constructor

        public NetworkScanner()
        {

        }

        #endregion

        #region Events

        public event EventHandler DeviceFound;
        public event EventHandler ScanComplete; // TODO: Implement

        protected virtual void OnDeviceFound(Device device)
        {
            DeviceFound?.Invoke(device, null);
        }

        protected virtual void OnScanComplete()
        {
            ScanComplete?.Invoke(null, null);
        }

        #endregion

        #region Methods

        public void StartScan()
        {
            deviceList = new DeviceList();

            // Add an event handler for each time an event is added to the collection
            deviceList.Devices.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(
            delegate (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                var device = (sender as ObservableCollection<Device>).Last();
                OnDeviceFound(device);
            });




            //PingRange(GetAllAddresses());
            PingAll();   //Automate pending
        }


        private static string NetworkGateway()
        {
            string ip = null;

            foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (f.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
                    {
                        ip = d.Address.ToString();
                    }
                }
            }

            return ip;
        }

        /// <summary>
        /// Ping a device
        /// </summary>
        /// <param name="host"></param>
        /// <param name="attempts"></param>
        /// <param name="timeout"></param>
        private void Ping(string host, int attempts, int timeout)
        {
            for (int i = 0; i < attempts; i++)
            {
                new Thread(delegate ()
                {
                    try
                    {
                        System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                        ping.PingCompleted += new PingCompletedEventHandler(PingCompleted);
                        ping.SendAsync(host, timeout, host);
                    }
                    catch
                    {
                        // Do nothing and let it try again until the attempts are exausted.
                        // Exceptions are thrown for normal ping failurs like address lookup
                        // failed.  For this reason we are supressing errors.
                    }
                }).Start();
            }
        }


        /// <summary>
        /// Ping all devices on the gateway
        /// </summary>
        private void PingAll()
        {
            string gate_ip = NetworkGateway();

            //Extracting and pinging all other ip's.
            string[] array = gate_ip.Split('.');

            for (int i = 2; i <= 255; i++)
            {
                string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + i;

                //time in milliseconds      
                Console.WriteLine("Ping: {0}", ping_var);
                Ping(ping_var, 4, 4000);
            }
        }


        /// <summary>
        /// Ping all devices on the gateway
        /// </summary>
        private List<IPAddress> GetAllAddresses()
        {
            List<IPAddress> ipList = new List<IPAddress>();

            string gate_ip = NetworkGateway();

            //Extracting and pinging all other ip's.
            string[] array = gate_ip.Split('.');

            for (int i = 2; i <= 255; i++)
            {
                string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + i;
                IPAddress ip;
                if (IPAddress.TryParse(ping_var, out ip))
                    ipList.Add(ip);
            }

            return ipList;
        }

        //TODO: doesnt work
        public void PingRange(List<IPAddress> range)
        {
            var finished = new CountdownEvent(1);
            foreach (IPAddress ip in range)
            {
                finished.AddCount(); // Indicate that a new ping is pending
                var pingSender = new Ping();
                pingSender.PingCompleted +=
                  (sender, e) =>
                  {
                      finished.Signal(); // Indicate that this ping is complete
                      
                      if (e.Reply != null && e.Reply.Status == IPStatus.Success)
                      {
                          string hostname = GetHostName((string)e.UserState);
                          string macaddress = GetMacAddress((string)e.UserState);

                          Device device = new Device((string)e.UserState, hostname, macaddress);

                          Console.WriteLine((string)e.UserState);
                          deviceList.AddDevice(device);
                      }
                      else
                      {
                          // MessageBox.Show(e.Reply.Status.ToString());
                      }
                  };
                string data = string.Empty;
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                PingOptions options = new PingOptions(64, true);
                pingSender.SendAsync(ip, 4000, buffer);
            }
            finished.Signal(); // Indicate that all pings have been submitted
            finished.Wait(); // Wait for all pings to complete
        }

        /// <summary>
        /// Callback for ping completion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                string hostname = GetHostName(ip);
                string macaddress = GetMacAddress(ip);

                Device device = new Device(ip, hostname, macaddress);
                deviceList.AddDevice(device);
                
            }
            else
            {
                // MessageBox.Show(e.Reply.Status.ToString());
            }
        }

        /// <summary>
        /// Get the hostname at a given IP address
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName;
                }
            }
            catch (SocketException)
            {
                // MessageBox.Show(e.Message.ToString());
            }

            return null;
        }


        /// <summary>
        /// Get the MAC address at a given IP address
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public string GetMacAddress(string ipAddress)
        {
            string macAddress = string.Empty;
            System.Diagnostics.Process Process = new System.Diagnostics.Process();
            Process.StartInfo.FileName = "arp";
            Process.StartInfo.Arguments = "-a " + ipAddress;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.CreateNoWindow = true;
            Process.Start();
            string strOutput = Process.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('-');
            if (substrings.Length >= 8)
            {
                macAddress = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2))
                         + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
                         + "-" + substrings[7] + "-"
                         + substrings[8].Substring(0, 2);
                return macAddress;
            }

            else
            {
                return "OWN Machine";
            }
        }
        #endregion
    }
}
