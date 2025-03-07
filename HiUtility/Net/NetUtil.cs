using System.Net.NetworkInformation;
using System.Text;

namespace MrHihi.HiUtility.Net;

public class NetUtil
{
    public static PingReply Ping(string address, int timeout = 120, string data = "")
    {
        Ping pingSender = new Ping ();
        PingOptions options = new PingOptions ();
        options.DontFragment = true;
        byte[] buffer = Encoding.ASCII.GetBytes (data);
        PingReply reply = pingSender.Send (address, timeout, buffer, options);
        return reply;
    }
}