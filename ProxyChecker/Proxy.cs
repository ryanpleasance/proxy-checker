using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Http;

namespace ProxyChecker
{
    public class Proxy
    {
        public int RowIndex { get; set; }
        public string Domain { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public long? Speed { get; set; }
        public string Status { get; set; }
        public Proxy(string domain, int port, string username = null, string password = null)
        {
            this.Domain = domain;
            this.Port = port;
            this.Username = username;
            this.Password = password;
            this.Status = "Unknown";

        }
        public static WebProxy Parse(Proxy input)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            WebProxy parsedProxy = new WebProxy();

            IPAddress[] ip = Dns.GetHostAddresses(input.Domain); // will throw exception if it cant resolve IP

            parsedProxy.Address = new Uri("http://" + ip[0] + ":" + input.Port);

            if (input.Username != null)
            {
                parsedProxy.Credentials = new NetworkCredential(input.Username, input.Password);
            }

            return parsedProxy;
        }
    }
}
