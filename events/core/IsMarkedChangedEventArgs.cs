using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using urakawa.core;

namespace urakawa.events.core
{
    public class IsMarkedChangedEventArgs: TreeNodeEventArgs
    {
        public IsMarkedChangedEventArgs(TreeNode src) : base(src)
        {
        }
    }
}
