using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgribankDigital
{
    class Logger
    {
        private static string FILE_LOG = ConfigurationManager.AppSettings["trans_log"];
        public static object _locked = new object();
        public static void Log(string message)
        {
            try
            {
                lock (_locked)
                {
                    string fileLog = PathLocation(FILE_LOG) + DateTime.Now.ToString("yyyyMMdd") + ".log";

                    string _message = string.Format("{0}{1}", message, Environment.NewLine);
                    File.AppendAllText(fileLog, _message);

                }


            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));
            }
        }
        public static string PathLocation(string value)
        {
            try
            {
                if (Directory.Exists(value))
                {
                    return value;
                }
                DirectoryInfo di = Directory.CreateDirectory(value);
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("The process failed: {0}", ex.ToString()));

            }
            return value;



        }
    }
}
