using System;
using System.Drawing;
using System.Windows.Forms;

namespace freetrain.util.controls
{
	/// <summary>
	/// TreeView control with drag-n-drop functionality
	/// </summary>
	public class DDTreeView : TreeView
	{
		public DDTreeView() {
			AllowDrop = true;
		}
		
		private TreeNode getNodeFrom( DragEventArgs e ) {
			return GetNodeAt( PointToClient(new Point(e.X,e.Y)) );
		}

		protected override void OnItemDrag(ItemDragEventArgs e) {
			base.OnItemDrag(e);
			// e.Item is a TreeNode
			DoDragDrop( new DataObject("treeNode",e.Item), DragDropEffects.Move );
		}

		protected override void OnDragDrop(DragEventArgs e) {
			base.OnDragDrop(e);

			TreeNode n = e.Data.GetData("treeNode") as TreeNode;
			if(n==null)		return;

			// move this node
			OnItemMoved( n, getNodeFrom(e) );
		}

		protected override void OnDragOver(DragEventArgs e) {
			base.OnDragOver(e);
			
			DDTreeNode target = getNodeFrom(e) as DDTreeNode;

			if( target!=null && target.canAcceptDrop) {
				// make sure the data is correct and it belongs to us.
				TreeNode data = e.Data.GetData("treeNode") as TreeNode;
				if( data!=null && data.TreeView==this ) {
					// make sure that data is not a parent of target
					TreeNode n = target;
					while(n!=null) {
						if(n==data)	break;
						n = n.Parent;
					}

					if( n==null ) {
						e.Effect = DragDropEffects.Move;
						return;
					}
				}
			}
			e.Effect = DragDropEffects.None;
		}


		/// <summary>
		/// This method is called when a drag-n-drop is completed.
		/// </summary>
		/// <param name="node">A node being dragged</param>
		/// <param name="newParent">new parent node</param>
		protected virtual void OnItemMoved( TreeNode node, TreeNode newParent ) {
			ItemMoved(node,newParent);
		}

		public ItemMovedHandler ItemMoved;
	}

	public delegate void ItemMovedHandler( TreeNode node, TreeNode newParent );

	public class DDTreeNode : TreeNode {
		public DDTreeNode() : base() {}
		public DDTreeNode( string text ) : base(text) {}

		public bool canAcceptDrop;
	}
}
