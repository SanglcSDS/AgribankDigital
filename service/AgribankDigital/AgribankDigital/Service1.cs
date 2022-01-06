using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;


namespace AgribankDigital
{
    public partial class Service1 : ServiceBase
    {

        private static string HOST_CLIENT = ConfigurationManager.AppSettings["ip_host"];
        private static int PORT_FORWARD = Int32.Parse(ConfigurationManager.AppSettings["port_listen"]);
        private static int PORT_CLIENT = Int32.Parse(ConfigurationManager.AppSettings["port_host"]);

        Thread listenerThread;

        TcpListener listener = null;
        Socket socketATM = null;
        Socket socketHost = null;

        public Service1()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);

        }

        protected override void OnStart(string[] args)
        {
            listenerThread = new Thread(new ThreadStart(ListenerMethod));

            listenerThread.Start();

        }


        protected void ListenerMethod()
        {
            try
            {
                Logger.Log("Service is started");

                listener = new TcpListener(IPAddress.Any, PORT_FORWARD);

                listener.Start();

                Logger.Log("Listening connect from ATM ...");

                socketATM = listener.AcceptSocket();

                Logger.Log("ATM connected: " + socketATM.Connected);

                //Tao ket noi toi Host
                Logger.Log("Connecting to Host ...");

                TcpClient tcpClient = new TcpClient(HOST_CLIENT, PORT_CLIENT);
                socketHost = tcpClient.Client;

                Logger.Log("Connected to Host : " + socketHost.Connected);

                if (socketATM.Connected && socketHost.Connected)
                {
                    //Gui/nhan data tu ATM - Host
                    ThreadPool.QueueUserWorkItem(ReceiveDataFromATM, null);
                    ThreadPool.QueueUserWorkItem(ReceiveDataFromHost, null);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message);
            }
        }

        byte[] ReceiveAll(Socket socket)
        {
            var buffer = new List<byte>();

            while (socket.Available > 0)
            {
                var currByte = new Byte[1];
                var byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);

                if (byteCounter.Equals(1))
                {
                    buffer.Add(currByte[0]);
                }
            }

            return buffer.ToArray();
        }

        void ReceiveDataFromATM(object state)
        {
            try
            {
                while (true)
                {
                    Byte[] data = ReceiveAll(socketATM);
                    if (data.Length > 0)
                    {
                        Logger.Log(Environment.NewLine + DateTime.Now.ToString("HH:mm:ss fff") + " ATM to FW:");
                        Logger.Log("> " + System.Text.Encoding.ASCII.GetString(data));

                        socketHost.Send(data);

                        Logger.Log(Environment.NewLine + DateTime.Now.ToString("HH:mm:ss fff") + " FW to Host:");
                        Logger.Log("> " + System.Text.Encoding.ASCII.GetString(data));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message);
            }
        }

        void ReceiveDataFromHost(object state)
        {
            try
            {
                while (true)
                {
                    Byte[] data = ReceiveAll(socketHost);

                    if (data.Length > 0)
                    {
                        Logger.Log(Environment.NewLine + DateTime.Now.ToString("HH:mm:ss fff") + " Host to FW:");
                        Logger.Log("< " + System.Text.Encoding.ASCII.GetString(data));

                        socketATM.Send(data);

                        Logger.Log(Environment.NewLine + DateTime.Now.ToString("HH:mm:ss fff") + " FW to ATM:");
                        Logger.Log("< " + System.Text.Encoding.ASCII.GetString(data));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error: " + ex.Message);
            }
        }

        protected override void OnStop()
        {
            if (socketATM != null)
                socketATM.Close();

            if (socketHost != null)
                socketHost.Close();

            if (listener != null)
                listener.Stop();

            listenerThread.Abort();
        }
    }
}
