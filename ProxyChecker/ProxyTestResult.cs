using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyFilter
{
    public class ProxyTestResult
    {
        public string Status { get; set; }
        public int? Speed { get; set; }
        public ProxyTestResult(string status, int speed)
        {
            this.Status = status;
            this.Speed = speed;
        }
        public ProxyTestResult(string status)
        {
            this.Status = status;
        }
    }
}
