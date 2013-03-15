using System;
using System.Windows.Forms;

namespace freetrain.util
{
	/// <summary>
	/// Provides a simple UI feedback.
	/// </summary>
	public class LongTask : IDisposable
	{
		public LongTask() {
			Cursor.Current = Cursors.WaitCursor;
		}

		public void Dispose() {
			Cursor.Current = Cursors.Default;
		}
	}
}
