using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using freetrain.contributions.others;
using freetrain.framework;

namespace freetrain.finance.stock
{
	/// <summary>
	/// prepare for the bank plugin
	/// </summary>
	[Serializable]
	public class StockMarketPlugIn : MenuContribution
	{
		static private StockMarketPlugIn _theInstance = null;
		static internal StockMarketPlugIn theInstance { get { return _theInstance; } }

		public StockMarketPlugIn( XmlElement e ) : base(e) {
			_theInstance = this;
			StockMarketConfig.init(e);
			StockCompanyModule.init();
			Economy.init(e);
		}

		public override void mergeMenu( MainMenu containerMenu ) {
			MenuItem item = new MenuItem();
			item.Text = "Stock Market";
			//! item.Text = "証券会社";
			item.Click += new System.EventHandler(onClick);

			containerMenu.MenuItems[1].MenuItems.Add(item);
		}

		private void onClick( object sender, EventArgs args ) {
			StockMarketForm.ShowForm();
		}
	}
}
