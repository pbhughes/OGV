using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Text;

namespace BuckSoft.Controls.FtpBrowseDialog
{
    public class FtpTreeNode : TreeNode
    {
        #region Prive Fields
        private StringCollection files;
        private StringCollection directories;
        private String host, path;
        private int port;
        #endregion

        #region Constructor
        public FtpTreeNode(String hosturl, String startpath, int ftpport)
        {
            files = new StringCollection();
            directories = new StringCollection();
            host = hosturl.TrimEnd('/');
            path = startpath.TrimEnd('/');
            port = ftpport;
            base.Text = FtpPath;
        }
        #endregion

        #region Public Properties
        public StringCollection Files
        {
            get
            {
                return files;
            }
        }
        public StringCollection Directories
        {
            get
            {
                return directories;
            }
        }
        public String Server
        {
            get
            {
                return host;
            }
        }
        public String Path
        {
            get
            {
                return path;
            }
        }
        public int Port
        {
            get
            {
                return port;
            }
        }
        public String Directory
        {
            get
            {
                String[] pathtokens = path.Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                return pathtokens[pathtokens.Length - 1];
            }
        }
        public String FtpPath
        {
            get
            {
                return ("ftp://" + host + "/" + path).TrimEnd('/');
            }
        }
        #endregion

        #region Public Methods
        public int AddFile(String file)
        {
            return files.Add(file);
        }
        public int AddDirectory(String dir)
        {
            return directories.Add(dir);
        }
        public void Select()
        {
            TreeView.SelectedNode = this;
            this.EnsureVisible();
        }
        #endregion
    }
}
