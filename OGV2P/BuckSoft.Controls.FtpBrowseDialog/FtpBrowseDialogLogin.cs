using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BuckSoft.Controls.FtpBrowseDialog
{
    public partial class FtpBrowseDialogLogin : Form
    {
        #region Constructors
        public FtpBrowseDialogLogin()
        {
            InitializeComponent();
            Port = 21;
            PassiveMode = true;
        }
        public FtpBrowseDialogLogin(String host, String path, int port, String username, String password, bool passivemode)
        {
            InitializeComponent();
            Server = host;
            Port = port;
            Username = username;
            Password = password;
            PassiveMode = passivemode;
        }
        #endregion

        #region Public Properties
        public String Server
        {
            get
            {
                return server.Text;
            }
            set
            {
                server.Text = value;
            }
        }
        public String StartPath
        {
            get
            {
                return startpath.Text;
            }
            set
            {
                startpath.Text = value;
            }
        }
        public String Username
        {
            get
            {
                return username.Text;
            }
            set
            {
                username.Text = value;
            }
        }
        public String Password
        {
            get
            {
                return password.Text;
            }
            set
            {
                password.Text = value;
            }
        }
        public int Port
        {
            get
            {
                return System.Convert.ToInt32(port.Value);
            }
            set
            {
                port.Value = new Decimal(value);
            }
        }
        public bool PassiveMode
        {
            get
            {
                return passivecheckbox.Checked;
            }
            set
            {
                passivecheckbox.Checked = value;
            }
        }
        #endregion

        #region Event Handlers
        private void input_TextChanged(object sender, EventArgs e)
        {
            Uri testuri;
            if (Uri.TryCreate("ftp://" + Server, UriKind.Absolute, out testuri) == false)
            {
                connectbutton.Enabled = false;
                return;
            }
            if (username.Text == String.Empty)
            {
                connectbutton.Enabled = false;
                return;
            }
            if (password.Text == String.Empty)
            {
                connectbutton.Enabled = false;
                return;
            }
            connectbutton.Enabled = true;
        }
        private void FtpBrowseDialogLogin_Load(object sender, EventArgs e)
        {
            Uri hosturi;
            if (Uri.TryCreate("ftp://" + Server, UriKind.Absolute, out hosturi) == false)
            {
                connectbutton.Enabled = false;
                return;
            }
            if (username.Text == String.Empty)
            {
                connectbutton.Enabled = false;
                return;
            }
            if (password.Text == String.Empty)
            {
                connectbutton.Enabled = false;
                return;
            }
            connectbutton.Enabled = true;
        }
        #endregion
    }
}