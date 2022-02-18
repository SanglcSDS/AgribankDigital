using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace CheckConnection
{
    class Host
    {
        //Socket socketATM;
        public Socket socketHost;
        TcpClient tcpClient;
        //TcpListener listener;
        static string serviceName = "";
  

        public Host()
        {
            while (true)
            {
                try
                {
                    TcpClient newTcpClient = new TcpClient(Utils.IP_HOST, Utils.PORT_HOST);
                    socketHost = newTcpClient.Client;

                    if (socketHost.Connected)
                    {
                        Logger.Log("Connected to Host : " + socketHost.Connected);
                        return;
                    }
                    else
                    {
                        Logger.Log("Cannot connect to Host, trying to reconnect ...");
                        Thread.Sleep(Utils.RESET_ERR_DELAY);
                        socketHost.Close();
                        tcpClient.Close();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception while connecting to Host: " + ex.Message);
                    Logger.Log("Cannot connect to Host, trying to reconnect ...");
                }
            }
        }
        public Process GetProcByID(int id)
        {
            Process[] processlist = Process.GetProcesses();
            return processlist.FirstOrDefault(pr => pr.Id == id);
        }

        public string ProcssesName()
        {

            using (var stream = new FileStream(path: Utils.IDPROCSSES_LOG, mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string ProcssesID = reader.ReadLine();
                    Process process = GetProcByID(Int32.Parse(ProcssesID));
                    if (process != null)
                    {
                        serviceName = process.ProcessName;
                        return process.ProcessName;
                    }


                    return "";
                }
            }

        }
        // ham  kill
        public void KillProcesses(string naneService)
        {

            Process[] runningProcesses = Process.GetProcesses();
            foreach (Process process in runningProcesses)
            {
                if (process.ProcessName.Equals(naneService))
                {


                    process.Kill();
                }

            }
        }

        public bool IsConnected()
        {
            try
            {
                bool check = !(socketHost.Poll(Utils.CHECK_CONNECTION_TIMEOUT, SelectMode.SelectRead) && socketHost.Available == 0);
                if (!check)
                    KillProcesses(ProcssesName());
                return check;
            }
            catch (SocketException)
            {
                KillProcesses(ProcssesName());

                return false;
            }
            catch (ObjectDisposedException)
            {
                KillProcesses(ProcssesName());

                return false;
            }
        }
        public void CheckConnectHost()
        {
            while (true)
            {

                if (!IsConnected())
                {
                    if (serviceName.Length > 0)
                    {
                        //start laij servic
                        ServiceController serviceController = new ServiceController("Agribank Digital");

                        serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                        serviceController.Start();
                        serviceController.WaitForStatus(ServiceControllerStatus.Running);
                    }


                }

            }
        }
        public void Close()
        {
            if (socketHost.Connected)
                socketHost.Disconnect(false);
            if (tcpClient.Connected)
                tcpClient.Close();
        }

    }
}
