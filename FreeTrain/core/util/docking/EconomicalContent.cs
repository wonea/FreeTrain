using System;
using System.Windows.Forms;

namespace freetrain.util.docking
{
	/// <summary>
	/// ContentEx implementation that destroys the associated control
	/// when the content is closed by the user.
	/// 
	/// The derived class needs to override the createControl method to
	/// create a new instance of the associated control.
	/// </summary>
	public abstract class EconomicalContent : ContentEx
	{
		public EconomicalContent( DockingManagerEx owner, string title, int imageIndex ) : base(owner,title,imageIndex) {
		}

		/// <summary>
		/// Create a new instance of the control.
		/// The returned control will be associated with the content
		/// and displayed automatically.
		/// </summary>
		protected abstract Control createControl();

		protected internal override void OnHidden() {
			// destroy the control
			if(base.Control!=null) {
				// not sure exactly why this can be null,
				// but sometimse it is. Be defensive against MagicLibrary
				base.Control.Hide();
				base.Control.Dispose();
				base.Control = null;
			}
		}

		protected internal override void OnShown() {
			// recreate the control
			base.Control = createControl();
			base.Control.Show();
		}
	}

	public class EconomicalContentImpl : EconomicalContent
	{
		public delegate Control WindowCreator();

		private readonly WindowCreator creator;

		public EconomicalContentImpl( DockingManagerEx owner, string title, int imageIndex, WindowCreator _creator )
			: base(owner,title,imageIndex) {

			this.creator = _creator;
		}

		protected override Control createControl() {
			return creator();
		}
	}
}
