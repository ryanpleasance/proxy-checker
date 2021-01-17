using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxyFilter
{
    public partial class MainFrom : Form
    {
        public IList<Proxy> ProxyList = new List<Proxy>();
        private bool _isWorking;
        private int _proxyNum;
        private int _proxyTested;
        private DateTime _startTime;

        public MainFrom()
        {
            InitializeComponent();
            progressLabel.Text = $@"{_proxyTested} / {_proxyNum} Proxies Tested";
        }

        private void btnAddProxy_Click(object sender, EventArgs e)
        {
            if (_isWorking) return;
            try
            {
                _isWorking = true;
                ProxyDataGridView.Enabled = false;

                // Get input from user
                var form = new AddProxyForm();
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK) ProxyList = form.GetData();

                // Populate Datagridview based off input
                foreach (var p in ProxyList)
                {
                    var pIndex = ProxyList.IndexOf(p);

                    if (p.Username != null)
                        ProxyList[pIndex].RowIndex = ProxyDataGridView.Rows.Add(p.Domain, p.Port, p.Username,
                            p.Password, null, p.Status);
                    else
                        ProxyList[pIndex].RowIndex =
                            ProxyDataGridView.Rows.Add(p.Domain, p.Port, "", "", null, p.Status);
                }

                ProxyDataGridView.Enabled = true;
                _proxyNum = ProxyList.Count;
                progressLabel.Text = $@"{_proxyTested} / {_proxyNum} Proxies Tested";
                _isWorking = false;
            }
            catch (Exception)
            {
                MessageBox.Show(@"Error 0! Error logged and sent off to be fixed");
                _isWorking = false;
            }
        }

        private async Task TestProxyList(bool fast)
        {
            ProxyDataGridView.Enabled = false;
            Application.DoEvents();
            try
            {
                _proxyNum = ProxyList.Count;

                await Task.Factory.StartNew(() =>
                {
                    Parallel.ForEach(ProxyList, new ParallelOptions {MaxDegreeOfParallelism = 32}, proxy =>
                    {
                        BeginInvoke((MethodInvoker) delegate { MarkTesting(proxy); });

                        var result = fast ? ProxyTester.QuickTest(proxy, tbUrl.Text) : ProxyTester.PageLoadTest(proxy, tbUrl.Text);

                        proxy.Speed = result.Speed;
                        proxy.Status = result.Status;

                        ++_proxyTested;
                        BeginInvoke((MethodInvoker) delegate
                        {
                            UpdateTick(proxy);
                            EstimateTimeLeft();
                        });
                    });
                });
                _proxyTested = 0;
            }
            catch (Exception)
            {
                MessageBox.Show(@"Error 1! Error logged and sent off to be fixed");
            }

            ProxyDataGridView.Enabled = true;
        }

        private void MarkTesting(Proxy p)
        {
            try
            {
                var row = ProxyDataGridView.Rows[p.RowIndex];
                row.Cells[4].Value = "Testing";
            }
            catch (Exception)
            {
                MessageBox.Show(@"Error 2! Error logged and sent off to be fixed");
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            if (_isWorking) return;
            try
            {
                _isWorking = true;
                ProxyDataGridView.Rows.Clear();
                ProxyList.Clear();
                _isWorking = false;
            }
            catch (Exception)
            {
                MessageBox.Show(@"Error 3! Error logged and sent off to be fixed");
                _isWorking = false;
            }
        }

        private void UpdateTick(Proxy p)
        {
            try
            {
                var row = ProxyDataGridView.Rows[p.RowIndex];
                row.Cells[4].Value = p.Speed > 0 ? p.Speed : null;
                row.Cells[5].Value = p.Status;
                progressLabel.Text = $@"{_proxyTested} / {_proxyNum} Proxies Tested";

                if (_proxyTested > 0) progressBar.Value = Convert.ToInt16(_proxyTested * 100.0 / _proxyNum);
            }
            catch (Exception)
            {
                MessageBox.Show(@"Error 4! Error logged and sent off to be fixed");
            }
        }

        private async void btnStartTestB_Click(object sender, EventArgs e)
        {
            if (_isWorking) return;
            _isWorking = true;
            _startTime = DateTime.Now;
            await TestProxyList(false);
            _isWorking = false;
            progressLabel.Text = $@"{_proxyNum} / {_proxyNum} Proxies Tested";
            progressBar.Value = progressBar.Maximum;
        }

        private async void btnStartTestA_Click(object sender, EventArgs e)
        {
            if (_isWorking) return;

            _isWorking = true;
            _startTime = DateTime.Now;
            await TestProxyList(true);
            _isWorking = false;
            progressLabel.Text = $@"{_proxyNum} / {_proxyNum} Proxies Tested";
            progressBar.Value = progressBar.Maximum;
        }

        private void ProxyDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            try
            {
                ProxyList.Clear();

                foreach (DataGridViewRow row in ProxyDataGridView.Rows)
                {
                    if (string.IsNullOrWhiteSpace(row.Cells[2].Value.ToString()))
                    {
                        ProxyList.Add(new Proxy(row.Cells[0].Value.ToString(),
                            Convert.ToInt32(row.Cells[1].Value.ToString()),
                            row.Cells[2].Value.ToString(),
                            row.Cells[3].Value.ToString()));
                    }
                    else
                    {
                        ProxyList.Add(new Proxy(row.Cells[0].Value.ToString(),
                            Convert.ToInt32(row.Cells[1].Value.ToString())));
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(@"Error 6! Error logged and sent off to be fixed");
            }
        }

        private void ProxyDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (ProxyDataGridView.Columns[e.ColumnIndex].Name != "SPEED") return;

                e.Value = e.Value?.ToString().Replace(" ms", "");
                e.CellStyle.Font = new Font(e.CellStyle.Font.FontFamily, 12F);

                if (!string.IsNullOrWhiteSpace(e.Value?.ToString()))
                {
                    long.TryParse(e.Value.ToString(), out var value);

                    if (value < 300)
                        e.CellStyle.ForeColor = Color.DarkGreen;
                    else if (value < 1000)
                        e.CellStyle.ForeColor = Color.Orange;
                    else
                        e.CellStyle.ForeColor = Color.Red;

                    if (!string.IsNullOrWhiteSpace(e.Value?.ToString())) e.Value += " ms";
                }
            }
            catch (Exception)
            {
                MessageBox.Show(@"Error 7! Error logged and sent off to be fixed");
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_isWorking) return;

            try
            {
                _isWorking = true;
                var speedFilter = 0;

                var export = new FilteredExport();
                export.ShowDialog();

                if (export.DialogResult == DialogResult.OK)
                {
                    speedFilter = export.GetData();
                }
                else
                {
                    _isWorking = false;
                    return;
                }

                var exportProxies = new List<string>();

                foreach (DataGridViewRow row in ProxyDataGridView.Rows)
                    if (row.Cells[5].Value.ToString() == "Working")
                    {
                        var speed = Convert.ToDouble(row.Cells[4].Value.ToString());

                        if (!(speed <= speedFilter)) continue;

                        var ip = row.Cells[0].Value.ToString();
                        var port = row.Cells[1].Value.ToString();

                        if (!string.IsNullOrWhiteSpace(row.Cells[2].Value.ToString())) // not auth
                        {
                            var user = row.Cells[2].Value.ToString();
                            var pass = row.Cells[3].Value.ToString();

                            exportProxies.Add(ip + ":" + port + ":" + user + ":" + pass);
                        }
                        else
                        {
                            exportProxies.Add(ip + ":" + port);
                        }
                    }

                _isWorking = false;
                ExportToFile(exportProxies);
            }
            catch (Exception)
            {
                MessageBox.Show(@"Error...");
                _isWorking = false;
            }
        }

        private static void ExportToFile(IEnumerable<string> input)
        {
            try
            {
                var save = new SaveFileDialog {FileName = "FilteredProxies.txt", Filter = @"Text File | *.txt"};

                if (save.ShowDialog() != DialogResult.OK) return;

                var writer = new StreamWriter(save.OpenFile());

                foreach (var p in input) writer.WriteLine(p);

                writer.Dispose();
                writer.Close();
            }
            catch (Exception)
            {
                MessageBox.Show(@"Error 9! Error logged and sent off to be fixed");
            }
        }

        private void EstimateTimeLeft()
        {
            if (_proxyNum == _proxyTested) return;

            var timeNow = DateTime.Now;
            var elapsedTime = (timeNow - _startTime).TotalSeconds;

            var timeLeft = elapsedTime / _proxyTested * (_proxyNum - _proxyTested);

            progressLabel.Text += $@" | Estimated {Math.Round(timeLeft, 0)}s left";
        }
    }
}