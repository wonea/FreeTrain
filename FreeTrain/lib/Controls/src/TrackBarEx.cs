using System;
using System.Windows.Forms;

namespace freetrain.controls
{
	/// <summary>
	/// TrackBar class with a bug fix to the mouse wheel support.
	/// </summary>
	public class TrackBarEx : TrackBar
	{
		public TrackBarEx() {
		}

		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			OnValueChanged(e);
		}
	}
}
