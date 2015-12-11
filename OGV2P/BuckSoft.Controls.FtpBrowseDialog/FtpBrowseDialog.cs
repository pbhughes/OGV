using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace BuckSoft.Controls.FtpBrowseDialog
{
    public partial class FtpBrowseDialog : Form
    {
        #region Private Fields
        private String path, startpath, file, host;
        private int ftpport;
        private BackgroundWorker fsloader;
        private FtpBrowseProgressDialog progress;
        private FtpWebRequest req;
        private FtpWebResponse rsp;
        private NetworkCredential ftpcred;
        private bool pasv;
        private bool testmode;
        private bool promptforserver;
        #endregion

        #region Constructors
        public FtpBrowseDialog(String hosturl, String path, int port, String username, String password, bool passivemode)
        {
            InitializeComponent();
            host = hosturl;
            startpath = path;
            ftpport = port;
            ftpcred = new NetworkCredential(username, password);
            pasv = passivemode;
            ImageList directoryimages = new ImageList();
            directoryimages.Images.Add(global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.CLSDFOLD);
            directoryimages.Images.Add(global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.OPENFOLD);
            directorytree.ImageList = directoryimages;
            directorytree.ImageIndex = 0;
            directorytree.SelectedImageIndex = 0;
            ImageList fileimages = new ImageList();
            fileimages.Images.Add(global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.CLSDFOLD);
            fileimages.Images.Add(global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.DOC);
            filelist.SmallImageList = fileimages;
            promptforserver = false;
         

        }

        public FtpBrowseDialog()
        {
            InitializeComponent();
            host = String.Empty;
            startpath = String.Empty;
            ftpport = 21;
            ftpcred = new NetworkCredential(String.Empty, String.Empty);
            pasv = true;
            ImageList directoryimages = new ImageList();
            directoryimages.Images.Add(global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.CLSDFOLD);
            directoryimages.Images.Add(global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.OPENFOLD);
            directorytree.ImageList = directoryimages;
            directorytree.ImageIndex = 0;
            directorytree.SelectedImageIndex = 0;
            ImageList fileimages = new ImageList();
            fileimages.Images.Add(global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.CLSDFOLD);
            fileimages.Images.Add(global::BuckSoft.Controls.FtpBrowseDialog.Properties.Resources.DOC);
            filelist.SmallImageList = fileimages;
            promptforserver = true;
            
        }
        #endregion

        #region Public Properties
        public bool TestMode
        {
            get
            {
                return testmode;
            }
            set
            {
                testmode = value;
            }
        }

        public String SelectedFile
        {
            get
            {
                return path + "/" + file;
            }
        }

        public String SelectedPath
        {
            get
            {
                return path;
            }
        }

        public String SelectedFileName
        {
            get
            {
                return file;
            }
        }

        public String SelectedFileUrl
        {
            get
            {
                String retstr = SelectedFile.Replace("ftp://", "http://");
                retstr = retstr.Replace(" ", "%20");
                return retstr;
            }
        }
        #endregion

        #region Private Methods
        private void LoadTestNodes()
        {
            
            FtpTreeNode root = new FtpTreeNode("root", "", 21);
            root.AddDirectory("Folder1");
            root.AddDirectory("Folder2");
            FtpTreeNode node1 = new FtpTreeNode("root", "Folder1", 21);
            node1.Text = "Folder1";
            FtpTreeNode subnode1 = new FtpTreeNode("root", "Folder1/SubfolderA", 21);
            subnode1.Text = "SubfolderA";
            node1.Directories.Add("SubfolderA");
            subnode1.Files.Add("SubfileA");
            node1.Nodes.Add(subnode1);
            node1.AddFile("File1");
            node1.AddFile("File2");
            FtpTreeNode node2 = new FtpTreeNode("root", "Folder2", 21);
            node2.Text = "Folder2";
            node2.AddFile("File3");
            root.Nodes.Add(node1);
            root.Nodes.Add(node2);
            root.Expand();
            directorytree.Nodes.Add(root); 
        }

        private void LoadSubNodes(FtpTreeNode rootnode)
        {
            UriBuilder ub;
            if (rootnode.Path != String.Empty) ub = new UriBuilder("ftp", rootnode.Server, rootnode.Port, rootnode.Path);
            else ub = new UriBuilder("ftp", rootnode.Server, rootnode.Port);
            String uristring = ub.Uri.OriginalString;
            req = (FtpWebRequest)FtpWebRequest.Create(ub.Uri);
            req.Credentials = ftpcred;
            req.UsePassive = pasv;
            req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;            
            try
            {
                rsp = (FtpWebResponse)req.GetResponse();
                StreamReader rsprdr = new StreamReader(rsp.GetResponseStream());
                String[] rsptokens = rsprdr.ReadToEnd().Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String str in rsptokens)
                {
                    if (str.Contains("<DIR>") == true)
                    {
                        String[] directorytokens = str.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        FtpTreeNode subnode = new FtpTreeNode(rootnode.Server, rootnode.Path + "/" + directorytokens[directorytokens.Length - 1].Trim(), rootnode.Port);
                        subnode.Text = subnode.Directory;
                        LoadSubNodes(subnode);
                        rootnode.AddDirectory(directorytokens[directorytokens.Length - 1].Trim());
                        rootnode.Nodes.Add(subnode);
                    }
                    else
                    {
                        String[] filetokens = str.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        if(filetokens.Length >= 9 )
                            if(filetokens[8] != "." && filetokens[8] != "..")
                                rootnode.AddFile(filetokens[filetokens.Length - 1].Trim());
                    }
                }
            }
            catch (WebException wEx)
            {

            }
        }

        private void SetSubnodeBackColor(FtpTreeNode node, Color c)
        {
            foreach (FtpTreeNode n in node.Nodes)
            {
                SetSubnodeBackColor(n, c);
                n.BackColor = c;
            }
            node.BackColor = c;
        }
        #endregion

        #region Event Triggers
        protected override void OnShown(EventArgs e)
        {
            if (testmode == true)
            {
                LoadTestNodes();
            }
            else if (promptforserver == true)
            {
                loadnewhostbutton.PerformClick();
            }
            else
            {
                fsloader = new BackgroundWorker();
                fsloader.WorkerReportsProgress = true;
                fsloader.WorkerSupportsCancellation = true;
                fsloader.DoWork += new DoWorkEventHandler(fsloader_DoWork);
                fsloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(fsloader_RunWorkerCompleted);
                progress = new FtpBrowseProgressDialog();
                progress.FormClosing += new FormClosingEventHandler(progress_FormClosing);
                fsloader.RunWorkerAsync();
                progress.ShowDialog();
            }
            base.OnShown(e);
        }

        void progress_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((progress.Cancelled == true) && (fsloader.IsBusy)) fsloader.CancelAsync();
        }

        void fsloader_DoWork(object sender, DoWorkEventArgs e)
        {
            FtpTreeNode rootnode = new FtpTreeNode(host, startpath, ftpport);
            LoadSubNodes(rootnode);
            e.Result = rootnode;
            
        }

        void fsloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            directorytree.Nodes.Clear();
            filelist.Items.Clear();
            FtpTreeNode rootnode = (FtpTreeNode)e.Result;
            directorytree.Nodes.Add(rootnode);
            rootnode.Expand();
            directorytree.SelectedNode = rootnode;
            progress.Close();
            splitContainer1.Panel1Collapsed = true;
        }

        private void directorytree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        private void filelist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (filelist.SelectedItems.Count > 0)
            {
                file = filelist.SelectedItems[0].Text;
                okbutton.Enabled = true;
            }
            else
            {
                file = String.Empty;
                okbutton.Enabled = false;
            }
        }

        private void filelist_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (filelist.SelectedItems.Count > 0)
            {
                file = filelist.SelectedItems[0].Text;
                okbutton.Enabled = true;
            }
            else
            {
                file = String.Empty;
                okbutton.Enabled = false;
            }
        }

        private void filelist_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = filelist.SelectedItems[0];
            switch (lvi.ImageIndex)
            {
                case 0:
                    FtpTreeNode parnode = (FtpTreeNode)directorytree.SelectedNode;
                    FtpTreeNode selnode = (FtpTreeNode)parnode.Nodes[lvi.Index];
                    selnode.Select();
                    break;
                case 1:
                    okbutton.PerformClick();
                    break;
            }
        }

        private void directorytree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.ImageIndex = 1;
            e.Node.SelectedImageIndex = 1;
        }

        private void directorytree_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.ImageIndex = 0;
            e.Node.SelectedImageIndex = 0;
        }

        private void directorytree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadFileList( (FtpTreeNode) e.Node );
        }

        private void LoadFileList(FtpTreeNode node)
        {
            filelist.Items.Clear();
            FtpTreeNode clicknode = node;
            path = clicknode.FullPath;
            file = String.Empty;
            okbutton.Enabled = false;
            foreach (String d in clicknode.Directories)
            {
                filelist.Items.Add(d, 0);
            }
            foreach (String f in clicknode.Files)
            {
                filelist.Items.Add(f, 1);
            }
            if (directorytree.SelectedNode == directorytree.Nodes[0]) updirectorybutton.Enabled = false;
            else updirectorybutton.Enabled = true;
            SetSubnodeBackColor(directorytree.Nodes[0] as FtpTreeNode, directorytree.BackColor);
            directorytree.SelectedNode.BackColor = Color.LightGray;
        }

        private void loadnewhostbutton_Click(object sender, EventArgs e)
        {
            FtpBrowseDialogLogin newlogin = new FtpBrowseDialogLogin(host, startpath, ftpport, ftpcred.UserName, ftpcred.Password, pasv);
            if (newlogin.ShowDialog() == DialogResult.OK)
            {
                host = newlogin.Server;
                path = newlogin.StartPath;
                ftpcred = new NetworkCredential(newlogin.Username, newlogin.Password);
                pasv = newlogin.PassiveMode;
                ftpport = newlogin.Port;
                fsloader = new BackgroundWorker();
                fsloader.WorkerReportsProgress = true;
                fsloader.WorkerSupportsCancellation = true;
                fsloader.DoWork += new DoWorkEventHandler(fsloader_DoWork);
                fsloader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(fsloader_RunWorkerCompleted);
                progress = new FtpBrowseProgressDialog();
                progress.FormClosing += new FormClosingEventHandler(progress_FormClosing);
                fsloader.RunWorkerAsync();
                progress.ShowDialog();
            }
        }

        private void updirectorybutton_Click(object sender, EventArgs e)
        {
            if (directorytree.SelectedNode != directorytree.Nodes[0])
            {
                FtpTreeNode curnode = (FtpTreeNode)directorytree.SelectedNode;
                curnode.Collapse();
                FtpTreeNode selnode = (FtpTreeNode)directorytree.SelectedNode.Parent;
                selnode.Select();
            }
        }

        private void filelist_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b') updirectorybutton.PerformClick();
        }
        #endregion
    }
}