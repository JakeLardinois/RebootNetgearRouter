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
                var telnetEnabled = EnableTelnet();
                if (telnetEnabled)
                {
                    Console.WriteLine($@"Telnet was successfully enabled on the Router.");
                    var successfulReboot = RebootRouter();
                    if (successfulReboot)
                        Console.WriteLine(@"The Router is being successfully rebooted");
                    else
                        Console.WriteLine(@"There was a problem and the Router was not successfully rebooted...");
                }
                else
                    Console.WriteLine(@"A problem happened and we're not going to try telnetting...");
            }
            else
                Console.WriteLine($@"Network Connectivity was Verified! A Successful Ping Response was received from {Settings.DestinationPingAddress}");

            Console.ReadLine();
        }

        /*public static bool RebootRouter()
        {
            var fileName = "telnet.exe";
            var telnetProcessConfig = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c DIR {fileName} {Settings.RouterIPAddress}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            Process telnetCommand = new Process { StartInfo = telnetProcessConfig };

            Console.WriteLine($@"Executing the Command: {telnetProcessConfig.FileName} {telnetProcessConfig.Arguments}");
            telnetCommand.Start();
            var output = telnetCommand.StandardOutput.ReadToEnd();
            Console.WriteLine($@"Response Recieved: {output}");
            var err = telnetCommand.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(err))
                Console.WriteLine($@"The following error was encountered during the command execution: {err}");
            telnetCommand.WaitForExit();

            if (output.Contains("#"))
            {
                telnetCommand.StandardInput.WriteLine("reboot");
                return true;
            }
            return false;
        }*/

        public static bool RebootRouter()
        {
            //create a new telnet connection to hostname "gobelijn" on port "23"
            TelnetConnection tc = new TelnetConnection(Settings.RouterIPAddress, 23);

            var response = tc.Read();
            Console.WriteLine(response);

            //login with user "root",password "rootpassword", using a timeout of 100ms, and show server output
            //string s = tc.Login(Settings.RouterUserName, Settings.RouterPassword, 100);
            //Console.Write(s);

            // server output should end with "$" or ">" or "#", otherwise the connection failed
            //string prompt = s.TrimEnd();
            //prompt = s.Substring(prompt.Length - 1, 1);
            //if (prompt != "$" || prompt != ">" || prompt != "#")
            //    throw new Exception("Connection failed");

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
            WriteByteArrayToFile(telnetEnable, fileName);

            var telnetEnableProcessConfig =
                new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    //WindowStyle = ProcessWindowStyle.Hidden,
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
