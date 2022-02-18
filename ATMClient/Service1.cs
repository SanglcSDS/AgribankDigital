using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
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
      
        TcpClient client = null;
        Socket socket = null;

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
            timer.Interval = 10000;
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
            socket.Close();
            client.Close();
            listenerThread.Abort();

        }
        void Connect()
        {
            try
            {
                client = new TcpClient(IP_CLIENT, Int32.Parse(PORT_CLIENT));
              //  client.Close();
                string str = File.ReadAllText("E:\\test.txt");
                SerialPort port = new SerialPort(PORT_CLIENT);
         
              // port.Close();

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(str);
                //NetworkStream stream = client.GetStream();
                //stream.Write(data, 0, data.Length);
                //data = new Byte[20024];
                //String responseData = String.Empty;
                //Int32 bytes = stream.Read(data, 0, data.Length);
                //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                //Console.WriteLine("Received: {0}", responseData);

                socket = client.Client;

                System.Timers.Timer timer = new System.Timers.Timer();
                //  timer.Interval = (int)TimeSpan.FromMinutes(Int32.Parse(DELAY_MINUTE)).TotalMilliseconds;
                timer.Interval = 10000;
                timer.Elapsed += timer_Elapsed;
                timer.Start();

                while (true)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        socket.Send(data);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        if (client != null && client.Connected)
                            client.Close();

                        socket.Disconnect(true);

                        client = new TcpClient(IP_CLIENT, Int32.Parse(PORT_CLIENT));
                        socket = client.Client;
                        //if (socket.Connected) break;
                    }

                }

                

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Err" + e.Message);

            }
            catch (SocketException t)
            {

                Console.WriteLine("Err" + t.Message);

            }


        }
    }
}
