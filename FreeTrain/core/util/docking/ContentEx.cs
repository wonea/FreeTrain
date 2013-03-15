using System;
using Crownwood.Magic.Docking;

namespace freetrain.util.docking
{
	/// <summary>
	/// ContentEx の概要の説明です。
	/// </summary>
	public class ContentEx : Content
	{
		/// <summary> event handler type </summary>
		public delegate void EventHandler();

		/// <summary> Fired when the content is closed by the user. </summary>
		public event EventHandler hidden;
		/// <summary> Fired when the content is shown by the user.</summary>
		public event EventHandler shown;

		public ContentEx( DockingManagerEx owner, string title, int imageIndex ) : base(owner) {
			base.Title = title;
			base.ImageList = owner.imageList;
			base.ImageIndex = imageIndex;
		}

		/// <summary>
		/// Equivalent of calling <code>DockingManager.ShowContent</code>
		/// </summary>
		public void show() {
			base.DockingManager.ShowContent(this);
		}
		/// <summary>
		/// Equivalent of calling <code>DockingManager.HideContent</code>
		/// </summary>
		public void hide() {
			base.DockingManager.HideContent(this);
		}

		/// <summary>
		/// Called when the content is closed by the user.
		/// Can be overrided but the base implementation should be invoked.
		/// </summary>
		protected internal virtual void OnHidden() {
			if(hidden!=null)
				hidden();
		}
		
		/// <summary>
		/// Called when the content is shown by the user.
		/// Can be overrided but the base implementation should be invoked.
		/// </summary>
		protected internal virtual void OnShown() {
			if(shown!=null)
				shown();
		}
	}
}
