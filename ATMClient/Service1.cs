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

namespace ATMClient
{
    public partial class Service1 : ServiceBase
    {
        private static string DELAY_MINUTE = ConfigurationManager.AppSettings["delayMinutes"];
        private static string IP_CLIENT = ConfigurationManager.AppSettings["ipclient"];
        private static string PORT_CLIENT = ConfigurationManager.AppSettings["portclient"];
        private static string MESSAGE = ConfigurationManager.AppSettings["message"];
        Thread listenerThread;
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
            System.Timers.Timer timer = new System.Timers.Timer();
            //  timer.Interval = (int)TimeSpan.FromMinutes(Int32.Parse(DELAY_MINUTE)).TotalMilliseconds;
            timer.Interval = 1000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();


        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            listenerThread = new Thread(new ThreadStart(Connect));

            listenerThread.Start();


        }
        protected override void OnStop()
        {
        }
        static void Connect()
        {
            try
            {
                TcpClient client = new TcpClient(IP_CLIENT, Int32.Parse(PORT_CLIENT));
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(MESSAGE + " " + DateTime.Now);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                data = new Byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Logger.Log("ArgumentNullException:{0}" + e);

            }
            catch (SocketException t)
            {
                Logger.Log("ArgumentNullException:{0} " + t);

            }


        }
    }
}
