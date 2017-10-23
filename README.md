# RebootNetgearRouter
A Utility that checks for network connectivity and reboots the Netgear Router if there is not a successful verification.

Verified to work on the Netgear Nighthawk R7000 

Much of the information gleaned for this application was taken from the article https://wiki.openwrt.org/toh/netgear/telnet.console

But basically what this application will do is to ping an IP address to check for network connectivity.  If it does not recieve a successful response,
then it will spit out the executable created from this repo https://github.com/insanid/NetgearTelnetEnable, 
in particular this executable https://github.com/insanid/NetgearTelnetEnable/blob/master/binaries/windows/telnetEnable.exe and then telnets
into the Netgear router to issue a reboot command.

The telnet client used by this application was initially created by Tom Janssens and is detailed https://www.codeproject.com/Articles/19071/Quick-tool-A-minimalistic-Telnet-library.  Some other repos
to check out in github regarding this functionality are: https://github.com/jonsagara/MinimalisticTelnet & https://github.com/aaron-salisbury/MinimalisticTelnet

The application is intended to run in a scheduled task on a windows machine and periodically execute to monitor network connectivity

Inside of App.config you need to specify:

	"DestinationPingAddress" which is the address that will be pinged to check for network connectivity. It can be an IP Address or a name; basically anything that would be specified in a Ping command.

	"RouterIPAddress" the IP Address of the Router.
	
	"RouterMACAddress which is the MAC Address of the router that you are looking to unlock Telnet.  The property must be all uppercase and no colons ex.: E4F4C609C352
	
	"RouterUserName" which is the admin username of the Router that you are looking to unlock Telnet.
	
	"Routerpassword" which is the password for the RouterUserName.
	
	"TelnetPort" the port to use for telnetting into the router. If an invalid value is supplied, then the default of 23 will be used.

Sample execution of telnetenable.exe:

	telnetenable 192.168.1.1 E4F4C609C352 myUsername myPassword

Sample execution of telnet into router:

	telnet 192.168.1.1
