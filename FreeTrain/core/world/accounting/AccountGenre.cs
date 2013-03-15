using System;
using System.Xml;
using freetrain.framework.plugin;

namespace freetrain.world.accounting
{
	/// <summary>
	/// Accounting genre. Used to categorize expenses and sales.
	/// </summary>
	[Serializable]
	public class AccountGenre : Contribution
	{
		public AccountGenre( XmlElement e ) : base(e) {
			name = XmlUtil.selectSingleNode(e,"name").InnerText;
			World.onNewWorld += new EventHandler(onNewWorld);
		}

		/// <summary> Name of this genre. </summary>
		public readonly string name;
		
		private TransactionHistory _history;


		/// <summary> Notified whenever the data gets updated. </summary>
		[NonSerialized]
		public AccountListener onUpdate;

		/// <summary>
		/// Get the transaction history of this genre.
		/// </summary>
		public TransactionHistory history {
			get {
				if( _history==null ) {
					_history = (TransactionHistory)World.world.otherObjects[this];
					if( _history==null )
						World.world.otherObjects.Add( this, _history=new TransactionHistory() );
				}

				return _history;
			}
		}

		private void onNewWorld( object sender, EventArgs e ) {
			_history = null;
		}

		/// <summary>
		/// Short-cut to the <code>AccountManager.spend</code> method.
		/// </summary>
		/// <param name="delta"></param>
		public void spend( long delta ) {
			AccountManager.theInstance.spend( delta, this );
		}

		/// <summary>
		/// Short-cut to the <code>AccountManager.earn</code> method.
		/// </summary>
		/// <param name="delta"></param>
		public void earn( long delta ) {
			AccountManager.theInstance.earn( delta, this );
		}

		public override string ToString() { return name; }


		//
		// Built-in genre.
		//
		public static AccountGenre RAIL_SERVICE {
			get {
				return (AccountGenre)PluginManager.theInstance
					.getContribution("{AC30BB0B-044C-4784-A83D-FCB1F60B3CF2}");
			}
		}
		public static AccountGenre ROAD_SERVICE {
			get {
				return (AccountGenre)PluginManager.theInstance
					.getContribution("{CC00A6D1-D078-4D3C-AFB2-EDC6CB9F4CB3}");
			}
		}
		public static AccountGenre SUBSIDIARIES {
			get {
				return (AccountGenre)PluginManager.theInstance
					.getContribution("{2A666F1A-9F40-4F67-98F4-DEAC1E55296D}");
			}
		}
		public static AccountGenre OTHERS {
			get {
				return (AccountGenre)PluginManager.theInstance
					.getContribution("{C0A9ABA5-801F-4AA6-93EA-6FF563C2B407}");
			}
		}
	}
}
