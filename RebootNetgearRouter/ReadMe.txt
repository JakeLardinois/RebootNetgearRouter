Much of the information gleaned for this application was taken from the article https://wiki.openwrt.org/toh/netgear/telnet.console

But basically what this application will do is to ping Google.com to check for network connectivity.  If it does not recieve a successful response,
then it will spit out the executable created from this repo https://github.com/insanid/NetgearTelnetEnable, 
in particular this executable https://github.com/insanid/NetgearTelnetEnable/blob/master/binaries/windows/telnetEnable.exe and then telnet
into the Netgear router (using whatever telnet client is installed on the local machine) and issue a reboot command to the router.

The idea is that this application can run in a scheduled task on a windows machine and monitor network connectivity

