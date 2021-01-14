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
        public IPEndPoint IPEndPoint { get; set; }
        public string Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public long? Speed { get; set; }
        private bool? isWorking = false;
        public string Status
        {
            get
            {
                switch (isWorking)
                {
                    case true:
                        return "Working";
                    case false:
                        return "Timed out";
                    default:
                        return "Unknown";
                }
            }
        }

        public Proxy(IPEndPoint endPoint, string type = null)
        {
            this.IPEndPoint = endPoint;
            this.Type = type;
        }
        public Proxy(IPEndPoint endPoint, string username, string password)
        {
            this.IPEndPoint = endPoint;
            this.Username = username;
            this.Password = password;
        }

        public void PerformTestA(string Url)
        {
            long result = Proxy.TestProxyA(this, Url);

            if (result > -1)
            {
                this.isWorking = true;
                this.Speed = result;
            }
            else
            {
                this.isWorking = false;
                this.Speed = -1;
            }

        }

        public void PerformTestB(string Url)
        {
            long result = Proxy.TestProxyB(this, Url);

            if (result > -1)
            {
                this.isWorking = true;
                this.Speed = result;
            }
            else
            {
                this.isWorking = false;
                this.Speed = -1;
            }

        }

        public static Proxy Parse(string str)
        {
            string[] parts = str.Split(':');

            try
            {
                string ipStr = parts[0];
                string portStr = parts[1];

                IPAddress ip;
                IPAddress.TryParse(ipStr, out ip);

                if (parts.Length > 2)
                {
                    string username = parts[2];
                    string password = parts[3];

                    return new Proxy(new IPEndPoint(ip, int.Parse(portStr)), username, password);
                }

                return new Proxy(new IPEndPoint(ip, int.Parse(portStr)));
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static long TestProxyA(Proxy proxy, string Url)
        {
            Stopwatch sw = new Stopwatch();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(Url));

            request.Proxy = new WebProxy(proxy.IPEndPoint.Address.ToString(), proxy.IPEndPoint.Port);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
            request.Timeout = 2000;
            request.ReadWriteTimeout = 100000;
            request.Method = "HEAD";

           

            if (!string.IsNullOrWhiteSpace(proxy.Username))
            {
                request.UseDefaultCredentials = false;
                request.Credentials = new NetworkCredential(proxy.Username, proxy.Password);
                request.Proxy.Credentials = new NetworkCredential(proxy.Username, proxy.Password);
            }

            try
            {
                sw.Start();
                WebResponse response = request.GetResponse();
                sw.Stop();
            }
            catch (Exception)
            {
                return -1;
            }
            return sw.ElapsedMilliseconds;

        }

        public static long TestProxyB(Proxy proxy, string Url)
        {
            Stopwatch sw = new Stopwatch();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(Url));

            request.Proxy = new WebProxy(proxy.IPEndPoint.Address.ToString(), proxy.IPEndPoint.Port);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
            request.Timeout = 2000;
            request.ReadWriteTimeout = 100000;

           

            if (!string.IsNullOrWhiteSpace(proxy.Username))
            {
                request.UseDefaultCredentials = false;
                request.Credentials = new NetworkCredential(proxy.Username, proxy.Password);
                request.Proxy.Credentials = new NetworkCredential(proxy.Username, proxy.Password);
            }

            try
            {
                sw.Start();
                WebResponse response = request.GetResponse();
                sw.Stop();
            }
            catch (Exception)
            {
                return -1;
            }
            return sw.ElapsedMilliseconds;
        }
    }
}
