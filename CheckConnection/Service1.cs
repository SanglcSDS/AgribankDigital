using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace CheckConnection
{
    public partial class Service1 : ServiceBase
    {
        static Thread mainThread = null;
        static Thread atmThread = null;
        static Thread hostThread = null;
        static ATM atm = null;
        static Host host = null;

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
           /* atm = new ATM();
            atmThread = new Thread(new ThreadStart(() => atm.CheckConnectATM()));
           
            atmThread.Start();*/

            host = new Host();
            hostThread = new Thread(new ThreadStart(() => host.CheckConnectHost()));
            hostThread.Start();
        }

       
     /*   public void main()
        {
            atm = new ATM();
            atmThread = new Thread(new ThreadStart(() => atm.CheckConnectATM()));
            atmThread.Start();

        }*/
        protected override void OnStop()
        {
           
            mainThread.Abort();
            atmThread.Abort();
            atm.Close();
            host.Close();
            hostThread.Abort();

        }

    }
}
