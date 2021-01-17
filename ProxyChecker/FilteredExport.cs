using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxyFilter
{
    public partial class FilteredExport : Form
    {
        public FilteredExport()
        {
            InitializeComponent();
            tbSpeedFilter.Controls.RemoveAt(0);
        }

        public int GetData()
        {
            return (int)tbSpeedFilter.Value;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
