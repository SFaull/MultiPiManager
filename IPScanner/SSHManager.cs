using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace IPScanner
{
    class SSHManager
    {
        private ConnectionInfo ConnNfo;

        public SSHManager()
        {

        }


        public bool Initialise(string host, string username, string password)
        {
            bool connected = false;
            try
            {

                // Setup Credentials and Server Information
                ConnNfo = new ConnectionInfo(host, 22, username,
                new AuthenticationMethod[]{

                // Pasword based Authentication
                new PasswordAuthenticationMethod(username,password),

                // Key Based Authentication (using keys in OpenSSH Format)
                /*
                new PrivateKeyAuthenticationMethod("username",new PrivateKeyFile[]{
                    new PrivateKeyFile(@"..\openssh.key","passphrase")
                }),
                */
                });


                using (var sshclient = new SshClient(ConnNfo))
                {
                    sshclient.Connect();
                    connected = sshclient.IsConnected;
                    sshclient.Disconnect();
                }
            }
            catch
            {
                connected = false;
            }

            return connected;
        }

        public string SendCommand(string command)
        {
            string response = string.Empty;

            // Execute a (SHELL) Command - prepare upload directory
            using (var sshclient = new SshClient(ConnNfo))
            {
                sshclient.Connect();
                using (var cmd = sshclient.CreateCommand(command))
                {
                    cmd.Execute();
                    Console.WriteLine("Command>" + cmd.CommandText);
                    Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                    Console.WriteLine("Response = {0}", cmd.Result);
                    response = cmd.Result;
                }
                sshclient.Disconnect();
            }
            return response;
        }

        public void UploadFile(string filepath, string targetdir)
        {
            // Upload A File
            using (var sftp = new SftpClient(ConnNfo))
            {
                sftp.Connect();
                sftp.ChangeDirectory(targetdir);
                using (var uplfileStream = System.IO.File.OpenRead(filepath))
                {
                    sftp.UploadFile(uplfileStream, filepath, true);
                }
                sftp.Disconnect();
            }
        }

    }
}
