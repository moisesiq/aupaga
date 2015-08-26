using System;

namespace CommonTools
{
    public delegate void TreeViewEventHandler(object sender, TreeViewEventArgs e);

    public class TreeViewEventArgs : EventArgs
    {
        public TreeViewEventArgs(CommonTools.Node oNode)
        {
            this.Node = oNode;
        }

        public CommonTools.Node Node { get; private set; }
    }
}
