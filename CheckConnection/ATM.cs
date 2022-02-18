using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;

namespace CheckConnection
{
    class ATM
    {
        public Socket socketATM;
        TcpListener listener;
        static string serviceName = "";
        public ATM()
        {

            listener = new TcpListener(IPAddress.Any, Utils.PORT_LISTIEN);
            listener.Start();
            socketATM = listener.AcceptSocket();
            listener.Stop();
            if (socketATM.Connected)
            {
                Logger.Log("Connected to ATM : " + socketATM.Connected);
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
            Console.WriteLine("Check connection");
            try
            {
                bool check = !(socketATM.Poll(Utils.CHECK_CONNECTION_TIMEOUT, SelectMode.SelectRead) && socketATM.Available == 0);

                Console.WriteLine("Check = " + check);
                if (!check)
                {
                    KillProcesses(ProcssesName());
                }
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

        public void CheckConnectATM()
        {
            while (true)
            {

                if (!IsConnected())
                {
                    if (serviceName.Length>0)
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
            if (socketATM.Connected)
                socketATM.Disconnect(false);
            listener.Stop();
        }

        public void Terminate()
        {
            if (socketATM.Connected)
                socketATM.Disconnect(false);
            listener.Stop();
        }

    }
}
