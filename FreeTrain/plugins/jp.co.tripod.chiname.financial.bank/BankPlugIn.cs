using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using freetrain.contributions.others;
using freetrain.framework;
using freetrain.framework.plugin;

namespace freetrain.views.bank
{
	/// <summary>
	/// prepare for the bank plugin
	/// </summary>
	[Serializable]
	public class BankPlugIn : MenuContribution
	{
		public BankPlugIn( XmlElement e ) : base(e) {
			BankConfig.init(e);
			BankModule.init();
		}

		public override void mergeMenu( MainMenu containerMenu ) {
			MenuItem item = new MenuItem();
			item.Text = "Bank Teller";
			//! item.Text = "銀行";
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[1].MenuItems.Add(item);
		}

		private void onClick( object sender, EventArgs args ) {
			BankCounterForm.ShowBankCounter();
		}
	}
}
