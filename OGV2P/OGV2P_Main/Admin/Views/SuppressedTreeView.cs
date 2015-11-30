using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace OGV2P.Admin.Views
{
    public class SuppressedTreeView : System.Windows.Forms.TreeView
    {
        private bool _supressExpandedCol = false;
        private DateTime _lastClick = DateTime.Now;

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            _supressExpandedCol = DateTime.Now.Subtract(_lastClick).TotalMilliseconds <= SystemInformation.DoubleClickTime;
            _lastClick = DateTime.Now;

            base.OnMouseDown(e);
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);
        }

        


    }
}
