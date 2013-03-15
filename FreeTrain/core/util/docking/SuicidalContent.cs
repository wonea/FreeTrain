using System;
using System.Windows.Forms;

namespace freetrain.util.docking
{
	/// <summary>
	/// ContentEx implementation that destroys itself
	/// as soon as the user hides the content.
	/// 
	/// The derived class should use the constructor to
	/// associate control to the content.
	/// </summary>
	public class SuicidalContent : ContentEx
	{
		public SuicidalContent( DockingManagerEx owner, string title, int imageIndex ) : base(owner,title,imageIndex) {
		}

		protected internal override void OnHidden() {
			// kill thyself
			DockingManager.Contents.Remove(this);
		}
	}
}
