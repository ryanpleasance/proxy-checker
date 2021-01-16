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

namespace ProxyChecker
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

        private void btnStorno_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public IList<Proxy> GetData()
        {
            IList<Proxy> proxyList = new List<Proxy>();

            using (StringReader reader = new StringReader(this.proxyTextBox.Text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        string[] parts = line.Split(':');

                        if (parts.Length > 2)
                        {
                            proxyList.Add(new Proxy(parts[0], Convert.ToInt32(parts[1]), parts[2], parts[3]));
                        }
                        else
                        {
                            proxyList.Add(new Proxy(parts[0], Convert.ToInt32(parts[1])));
                        }
                    }
                    catch (Exception ex)
                    {
                        // ignored
                    }
                }
            }
            return proxyList;
        }

        private void proxyTextBox_Enter(object sender, EventArgs e)
        {
            if (proxyTextBox.Text == "IP:PORT:USERNAME:PASSWORD")
            {
                proxyTextBox.Text = "";
            }
        }

        private void proxyTextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(proxyTextBox.Text))
                proxyTextBox.Text = "IP:PORT:USERNAME:PASSWORD";
        }
    }
}
