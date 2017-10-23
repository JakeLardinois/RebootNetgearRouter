using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RebootNetgearRouter
{
    class Program
    {
        private static TelnetConnection _tc;

        static void Main(string[] args)
        {
            var connected = CheckNetworkConnectivity(Settings.DestinationPingAddress);

            if (!connected)
            {
                Console.WriteLine($@"There appears to be a problem with Network Connectivity...");
                Console.WriteLine($@"A NO Ping Response was received from {Settings.DestinationPingAddress}...");

                Console.WriteLine(@"Let's enable Telnet on the Router...");
                var telnetEnabled = EnableTelnet();
                if (telnetEnabled)
                {
                    Console.WriteLine($@"Telnet was successfully enabled on the Router.");

                    Console.WriteLine(@"Now let's telnet into the Router and reboot it...");
                    var successfulReboot = RebootRouter();
                    if (successfulReboot)
                        Console.WriteLine(@"The command was succesfully sent. The Router is being rebooted.");
                    else
                        Console.WriteLine(@"There was a problem and the Router was NOT successfully rebooted.");
                }
                else
                    Console.WriteLine(@"There was a problem when enabling telnet.");
            }
            else
                Console.WriteLine($@"Network Connectivity was Verified! A Successful Ping Response was received from {Settings.DestinationPingAddress}");

            Console.ReadLine();
        }

        public static bool RebootRouter()
        {
            TelnetConnection tc = new TelnetConnection(Settings.RouterIPAddress, 23);

            var response = tc.Read();
            Console.WriteLine(response);

            if (tc.IsConnected)
            {
                tc.WriteLine("reboot");
                return true;
            }
            return false;
        }

        public static bool EnableTelnet()
        {
            var telnetEnable = RebootNetgearRouter.Properties.Resources.telnetEnable;
            var fileName = System.Environment.CurrentDirectory + "\\telnetenable.exe";

            Console.WriteLine($@"Creating {fileName}...");
            WriteByteArrayToFile(telnetEnable, fileName);

            var telnetEnableProcessConfig =
                new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = $"{Settings.RouterIPAddress} {Settings.RouterMACAddress} {Settings.RouterUserName} {Settings.RouterPassword}"
                };
            var telnetEnableCommand = new Process { StartInfo = telnetEnableProcessConfig };

            Console.WriteLine($@"Executing the Command: {telnetEnableProcessConfig.FileName} {telnetEnableProcessConfig.Arguments}");
            telnetEnableCommand.Start();
            var output = telnetEnableCommand.StandardOutput.ReadToEnd();
            Console.WriteLine($@"Response Recieved: {output}");
            var err = telnetEnableCommand.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(err))
                Console.WriteLine($@"The following error was encountered during the command execution: {err}");
            telnetEnableCommand.WaitForExit();

            Console.WriteLine($@"Removing {fileName}...");
            File.Delete(fileName);

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
