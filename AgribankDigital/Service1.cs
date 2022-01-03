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
using System.Threading.Tasks;

namespace AgribankDigital
{
    public partial class Service1 : ServiceBase
    {
        private static string HOST_CLIENT = ConfigurationManager.AppSettings["hostClient"];
        private static string PORT_FORWARD = ConfigurationManager.AppSettings["portForward"];
        private static string PORT_CLIENT = ConfigurationManager.AppSettings["portClient"];
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



            /*    System.Timers.Timer timer = new System.Timers.Timer();

                  timer.Interval = 200;
                 timer.Elapsed += timer_Elapsed;
                 timer.Start();*/

        }
        /*   void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
           {
               string date = e.SignalTime.ToString("yyyyMMdd");
               Logger.Log(" Timer:" + e.SignalTime);


           }*/
        protected void ListenerMethod()
        {


            selfListener = new TcpListener(IPAddress.Any, Int32.Parse(PORT_FORWARD));
            selfListener.Start();

            Byte[] bytes1 = new Byte[256];
            String data = null;
            try
            {
                while (true)
                {

                    TcpClient atmClient = selfListener.AcceptTcpClient();
                    TcpClient hostClient = new TcpClient(HOST_CLIENT, Int32.Parse(PORT_CLIENT));

                    Console.WriteLine("Connected");

                    data = null;

                    NetworkStream atmStream = atmClient.GetStream();
                    NetworkStream hostStream = hostClient.GetStream();

                    int i;
                    while ((i = atmStream.Read(bytes1, 0, bytes1.Length)) != 0)
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes1, 0, i);
                        Logger.Log("ATM > " + data + DateTime.Now);
                        Console.WriteLine("Received: {0}", data);
                        byte[] atmMsg = System.Text.Encoding.ASCII.GetBytes(data);

                        // send to  host
                        hostStream.Write(atmMsg, 0, atmMsg.Length);
                        Logger.Log("HOST < " + data + DateTime.Now);

                        Byte[] hostData = new Byte[256];

                        // host response
                        String responseData = String.Empty;
                        Int32 bytes = hostStream.Read(hostData, 0, hostData.Length);
                        responseData = System.Text.Encoding.ASCII.GetString(hostData, 0, bytes);
                        Logger.Log("HOST > " + responseData + DateTime.Now);
                        Console.WriteLine("Received: {0}", responseData);

                        byte[] hostMsg = System.Text.Encoding.ASCII.GetBytes(responseData);

                        // response data to atm
                        Logger.Log("ATM < " + responseData + DateTime.Now);
                        atmStream.Write(hostMsg, 0, hostMsg.Length);
                    }

                    hostStream.Close();
                    hostClient.Close();
                    atmClient.Close();



                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
           
        
        }


        protected override void OnStop()
        {
        }
    }
}
