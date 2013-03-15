using System;
using System.Windows.Forms;
using Crownwood.Magic.Docking;

namespace freetrain.util.docking
{
	public class DockingManagerEx : DockingManager, IDisposable
	{
		/// <summary>
		/// Repository of icons associated with the contents of this docking manager.
		/// </summary>
		public readonly ImageList imageList = new ImageList();

		public DockingManagerEx( Form owner ) : base(owner, Crownwood.Magic.Common.VisualStyle.IDE ) {
			this.ContentShown  += new ContentHandler(OnContentShownHandler);
			this.ContentHidden += new ContentHandler(OnContentHiddenHandler);
		}


		public void Dispose() {
			imageList.Dispose();
		}

		// distribute the events to ContentEx so that it can be caught individually.
		//
		private void OnContentShownHandler( Content c, EventArgs ea ) {
			((ContentEx)c).OnShown();
		}

		private void OnContentHiddenHandler( Content c, EventArgs ea ) {
			((ContentEx)c).OnHidden();
		}
	}
}
