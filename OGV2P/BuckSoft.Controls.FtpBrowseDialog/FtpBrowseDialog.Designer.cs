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
            this.updirectorybutton = new System.Windows.Forms.ToolStripButton();
            this.loadnewhostbutton = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cancelbutton = new System.Windows.Forms.Button();
            this.okbutton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.directorytree = new System.Windows.Forms.TreeView();
            this.filelist = new System.Windows.Forms.ListView();
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
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updirectorybutton,
            this.loadnewhostbutton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1204, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // updirectorybutton
            // 
            this.updirectorybutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.updirectorybutton.Enabled = false;
            this.updirectorybutton.Image = global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.GoToParentFolderHS;
            this.updirectorybutton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.updirectorybutton.Name = "updirectorybutton";
            this.updirectorybutton.Size = new System.Drawing.Size(24, 24);
            this.updirectorybutton.Text = "toolStripButton1";
            this.updirectorybutton.Click += new System.EventHandler(this.updirectorybutton_Click);
            // 
            // loadnewhostbutton
            // 
            this.loadnewhostbutton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.loadnewhostbutton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadnewhostbutton.Image = global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.openHS;
            this.loadnewhostbutton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.loadnewhostbutton.Name = "loadnewhostbutton";
            this.loadnewhostbutton.Size = new System.Drawing.Size(24, 24);
            this.loadnewhostbutton.Text = "toolStripButton1";
            this.loadnewhostbutton.ToolTipText = "Open New Server";
            this.loadnewhostbutton.Click += new System.EventHandler(this.loadnewhostbutton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelbutton);
            this.panel1.Controls.Add(this.okbutton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 365);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1204, 36);
            this.panel1.TabIndex = 1;
            // 
            // cancelbutton
            // 
            this.cancelbutton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelbutton.Location = new System.Drawing.Point(980, 4);
            this.cancelbutton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cancelbutton.Name = "cancelbutton";
            this.cancelbutton.Size = new System.Drawing.Size(100, 28);
            this.cancelbutton.TabIndex = 1;
            this.cancelbutton.Text = "Cancel";
            this.cancelbutton.UseVisualStyleBackColor = true;
            // 
            // okbutton
            // 
            this.okbutton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.okbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okbutton.Enabled = false;
            this.okbutton.Location = new System.Drawing.Point(1088, 4);
            this.okbutton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.okbutton.Name = "okbutton";
            this.okbutton.Size = new System.Drawing.Size(100, 28);
            this.okbutton.TabIndex = 0;
            this.okbutton.Text = "Open";
            this.okbutton.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.directorytree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.filelist);
            this.splitContainer1.Size = new System.Drawing.Size(1204, 338);
            this.splitContainer1.SplitterDistance = 401;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            this.splitContainer1.TabStop = false;
            // 
            // directorytree
            // 
            this.directorytree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directorytree.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directorytree.Location = new System.Drawing.Point(0, 0);
            this.directorytree.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.directorytree.Name = "directorytree";
            this.directorytree.PathSeparator = "/";
            this.directorytree.ShowLines = false;
            this.directorytree.ShowRootLines = false;
            this.directorytree.Size = new System.Drawing.Size(401, 338);
            this.directorytree.TabIndex = 0;
            this.directorytree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.directorytree_BeforeCollapse);
            this.directorytree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.directorytree_BeforeExpand);
            this.directorytree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.directorytree_AfterSelect);
            this.directorytree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.directorytree_NodeMouseClick);
            // 
            // filelist
            // 
            this.filelist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filelist.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filelist.Location = new System.Drawing.Point(0, 0);
            this.filelist.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.filelist.MultiSelect = false;
            this.filelist.Name = "filelist";
            this.filelist.Size = new System.Drawing.Size(798, 338);
            this.filelist.TabIndex = 0;
            this.filelist.UseCompatibleStateImageBehavior = false;
            this.filelist.View = System.Windows.Forms.View.List;
            this.filelist.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.filelist_ItemSelectionChanged);
            this.filelist.SelectedIndexChanged += new System.EventHandler(this.filelist_SelectedIndexChanged);
            this.filelist.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.filelist_KeyPress);
            this.filelist.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.filelist_MouseDoubleClick);
            // 
            // FtpBrowseDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 401);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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