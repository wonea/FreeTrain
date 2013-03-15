using System;

namespace freetrain.controls
{
	/// <summary>
	/// WebBrowser control.
	/// </summary>
	public class WebBrowser : System.Windows.Forms.WebBrowser {
		public WebBrowser() { }

		public void navigate( string url ) {
			base.Navigate(url);
		}
	}
}
