using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyFilter
{
    internal class ProxyTester
    {
        public static ProxyTestResult QuickTest(Proxy proxy, string Url)
        {
            Stopwatch sw = new Stopwatch();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(Url));

            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
            request.Timeout = 5000;
            request.ReadWriteTimeout = 10000;
            request.Method = "HEAD";

            if (!string.IsNullOrWhiteSpace(proxy.Username))
            {
                request.UseDefaultCredentials = false;
                request.Credentials = new NetworkCredential(proxy.Username, proxy.Password);
            }

            try
            {
                request.Proxy = Proxy.Parse(proxy);

                sw.Start();
                request.GetResponse();
                sw.Stop();
            }
            catch (Exception)
            {
                return new ProxyTestResult("Timed out");
            }
            return new ProxyTestResult("Working", Convert.ToInt32(sw.ElapsedMilliseconds));
        }

        public static ProxyTestResult PageLoadTest(Proxy proxy, string Url)
        {
            var sw = new Stopwatch();

            var request = (HttpWebRequest)WebRequest.Create(new Uri(Url));

            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
            request.Timeout = 5000;
            request.ReadWriteTimeout = 10000;

            if (!string.IsNullOrWhiteSpace(proxy.Username))
            {
                request.UseDefaultCredentials = false;
                request.Credentials = new NetworkCredential(proxy.Username, proxy.Password);
            }

            try
            {
                request.Proxy = Proxy.Parse(proxy);

                sw.Start();
                request.GetResponse();
                sw.Stop();
            }
            catch (Exception)
            {
                return new ProxyTestResult("Timed out");
            }
            return new ProxyTestResult("Working", Convert.ToInt32(sw.ElapsedMilliseconds));
        }
    }
}
