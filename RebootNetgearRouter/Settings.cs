using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebootNetgearRouter
{
    public static class Settings
    {
        //public static string ConnectionString => ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public static string RouterMACAddress => ConfigurationManager.AppSettings["RouterMACAddress"];
        public static string RouterUserName => ConfigurationManager.AppSettings["RouterUserName"];
        public static string RouterPassword => ConfigurationManager.AppSettings["RouterPassword"];
        public static string DestinationPingAddress => ConfigurationManager.AppSettings["DestinationPingAddress"];
    }
}
