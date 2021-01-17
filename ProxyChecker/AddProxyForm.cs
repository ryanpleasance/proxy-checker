using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxyFilter
{
    public partial class AddProxyForm : Form
    {
        public AddProxyForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        public IList<Proxy> GetData()
        {
            IList<Proxy> proxyList = new List<Proxy>();

            using (var reader = new StringReader(this.proxyTextBox.Text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        var parts = line.Split(':');

                        proxyList.Add(parts.Length > 2
                            ? new Proxy(parts[0], Convert.ToInt32(parts[1]), parts[2], parts[3])
                            : new Proxy(parts[0], Convert.ToInt32(parts[1])));
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            return proxyList;
        }

        private void proxyTextBox_Enter(object sender, EventArgs e)
        {
            if (proxyTextBox.Text == @"IP:PORT:USERNAME:PASSWORD")
            {
                proxyTextBox.Text = "";
            }
        }

        private void proxyTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(proxyTextBox.Text))
                proxyTextBox.Text = @"IP:PORT:USERNAME:PASSWORD";
        }
    }
}
