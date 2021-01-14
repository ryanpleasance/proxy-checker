using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxyChecker
{
    public partial class MainFrom : Form
    {
        int proxyTested = 0;
        int proxyNum = 0;

        public IList<Proxy> proxyList = new List<Proxy>();

        public MainFrom()
        {
            InitializeComponent();
            progressLabel.Text = String.Format("{0} / {1} Proxies Tested", proxyTested, proxyNum);
        }

        private void btnAddProxy_Click(object sender, EventArgs e)
        {
            AddProxyForm form = new AddProxyForm();
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK)
            {
                proxyList = form.GetData();
            }

            foreach (Proxy p in proxyList)
            {
                int pIndex = proxyList.IndexOf(p);
                proxyList[pIndex].RowIndex = ProxyDataGridView.Rows.Add(p.IPEndPoint.Address, p.IPEndPoint.Port, p.Username + ":" + p.Password, p.Speed, p.Status);
            }
            proxyNum = this.proxyList.Count;
            progressLabel.Text = String.Format("{0} / {1} Proxies Tested", proxyTested, proxyNum);
        }

        private void testProxyList(bool fast)
        {
            proxyNum = this.proxyList.Count;
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(this.proxyList, new ParallelOptions() { MaxDegreeOfParallelism = 32 }, proxy =>
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        this.MarkTesting(proxy);
                    });
                    
                    if (fast) proxy.PerformTestA(tbUrl.Text);
                    else proxy.PerformTestB(tbUrl.Text);

                    ++proxyTested;
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        this.UpdateTick(proxy);
                    });
                });
            });
            proxyTested = 0;
        }

        private void MarkTesting(Proxy p)
        {
            DataGridViewRow row = ProxyDataGridView.Rows[p.RowIndex];
            row.Cells[4].Value = "Testing";
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            this.ProxyDataGridView.Rows.Clear();
            proxyList.Clear();
        }

        private void UpdateTick(Proxy p)
        {
            UpdateTable(p);
            if (proxyTested > 0)
            {
                progressBar.Value = Convert.ToInt16(proxyTested * 100.0 / proxyNum);
            }
        }
        private void UpdateTable(Proxy p)
        {
            DataGridViewRow row = ProxyDataGridView.Rows[p.RowIndex];
            if (p.Speed > 0)
            {
                row.Cells[3].Value = p.Speed + " ms";
            } else
            {
                row.Cells[3].Value = "";
            }
            row.Cells[4].Value = p.Status;
            progressLabel.Text = String.Format("{0} / {1} Proxies Tested", proxyTested, proxyNum);
        }

        private void btnStartTestB_Click(object sender, EventArgs e)
        {
            testProxyList(false);
        }

        private void btnStartTestA_Click(object sender, EventArgs e)
        {
            testProxyList(true);
        }

        private void ProxyDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            proxyList.Clear();

            foreach (DataGridViewRow row in ProxyDataGridView.Rows)
            {
                string ip = row.Cells[0].Value.ToString();
                string port = row.Cells[1].Value.ToString();
                    
                if (string.IsNullOrWhiteSpace(row.Cells[2].Value.ToString())) // not auth
                {
                    string auth = row.Cells[2].Value.ToString();

                    proxyList.Add(Proxy.Parse(ip + ":" + port + ":" + auth));
                } else
                {
                    proxyList.Add(Proxy.Parse(ip + ":" + port));
                }
            }
        }
    }
}
