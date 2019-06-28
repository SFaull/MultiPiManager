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

/* Code adapted from https://www.codeproject.com/Tips/889483/How-to-List-all-devices-info-on-your-WLAN-router */

namespace IPScanner
{
    public partial class MainForm : Form
    {
        NetworkScanner nscan;
        DeviceList deviceList;
        Device selectedDevice;

        public MainForm()
        {
            InitializeComponent();

            lstLocal.View = View.Details;
            lstLocal.Clear();
            lstLocal.GridLines = true;
            lstLocal.FullRowSelect = true;
            lstLocal.Columns.Add("IP", 100);
            lstLocal.Columns.Add("HostName", 200);
            lstLocal.Columns.Add("MAC Address", 300);
            lstLocal.Sorting = SortOrder.Descending;


            
        }

        private void NetworkScanner_OnScanComplete(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate {
                MessageBox.Show("Scan Complete");
            });
            
        }

        private void NetworkScanner_OnDeviceFound(object sender, EventArgs e)
        {
            var device = sender as Device;
            if(device != null)
                AddToList(device);
        }

        /// <summary>
        /// Adds a single device to the list
        /// </summary>
        /// <param name="device"></param>
        private void AddToList(Device device)
        {
            this.Invoke((MethodInvoker)delegate {

                // Logic for Ping Reply Success
                ListViewItem item;

                string[] arr = new string[3];

                //store all three parameters to be shown on ListView
                arr[0] = device.IpAddress;
                arr[1] = device.HostName;
                arr[2] = device.MacAddress;

                item = new ListViewItem(arr);

                lstLocal.Items.Add(item);
                deviceList.AddDevice(device);
            });

        }

        /// <summary>
        /// Restart the search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            nscan.DeviceFound -= NetworkScanner_OnDeviceFound;
            lstLocal.Clear();
            nscan = new NetworkScanner();
            nscan.DeviceFound += NetworkScanner_OnDeviceFound;
            nscan.StartScan();
        }

        private void lstLocal_MouseClick(object sender, MouseEventArgs e)
        {
            var listview = sender as ListView;

            for (int i = 0; i < listview.Items.Count; i++)
            {
                var rectangle = listview.GetItemRect(i);
                if (rectangle.Contains(e.Location))
                {
                    string ip = listview.GetItemAt(e.Location.X, e.Location.Y).Text;
                    SelectDevice(deviceList.GetDeviceByIpAddress(ip));
                    return;
                }
            }
        }

        private void SelectDevice(Device device)
        {
            gbDeviceInfo.Visible = false;
            selectedDevice = device;

            if (gbConnect.Visible == false)
                gbConnect.Visible = true;

            txtIpAddress.Text = selectedDevice.IpAddress;
            txtHostname.Text = selectedDevice.HostName;
            txtMacAddress.Text = selectedDevice.MacAddress;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            selectedDevice = null;
            deviceList = new DeviceList();
            nscan = new NetworkScanner();
            nscan.DeviceFound += NetworkScanner_OnDeviceFound;
            nscan.ScanComplete += NetworkScanner_OnScanComplete;
            nscan.StartScan();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            SSHManager sshManager = new SSHManager();
            bool connected = sshManager.Initialise(txtIpAddress.Text, txtUsername.Text, txtPassword.Text);

            if (!connected)
            {
                MessageBox.Show("Connection Refused");
                return;
            }

            lblDeviceInfo.Text = sshManager.SendCommand("hostnamectl");
            gbDeviceInfo.Visible = true;
            //MessageBox.Show(response);

            var newDevice = new LinuxDevice(selectedDevice);
            newDevice.SetCredentials(txtUsername.Text, txtPassword.Text);
            deviceList.AddLinuxDevice(newDevice);

            // example of how to iterate over the list and determin which devices are linux devices
            /*
            foreach(var dev in deviceList.Devices)
            {
                var tmp = dev as LinuxDevice;

                Console.WriteLine("type? --> {0}", dev.GetType());
            }
            */

        }
    }
}
