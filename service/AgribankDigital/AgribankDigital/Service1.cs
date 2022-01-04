using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgribankDigital
{
    public partial class Service1 : ServiceBase
    {
        private static string HOST_CLIENT = ConfigurationManager.AppSettings["ip_host"];
        private static string PORT_FORWARD = ConfigurationManager.AppSettings["port_listen"];
        private static string PORT_CLIENT = ConfigurationManager.AppSettings["port_host"];
        private static string BYTE = ConfigurationManager.AppSettings["byte"];
        Thread listenerThread;
        TcpListener selfListener;
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

            selfListener = new TcpListener(Int32.Parse(PORT_FORWARD));
            selfListener.Start();

            Byte[] bytes1 = new Byte[Int32.Parse(BYTE)];
            String data = null;
            while (true)
            {

                try
                {
                    TcpClient atmClient = selfListener.AcceptTcpClient();
                    TcpClient hostClient = new TcpClient(HOST_CLIENT, Int32.Parse(PORT_CLIENT));

                    data = null;

                    NetworkStream atmStream = atmClient.GetStream();
                    NetworkStream hostStream = hostClient.GetStream();

                    int i;
                    while ((i = atmStream.Read(bytes1, 0, bytes1.Length)) != 0)
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes1, 0, i);
                        Logger.Log(DateTime.Now.ToString());
                        Logger.Log("ATM > " + data);
                        byte[] atmMsg = System.Text.Encoding.ASCII.GetBytes(data);

                        // send to  host
                        hostStream.Write(atmMsg, 0, atmMsg.Length);
                        Logger.Log(DateTime.Now.ToString());
                        Logger.Log("HOST < " + data);

                        Byte[] hostData = new Byte[Int32.Parse(BYTE)];

                        // host response
                        String responseData = String.Empty;
                        Int32 bytes = hostStream.Read(hostData, 0, hostData.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(hostData, 0, bytes);
                        Logger.Log(DateTime.Now.ToString());
                        Logger.Log("HOST > " + responseData);


                        byte[] hostMsg = System.Text.Encoding.ASCII.GetBytes(responseData);

                        Logger.Log(DateTime.Now.ToString());
                        Logger.Log("ATM < " + responseData);
                        atmStream.Write(hostMsg, 0, hostMsg.Length);
                    }

                    hostStream.Close();
                    hostClient.Close();
                    atmClient.Close();
                }
                catch (SocketException e)
                {
                    Logger.Log("SocketException: {0}"+ e);
                }
            }
        }


        protected override void OnStop()
        {
            selfListener.Stop();
        }
    }
}
