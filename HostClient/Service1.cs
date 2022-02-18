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
using System.IO.Ports;
using System.Threading.Tasks;

namespace HostClient
{
    public partial class Service1 : ServiceBase
    {

        private static string IP_HOST = ConfigurationManager.AppSettings["iphost"];
        private static string PORT_HOST = ConfigurationManager.AppSettings["porthost"];
        private static string MESSAGE = ConfigurationManager.AppSettings["message"];
        TcpListener hostlistener;
        SerialPort port;
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
            
            IPAddress ipAddress = IPAddress.Parse(IP_HOST);
            hostlistener = new TcpListener(ipAddress, Int32.Parse(PORT_HOST));
            hostlistener.Start();
            Socket client1 = hostlistener.AcceptSocket();
            Byte[] data = new Byte[1024];
            //if (client1.Receive(data) > 0) {
            //    Console.WriteLine(System.Text.Encoding.ASCII.GetString(data));
            //}
            data = System.Text.Encoding.ASCII.GetBytes(MESSAGE);
            System.Timers.Timer timer = new System.Timers.Timer();
            //  timer.Interval = (int)TimeSpan.FromMinutes(Int32.Parse(DELAY_MINUTE)).TotalMilliseconds;
            timer.Interval = 10000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                Console.WriteLine("===> " + DateTime.Now);

                try
                {
                    client1.Send(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    client1.Close();
                    hostlistener.Stop();
                    hostlistener = new TcpListener(ipAddress, Int32.Parse(PORT_HOST));
                    hostlistener.Start();

                    client1 = hostlistener.AcceptSocket();
                    client1.Send(data);
                }
            }
            byte[] dataReceive = new Byte[1024];
            client1.Receive(dataReceive);
            Console.WriteLine(System.Text.Encoding.ASCII.GetString(dataReceive));
        }

        protected override void OnStop()
        {
            hostlistener.Stop();
        }
    }
}
