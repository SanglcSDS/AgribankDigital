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
using System.Threading.Tasks;

namespace HostClient
{
    public partial class Service1 : ServiceBase
    {

        private static string IP_HOST = ConfigurationManager.AppSettings["iphost"];
        private static string PORT_HOST = ConfigurationManager.AppSettings["porthost"];
        private static string MESSAGE = ConfigurationManager.AppSettings["message"];
        TcpListener hostlistener;
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
            // Int32 port1 = 1234;
            IPAddress ipAddress = IPAddress.Parse(IP_HOST);
            hostlistener = new TcpListener(ipAddress, Int32.Parse(PORT_HOST));
            hostlistener.Start();

            Byte[] bytes1 = new Byte[256];
            String data1 = null;

            while (true)
            {
                TcpClient client1 = hostlistener.AcceptTcpClient();
                data1 = null;
                NetworkStream stream1 = client1.GetStream();
                int i;
                while ((i = stream1.Read(bytes1, 0, bytes1.Length)) != 0)
                {
                    data1 = System.Text.Encoding.ASCII.GetString(bytes1, 0, i);
                    Logger.Log("Received: {0}" + data1);


                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(MESSAGE);


                    stream1.Write(msg, 0, msg.Length);
                }

                client1.Close();
            }

        }


        protected override void OnStop()
        {
        }
    }
}
