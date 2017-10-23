Much of the information gleaned for this application was taken from the article https://wiki.openwrt.org/toh/netgear/telnet.console

But basically what this application will do is to ping Google.com to check for network connectivity.  If it does not recieve a successful response,
then it will spit out the executable created from this repo https://github.com/insanid/NetgearTelnetEnable, 
in particular this executable https://github.com/insanid/NetgearTelnetEnable/blob/master/binaries/windows/telnetEnable.exe and then telnet
into the Netgear router (using whatever telnet client is installed on the local machine) and issue a reboot command to the router.

The idea is that this application can run in a scheduled task on a windows machine and monitor network connectivity

Inside of App.config you need to specify
	RouterMACAddress which is the MAC Address of the router that you are looking to unlock Telnet.  The property must be all uppercase and no colons ex.:
		E4F4C609C352
	RouterUserName which is the admin username of the Router that you are looking to unlock Telnet.
	Routerpassword which is the password for the RouterUserName specified above.

	Sample execution of telnetenable.exe: 
		telnetenable 192.168.1.1 E4F4C609C352 myUsername myPassword