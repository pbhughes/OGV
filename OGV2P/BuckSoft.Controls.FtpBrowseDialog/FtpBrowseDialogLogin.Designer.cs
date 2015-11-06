namespace BuckSoft.Controls.FtpBrowseDialog
{
    partial class FtpBrowseDialogLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.username = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.server = new System.Windows.Forms.TextBox();
            this.passivecheckbox = new System.Windows.Forms.CheckBox();
            this.portlabel = new System.Windows.Forms.Label();
            this.connectbutton = new System.Windows.Forms.Button();
            this.cancelbutton = new System.Windows.Forms.Button();
            this.port = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.startpath = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.port)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Username";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Password";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(83, 108);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(100, 20);
            this.username.TabIndex = 2;
            this.username.TextChanged += new System.EventHandler(this.input_TextChanged);
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(83, 138);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(100, 20);
            this.password.TabIndex = 3;
            this.password.TextChanged += new System.EventHandler(this.input_TextChanged);
            // 
            // server
            // 
            this.server.Location = new System.Drawing.Point(83, 18);
            this.server.Name = "server";
            this.server.Size = new System.Drawing.Size(307, 20);
            this.server.TabIndex = 0;
            this.server.TextChanged += new System.EventHandler(this.input_TextChanged);
            // 
            // passivecheckbox
            // 
            this.passivecheckbox.AutoSize = true;
            this.passivecheckbox.Checked = true;
            this.passivecheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.passivecheckbox.Location = new System.Drawing.Point(83, 168);
            this.passivecheckbox.Name = "passivecheckbox";
            this.passivecheckbox.Size = new System.Drawing.Size(93, 17);
            this.passivecheckbox.TabIndex = 4;
            this.passivecheckbox.Text = "Passive Mode";
            this.passivecheckbox.UseVisualStyleBackColor = true;
            // 
            // portlabel
            // 
            this.portlabel.AutoSize = true;
            this.portlabel.Location = new System.Drawing.Point(22, 80);
            this.portlabel.Name = "portlabel";
            this.portlabel.Size = new System.Drawing.Size(26, 13);
            this.portlabel.TabIndex = 7;
            this.portlabel.Text = "Port";
            // 
            // connectbutton
            // 
            this.connectbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.connectbutton.Enabled = false;
            this.connectbutton.Location = new System.Drawing.Point(315, 164);
            this.connectbutton.Name = "connectbutton";
            this.connectbutton.Size = new System.Drawing.Size(75, 23);
            this.connectbutton.TabIndex = 5;
            this.connectbutton.Text = "Connect";
            this.connectbutton.UseVisualStyleBackColor = true;
            // 
            // cancelbutton
            // 
            this.cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelbutton.Location = new System.Drawing.Point(234, 164);
            this.cancelbutton.Name = "cancelbutton";
            this.cancelbutton.Size = new System.Drawing.Size(75, 23);
            this.cancelbutton.TabIndex = 6;
            this.cancelbutton.Text = "Cancel";
            this.cancelbutton.UseVisualStyleBackColor = true;
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(83, 78);
            this.port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(65, 20);
            this.port.TabIndex = 8;
            this.port.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.port.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Start Path";
            // 
            // startpath
            // 
            this.startpath.Location = new System.Drawing.Point(83, 48);
            this.startpath.Name = "startpath";
            this.startpath.Size = new System.Drawing.Size(226, 20);
            this.startpath.TabIndex = 10;
            this.startpath.TextChanged += new System.EventHandler(this.input_TextChanged);
            // 
            // FtpBrowseDialogLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 203);
            this.ControlBox = false;
            this.Controls.Add(this.startpath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.port);
            this.Controls.Add(this.cancelbutton);
            this.Controls.Add(this.connectbutton);
            this.Controls.Add(this.portlabel);
            this.Controls.Add(this.passivecheckbox);
            this.Controls.Add(this.server);
            this.Controls.Add(this.password);
            this.Controls.Add(this.username);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FtpBrowseDialogLogin";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.FtpBrowseDialogLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.port)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.TextBox server;
        private System.Windows.Forms.CheckBox passivecheckbox;
        private System.Windows.Forms.Label portlabel;
        private System.Windows.Forms.Button connectbutton;
        private System.Windows.Forms.Button cancelbutton;
        private System.Windows.Forms.NumericUpDown port;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox startpath;
    }
}