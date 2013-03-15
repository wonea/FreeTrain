using System;
using System.Windows.Forms;

namespace freetrain.controllers
{
	/// <summary>
	/// Implemented by a dialog box who hosts <c>ModalController</c>s.
	/// </summary>
	public interface IControllerSite
	{
		/// <summary>
		/// Close the host.
		/// </summary>
		void close();

		string name { get; } // TODO: fix
	}

	public class ControllerSiteImpl : IControllerSite
	{
		private readonly Form owner;

		public ControllerSiteImpl( Form _owner ) { this.owner=_owner; }
 
		public void close() {
			owner.Close();
		}

		public string name {
			get {
				return owner.Text;
			}
		}
	}
}
