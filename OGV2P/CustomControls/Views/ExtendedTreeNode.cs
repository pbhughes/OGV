using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomControls.Views 
{
    public class ExtendedTreeNode : System.Windows.Forms.TreeNode
    {
        private long _startingHash;

        public long StartingHash
        {
            get
            {
                return _startingHash;
            }

            set
            {
                _startingHash = value;
            }
        }

      
    }
}
