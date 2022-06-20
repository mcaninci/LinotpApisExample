using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinotpTokenDelete
{
    public partial class frmTokenDelete : Form
    {
        public frmTokenDelete()
        {
            InitializeComponent();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            string userName = txt_Username.Text;
            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("User name cannot be empty");
                return;

            }

            var res = LinotpApiHelper.SyncDeleteToken(userName);
            if (!string.IsNullOrEmpty(res.Item2))
               MessageBox.Show(res.Item2);
            return;
        }
    }
}
