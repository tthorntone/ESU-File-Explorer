using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    public static class Extensions
    {
        public static IEnumerable<TreeNode> GetChildren(this TreeNode Parent)
        {
            return Parent.Nodes.Cast<TreeNode>().Concat(
                   Parent.Nodes.Cast<TreeNode>().SelectMany(GetChildren));
        }
    }
}