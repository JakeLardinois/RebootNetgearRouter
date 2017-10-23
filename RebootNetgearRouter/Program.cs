using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace RebootNetgearRouter
{
    class Program
    {
        static void Main(string[] args)
        {
            var connected = CheckNetworkConnectivity(Settings.DestinationPingAddress);

            if (!connected)
            {

                var telnetEnabled = EnableTelnet();
                if (telnetEnabled)
                {
                    Console.WriteLine($@"Telnet was successfully enabled on the Router.");


                }
                else
                    Console.WriteLine(@"A problem happened and we're not going to try telnetting...");
            }
            else
                Console.WriteLine($@"Network Connectivity was Verified! A Successful Ping Response was received from {Settings.DestinationPingAddress}");

        }

        public static void RebootRouter()
        {
            
        }

        public static bool EnableTelnet()
        {
            var telnetEnable = RebootNetgearRouter.Properties.Resources.telnetEnable;
            var fileName = System.Environment.CurrentDirectory + "telnetenable.exe";
            WriteByteArrayToFile(telnetEnable, fileName);

            var telnetEnableProcessConfig =
                new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    //WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = $"{Settings.RouterIPAddress} {Settings.RouterMACAddress} + {Settings.RouterUserName} {Settings.RouterPassword}"
                };
            var telnetEnableCommand = new Process { StartInfo = telnetEnableProcessConfig };

            Console.WriteLine($@"Executing the Command: {telnetEnableProcessConfig.FileName} + {telnetEnableProcessConfig.Arguments}");
            telnetEnableCommand.Start();
            var output = telnetEnableCommand.StandardOutput.ReadToEnd();
            Console.WriteLine($@"Response Recieved: {output}");
            var err = telnetEnableCommand.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(err))
                Console.WriteLine($@"The following error was encountered during the command execution: {err}");
            telnetEnableCommand.WaitForExit();

            if (output.ToUpper().Contains("TELNET SHOULD BE ENABLED"))
                return true;
            return false;
        }

        public static bool CheckNetworkConnectivity(string ipAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(ipAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            return pingable;
        }

        public static void WriteByteArrayToFile(byte[] buff, string strFileName)
        {
            FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(buff);
            bw.Close();
        }
    }
}
