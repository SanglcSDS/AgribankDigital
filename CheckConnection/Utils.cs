using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CheckConnection
{
   public static class Utils
    {
        public static int PORT_HOST = Int32.Parse(ConfigurationManager.AppSettings["port_host"]);
        public static string IP_HOST = ConfigurationManager.AppSettings["ip_host"];
        public static int RESET_ERR_DELAY = Int32.Parse(ConfigurationManager.AppSettings["reset_err_delay"]);
        public static string IDPROCSSES_LOG = ConfigurationManager.AppSettings["idprocesses_log"];
        public static int PORT_LISTIEN = Int32.Parse(ConfigurationManager.AppSettings["port_listen"]);
        public static int CHECK_CONNECTION_TIMEOUT = Int32.Parse(ConfigurationManager.AppSettings["check_connection_timeout"]);
    }
}
