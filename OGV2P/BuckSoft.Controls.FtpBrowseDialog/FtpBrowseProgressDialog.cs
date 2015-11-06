using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BuckSoft.Controls.FtpBrowseDialog
{
    public partial class FtpBrowseProgressDialog : Form
    {
        private bool cancelled;
        public FtpBrowseProgressDialog()
        {
            cancelled = false;
            InitializeComponent();
        }
        public bool Cancelled
        {
            get
            {
                return cancelled;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            cancelled = true;
        }

    }
}