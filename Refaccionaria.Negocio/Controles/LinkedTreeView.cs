using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Refaccionaria.Negocio
{
    // Fuente: http://stackoverflow.com/questions/6442911/how-do-i-sync-scrolling-in-2-treeviews-using-the-slider
    public partial class LinkedTreeView : TreeView
    {
        public LinkedTreeView()
            : base()
        {
            this.AfterSelect += new TreeViewEventHandler(LinkedTreeView_AfterSelect);
            this.AfterExpand += new TreeViewEventHandler(LinkedTreeView_AfterExpand);
            this.AfterCollapse += new TreeViewEventHandler(LinkedTreeView_AfterCollapse);
        }

        private List<LinkedTreeView> linkedTreeViews = new List<LinkedTreeView>();

        /// <summary>
        /// Links the specified tree view to this tree view.  Whenever either treeview
        /// scrolls, the other will scroll too.
        /// </summary>
        /// <param name="treeView">The TreeView to link.</param>
        public void AddLinkedTreeView(LinkedTreeView treeView)
        {
            if (treeView == this)
                throw new ArgumentException("Cannot link a TreeView to itself!", "treeView");

            if (!linkedTreeViews.Contains(treeView))
            {
                //add the treeview to our list of linked treeviews
                linkedTreeViews.Add(treeView);
                //add this to the treeview's list of linked treeviews
                treeView.AddLinkedTreeView(this);

                //make sure the TreeView is linked to all of the other TreeViews that this TreeView is linked to
                for (int i = 0; i < linkedTreeViews.Count; i++)
                {
                    //get the linked treeview
                    var linkedTreeView = linkedTreeViews[i];
                    //link the treeviews together
                    if (linkedTreeView != treeView)
                        linkedTreeView.AddLinkedTreeView(treeView);
                }
            }
        }

        /// <summary>
        /// Sets the destination's scroll positions to that of the source.
        /// </summary>
        /// <param name="source">The source of the scroll positions.</param>
        /// <param name="dest">The destinations to set the scroll positions for.</param>
        private void SetScrollPositions(LinkedTreeView source, LinkedTreeView dest)
        {
            //get the scroll positions of the source
            int horizontal = User32.GetScrollPos(source.Handle, Orientation.Horizontal);
            int vertical = User32.GetScrollPos(source.Handle, Orientation.Vertical);
            //set the scroll positions of the destination
            User32.SetScrollPos(dest.Handle, Orientation.Horizontal, horizontal, true);
            User32.SetScrollPos(dest.Handle, Orientation.Vertical, vertical, true);
        }

        protected override void WndProc(ref Message m)
        {
            //process the message
            base.WndProc(ref m);

            //pass scroll messages onto any linked views
            if (m.Msg == User32.WM_VSCROLL || m.Msg == User32.WM_MOUSEWHEEL)
            {
                foreach (var linkedTreeView in linkedTreeViews)
                {
                    //set the scroll positions of the linked tree view
                    SetScrollPositions(this, linkedTreeView);
                    //copy the windows message
                    Message copy = new Message
                    {
                        HWnd = linkedTreeView.Handle,
                        LParam = m.LParam,
                        Msg = m.Msg,
                        Result = m.Result,
                        WParam = m.WParam
                    };

                    //pass the message onto the linked tree view
                    linkedTreeView.RecieveWndProc(ref copy);
                }
            }
        }

        /// <summary>
        /// Recieves a WndProc message without passing it onto any linked treeviews.  This is useful to avoid infinite loops.
        /// </summary>
        /// <param name="m">The windows message.</param>
        private void RecieveWndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        /// <summary>
        /// Imported functions from the User32.dll
        /// </summary>
        private class User32
        {
            public const int WM_VSCROLL = 0x115;
            public const int WM_MOUSEWHEEL = 0x020A;

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern int GetScrollPos(IntPtr hWnd, System.Windows.Forms.Orientation nBar);

            [DllImport("user32.dll")]
            public static extern int SetScrollPos(IntPtr hWnd, System.Windows.Forms.Orientation nBar, int nPos, bool bRedraw);
        }

        #region [ Funcionalidad agregada - Moisés ]

        void LinkedTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var IndicesRuta = this.IndicesRuta(e.Node);
            foreach (var oArbol in this.linkedTreeViews)
                oArbol.SelectedNode = this.NodoDeIndicesRuta(oArbol, IndicesRuta);
        }

        void LinkedTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            var IndicesRuta = this.IndicesRuta(e.Node);
            foreach (var oArbol in this.linkedTreeViews)
                this.NodoDeIndicesRuta(oArbol, IndicesRuta).Expand();
        }

        void LinkedTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            var IndicesRuta = this.IndicesRuta(e.Node);
            foreach (var oArbol in this.linkedTreeViews)
                this.NodoDeIndicesRuta(oArbol, IndicesRuta).Collapse();
        }

        public List<int> IndicesRuta(TreeNode oNodo)
        {
            List<int> Indices = new List<int>();
            while (oNodo != null)
            {
                Indices.Add(oNodo.Index);
                oNodo = oNodo.Parent;
            }
            Indices.Reverse();

            return Indices;
        }

        public TreeNode NodoDeIndicesRuta(TreeView oArbol, List<int> Indices)
        {
            var oNodo = oArbol.Nodes[Indices[0]];
            for (int iInd = 1; iInd < Indices.Count; iInd++)
            {
                oNodo = oNodo.Nodes[Indices[iInd]];
            }
            return oNodo;
        }

        public void RemoveLinkedTreeView(LinkedTreeView treeView)
        {
            if (this.linkedTreeViews.Contains(treeView))
                this.linkedTreeViews.Remove(treeView);
            if (treeView.linkedTreeViews.Contains(this))
                treeView.linkedTreeViews.Remove(this);
        }

        #endregion
    }
}
