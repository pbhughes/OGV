namespace BuckSoft.Controls.FtpBrowseDialog
{
    partial class FtpBrowseDialog
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cancelbutton = new System.Windows.Forms.Button();
            this.okbutton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.directorytree = new System.Windows.Forms.TreeView();
            this.filelist = new System.Windows.Forms.ListView();
            this.loadnewhostbutton = new System.Windows.Forms.ToolStripButton();
            this.updirectorybutton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updirectorybutton,
            this.loadnewhostbutton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(903, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelbutton);
            this.panel1.Controls.Add(this.okbutton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 621);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(903, 29);
            this.panel1.TabIndex = 1;
            // 
            // cancelbutton
            // 
            this.cancelbutton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelbutton.Location = new System.Drawing.Point(735, 3);
            this.cancelbutton.Name = "cancelbutton";
            this.cancelbutton.Size = new System.Drawing.Size(75, 23);
            this.cancelbutton.TabIndex = 1;
            this.cancelbutton.Text = "Cancel";
            this.cancelbutton.UseVisualStyleBackColor = true;
            // 
            // okbutton
            // 
            this.okbutton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.okbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okbutton.Enabled = false;
            this.okbutton.Location = new System.Drawing.Point(816, 3);
            this.okbutton.Name = "okbutton";
            this.okbutton.Size = new System.Drawing.Size(75, 23);
            this.okbutton.TabIndex = 0;
            this.okbutton.Text = "Open";
            this.okbutton.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.directorytree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.filelist);
            this.splitContainer1.Size = new System.Drawing.Size(903, 596);
            this.splitContainer1.SplitterDistance = 301;
            this.splitContainer1.TabIndex = 2;
            this.splitContainer1.TabStop = false;
            // 
            // directorytree
            // 
            this.directorytree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directorytree.Location = new System.Drawing.Point(0, 0);
            this.directorytree.Name = "directorytree";
            this.directorytree.PathSeparator = "/";
            this.directorytree.ShowLines = false;
            this.directorytree.ShowRootLines = false;
            this.directorytree.Size = new System.Drawing.Size(301, 596);
            this.directorytree.TabIndex = 0;
            this.directorytree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.directorytree_BeforeExpand);
            this.directorytree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.directorytree_BeforeCollapse);
            this.directorytree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.directorytree_AfterSelect);
            this.directorytree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.directorytree_NodeMouseClick);
            // 
            // filelist
            // 
            this.filelist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filelist.Location = new System.Drawing.Point(0, 0);
            this.filelist.MultiSelect = false;
            this.filelist.Name = "filelist";
            this.filelist.Size = new System.Drawing.Size(598, 596);
            this.filelist.TabIndex = 0;
            this.filelist.UseCompatibleStateImageBehavior = false;
            this.filelist.View = System.Windows.Forms.View.List;
            this.filelist.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.filelist_MouseDoubleClick);
            this.filelist.SelectedIndexChanged += new System.EventHandler(this.filelist_SelectedIndexChanged);
            this.filelist.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.filelist_ItemSelectionChanged);
            this.filelist.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filelist_KeyPress);
            // 
            // loadnewhostbutton
            // 
            this.loadnewhostbutton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.loadnewhostbutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadnewhostbutton.Image = global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.openHS;
            this.loadnewhostbutton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadnewhostbutton.Name = "loadnewhostbutton";
            this.loadnewhostbutton.Size = new System.Drawing.Size(23, 22);
            this.loadnewhostbutton.Text = "toolStripButton1";
            this.loadnewhostbutton.ToolTipText = "Open New Server";
            this.loadnewhostbutton.Click += new System.EventHandler(this.loadnewhostbutton_Click);
            // 
            // updirectorybutton
            // 
            this.updirectorybutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.updirectorybutton.Enabled = false;
            this.updirectorybutton.Image = global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.GoToParentFolderHS;
            this.updirectorybutton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.updirectorybutton.Name = "updirectorybutton";
            this.updirectorybutton.Size = new System.Drawing.Size(23, 22);
            this.updirectorybutton.Text = "toolStripButton1";
            this.updirectorybutton.Click += new System.EventHandler(this.updirectorybutton_Click);
            // 
            // FtpBrowseDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 650);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FtpBrowseDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FtpBrowseDialog";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelbutton;
        private System.Windows.Forms.Button okbutton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView directorytree;
        private System.Windows.Forms.ListView filelist;
        private System.Windows.Forms.ToolStripButton loadnewhostbutton;
        private System.Windows.Forms.ToolStripButton updirectorybutton;
    }
}