using System;
using System.Runtime.Serialization;
using freetrain.util;

namespace freetrain.world.subsidiaries
{
	public delegate void SubsidiaryMarketListener( SubsidiaryMarket sender, SubsidiaryCompany company );


	/// <summary>
	/// A list of <c>SubsidiaryCompany</c>s that are being sold.
	/// </summary>
	[Serializable]
	public class SubsidiaryMarket : IDeserializationCallback
	{
		private SubsidiaryMarket() {
			volatileEvents = new Events();
		}
		public void OnDeserialization(object sender) {
			volatileEvents = new Events();
		}


		/// <summary>
		/// The market for third party companies. The player can buy them.
		/// </summary>
		public static SubsidiaryMarket BUY {
			get {
				return theInstance("buy");
			}
		}
		/// <summary>
		/// The market for companies owned by the player. The player can sell them.
		/// </summary>
		public static SubsidiaryMarket SELL {
			get {
				return theInstance("sell");
			}
		}

		private static SubsidiaryMarket theInstance(string suffix) {
				string name = typeof(SubsidiaryMarket).Name + suffix;
				SubsidiaryMarket r = (SubsidiaryMarket)World.world.otherObjects[name];
				if(r==null)
					World.world.otherObjects[name] = r = new SubsidiaryMarket();
				return r;
		}
		


		/// <summary>
		/// Set of on-sale companies.
		/// </summary>
		private readonly Set onSale = new Set();
		
		[Serializable]
		public class Events {
			/// <summary>
			/// Fires when a new company enters the market.
			/// </summary>
			public SubsidiaryMarketListener onAdded;
			
			/// <summary>
			/// Fires when a company leaves the market.
			/// </summary>
			public SubsidiaryMarketListener onRemoved;
		}

		public readonly Events persistentEvents = new Events();
		
		[NonSerialized]
		public Events volatileEvents;


		/// <summary>
		/// Returns the list of companies on sale.
		/// </summary>
		public SubsidiaryCompany[] companiesOnSale {
			get {
				return (SubsidiaryCompany[])onSale.toArray(typeof(SubsidiaryCompany));
			}
		}

		/// <summary>
		/// Shouldn't be called from outside the package.
		/// </summary>
		internal void add( SubsidiaryCompany company ) {
			onSale.add(company);
			if( persistentEvents.onAdded!=null )
				persistentEvents.onAdded(this,company);
			if( volatileEvents.onAdded!=null )
				volatileEvents.onAdded(this,company);
		}
		
		/// <summary>
		/// Shouldn't be called from outside the package.
		/// </summary>
		internal void remove( SubsidiaryCompany company ) {
			onSale.remove(company);
			if( persistentEvents.onRemoved!=null )
				persistentEvents.onRemoved(this,company);
			if( volatileEvents.onRemoved!=null )
				volatileEvents.onRemoved(this,company);
		}

		/// <summary>
		/// Shouldn't be called from outside the package.
		/// </summary>
		internal bool contains( SubsidiaryCompany company ) {
			return onSale.contains(company);
		}
	}
}
