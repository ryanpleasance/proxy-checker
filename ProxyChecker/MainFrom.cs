using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        DateTime startTime;

        public IList<Proxy> proxyList = new List<Proxy>();

        public MainFrom()
        {
            InitializeComponent();
            progressLabel.Text = String.Format("{0} / {1} Proxies Tested", proxyTested, proxyNum);
        }

        private void btnAddProxy_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;

                // Get input from user
                AddProxyForm form = new AddProxyForm();
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    proxyList = form.GetData();
                }

                // Populate Datagridview based off input
                foreach (Proxy p in proxyList)
                {
                    int pIndex = proxyList.IndexOf(p);

                    if (p.Username != null)
                    {
                        proxyList[pIndex].RowIndex = ProxyDataGridView.Rows.Add(p.Domain, p.Port, p.Username + ":" + p.Password, null, p.Status);
                    } else
                    {
                        proxyList[pIndex].RowIndex = ProxyDataGridView.Rows.Add(p.Domain, p.Port, "", null, p.Status);
                    }
                }
                this.Enabled = true;
                proxyNum = this.proxyList.Count;
                progressLabel.Text = String.Format("{0} / {1} Proxies Tested", proxyTested, proxyNum);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
                MessageBox.Show("Error 0! Error logged and sent off to be fixed");
            }

        }

        private void testProxyList(bool fast)
        {
            this.Enabled = false;
            try
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

                        ProxyTestResult result;
                        if (fast) result = ProxyTester.QuickTest(proxy, tbUrl.Text);
                        else result = ProxyTester.PageLoadTest(proxy, tbUrl.Text);

                        proxy.Speed = result.Speed;
                        proxy.Status = result.Status;

                        ++proxyTested;
                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            this.UpdateTick(proxy);
                            this.EstimateTimeLeft();
                        });
                    });
                });
                proxyTested = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 1! Error logged and sent off to be fixed");
                
            }
            this.Enabled = true;
        }

        private void MarkTesting(Proxy p)
        {
            try
            {
                DataGridViewRow row = ProxyDataGridView.Rows[p.RowIndex];
                row.Cells[4].Value = "Testing";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 2! Error logged and sent off to be fixed");
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProxyDataGridView.Rows.Clear();
                proxyList.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 3! Error logged and sent off to be fixed");
            }
        }

        private void UpdateTick(Proxy p)
        {
            try
            {
                DataGridViewRow row = ProxyDataGridView.Rows[p.RowIndex];
                if (p.Speed > 0)
                {
                    row.Cells[4].Value = p.Speed;
                }
                else
                {
                    row.Cells[4].Value = null;
                }
                row.Cells[5].Value = p.Status;
                progressLabel.Text = String.Format("{0} / {1} Proxies Tested", proxyTested, proxyNum);

                if (proxyTested > 0)
                {
                    progressBar.Value = Convert.ToInt16(proxyTested * 100.0 / proxyNum);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 4! Error logged and sent off to be fixed");
            }
        }

        private void btnStartTestB_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            testProxyList(false);
        }

        private void btnStartTestA_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            testProxyList(true);
        }

        private void ProxyDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            try
            {
                proxyList.Clear();

                foreach (DataGridViewRow row in ProxyDataGridView.Rows)
                {
                    string ip = row.Cells[0].Value.ToString();
                    string port = row.Cells[1].Value.ToString();

                    if (string.IsNullOrWhiteSpace(row.Cells[2].Value.ToString())) // not auth
                    {
                        proxyList.Add(new Proxy(row.Cells[0].Value.ToString(), Convert.ToInt32(row.Cells[1].Value.ToString()), row.Cells[2].Value.ToString(), row.Cells[3].Value.ToString()));
                    }
                    else
                    {
                        proxyList.Add(new Proxy(row.Cells[0].Value.ToString(), Convert.ToInt32(row.Cells[1].Value.ToString())));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 6! Error logged and sent off to be fixed");
            }
        }

        private void ProxyDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (this.ProxyDataGridView.Columns[e.ColumnIndex].Name == "SPEED")
                {
                    e.Value = e.Value?.ToString().Replace(" ms", "");
                    e.CellStyle.Font = new Font(e.CellStyle.Font.FontFamily, 12F);

                    if (!string.IsNullOrWhiteSpace(e.Value?.ToString()))
                    {
                        long value;
                        long.TryParse(e.Value.ToString(), out value);

                        if (value < 300)
                        {
                            e.CellStyle.ForeColor = Color.DarkGreen;
                        }
                        else if (value < 1000)
                        {
                            e.CellStyle.ForeColor = Color.Orange;
                        } 
                        else
                        {
                            e.CellStyle.ForeColor = Color.Red;
                        }

                        if (!string.IsNullOrWhiteSpace(e.Value?.ToString()))
                        {
                            e.Value += " ms";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 7! Error logged and sent off to be fixed");
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                int speedFilter = 0;

                FilteredExport export = new FilteredExport();
                export.ShowDialog();

                if (export.DialogResult == DialogResult.OK)
                {
                    speedFilter = export.GetData();
                }
                else return;

                List<string> exportProxies = new List<string>();

                foreach (DataGridViewRow row in ProxyDataGridView.Rows)
                {
                    if (row.Cells[5].Value.ToString() == "Working")
                    {
                        double speed = Convert.ToDouble(row.Cells[4].Value.ToString());

                        if (speed <= speedFilter)
                        {
                            string ip = row.Cells[0].Value.ToString();
                            string port = row.Cells[1].Value.ToString();
                            string user = "";
                            string pass = "";

                            if (!string.IsNullOrWhiteSpace(row.Cells[2].Value.ToString())) // not auth
                            {
                                user = row.Cells[2].Value.ToString();
                                pass = row.Cells[3].Value.ToString();

                                exportProxies.Add(ip + ":" + port + ":" + user + ":" + pass);
                            }
                            else
                            {
                                exportProxies.Add(ip + ":" + port);
                            }
                        }
                    }
                }

                ExportToFile(exportProxies);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void ExportToFile(List<string> input)
        {
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                save.FileName = "FilteredProxies.txt";
                save.Filter = "Text File | *.txt";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter writer = new StreamWriter(save.OpenFile());

                    foreach (string p in input)
                    {
                        writer.WriteLine(p);
                    }

                    writer.Dispose();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error 9! Error logged and sent off to be fixed");
            }
        }

        private void EstimateTimeLeft()
        {
            if (proxyNum != proxyTested)
            {
                DateTime timeNow = DateTime.Now;
                double elapsedTime = (timeNow - startTime).TotalSeconds;

                double timeLeft = (elapsedTime / proxyTested) * (proxyNum - proxyTested);

                progressLabel.Text += string.Format(" | Estimated {0}s left", Math.Round(timeLeft, 0));
            }
        }
    }
 }

