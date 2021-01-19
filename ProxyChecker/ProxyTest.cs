using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProxyFilter
{
    internal class ProxyTester
    {
        public static async Task<ProxyTestResult> TestProxy(Proxy proxy, string Url, bool dataSave)
        {
            var sw = new Stopwatch();

            try
            {
                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = Proxy.Parse(proxy)
                };

                var client = new HttpClient(handler: httpClientHandler, disposeHandler: true)
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(Url),
                    Method = dataSave ? HttpMethod.Head : HttpMethod.Get
                };


                sw.Start();
                var response = await client.SendAsync(request);
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
