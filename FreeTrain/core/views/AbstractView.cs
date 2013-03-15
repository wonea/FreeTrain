using System;
using System.Windows.Forms;
using freetrain.world;
using freetrain.framework;

namespace freetrain.views
{
	/// <summary>
	/// Viewからコンテナに対するインターフェース
	/// </summary>
	public interface IView
	{
		/// <summary>Starts displaying view window(s).</summary>
		void show( Form parent );

		/// <summary>Closes and disposes view window(s)</summary>
		void close();
	}

	/// <summary>
	/// Partial default implementation of IView for
	/// those views that only have one Form as its window.
	/// </summary>
	public abstract class AbstractView : IView
	{
		/// <summary>
		/// Reference to the view window.
		/// </summary>
		protected readonly Form form;

		public AbstractView( Form form ) {
			this.form = form;
		}

		public void show( Form parent ) {
			form.MdiParent = parent;
			form.Show();
		}

		public void close() {
			form.Close();
			form.Dispose();
			MainWindow.mainWindow.removeView(this);
		}
	}
}
