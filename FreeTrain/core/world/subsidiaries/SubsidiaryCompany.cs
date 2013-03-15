using System;
using System.Diagnostics;
using freetrain.world.structs;
using freetrain.world.accounting;

namespace freetrain.world.subsidiaries
{
	/// <summary>
	/// Tradable subsidiary company.
	/// 
	/// This object is accessible through the queryInterface method.
	/// </summary>
	[Serializable]
	public class SubsidiaryCompany
	{
		private bool _isOwned;

		/// <summary>
		/// Current market list price.
		/// </summary>
		private long price;

		private long totalLandValue;


		/// <summary>Gets the corresponding entity.</summary>
		public readonly SubsidiaryEntity owner;
		
		public SubsidiaryCompany( SubsidiaryEntity _owner, bool initiallyOwned ) {
			this.owner = _owner;
			this._isOwned = initiallyOwned;
			owner.onEntityRemoved += new EventHandler(onOwnerRemoved);
			updateCurrentPrice();

			if( _isOwned )	SubsidiaryMarket.SELL.add(this);
			
			registerClock();
		}

		public bool isOwned { get { return _isOwned; } }

		public bool onSale {
			get {
				return SubsidiaryMarket.BUY.contains(this);
			}
		}

		public long currentMarketPrice { get { return price; } }
		
		/// <summary>
		/// Profit of this company for the past 1 year.
		/// </summary>
		public long profit { get { return Parameters.profit(owner.structurePrice,totalLandValue); } }

		/// <summary>
		/// Operational cost of this company for the past 1 year.
		/// </summary>
		public long cost { get { return Parameters.operationCost(owner.structurePrice,totalLandValue); } }

		/// <summary>
		/// Sales of this company for the past 1 year.
		/// </summary>
		public long sales { get { return Parameters.sales(owner.structurePrice,totalLandValue); } }
		
		/// <summary>
		/// Signals a chane in prices.
		/// </summary>
		[NonSerialized]
		public EventHandler onPriceChangedV;

		public EventHandler onPriceChanged;

	
		/// <summary>
		/// Sells this company. The company must be owned by the player.
		/// </summary>
		public void sell() {
			Debug.Assert(isOwned);
			_isOwned = false;
			SubsidiaryMarket.SELL.remove(this);

			AccountGenre.SUBSIDIARIES.earn( currentMarketPrice );
		}

		/// <summary>
		/// Buys this company. The company must not be owned by the player,
		/// and the company must be listed on <c>SubsidiaryMarket</c>.
		/// </summary>
		public void buy() {
			Debug.Assert(!isOwned);
			Debug.Assert(onSale);
			_isOwned = true;

			// remove from the market
			SubsidiaryMarket.BUY.remove(this);
			SubsidiaryMarket.SELL.add(this);

			AccountGenre.SUBSIDIARIES.spend( currentMarketPrice );
		}
		
		/// <summary>
		/// Updates the price of this entity.
		/// </summary>
		public void updateCurrentPrice() {
			totalLandValue = owner.totalLandPrice;
			price += owner.structurePrice + totalLandValue;
			price += 10*profit;

			if( onPriceChanged!=null )	onPriceChanged (this,null);
			if( onPriceChangedV!=null)	onPriceChangedV(this,null);
		}

		
		/// <summary>
		/// Shouldn't be invoked explicitly.
		/// </summary>
		public void onOwnerRemoved( object owner, EventArgs e ) {
			// this structure is being removed.
			// removed it from the market if it's in it.
			SubsidiaryMarket.BUY.remove(this);
			unregisterClock();
		}

		/// <summary>
		/// Shouldn't be invoked explicitly.
		/// </summary>
		public void clockHandler() {
			updateCurrentPrice();

			if( !isOwned ) {
				if(onSale)
					SubsidiaryMarket.BUY.remove(this);
				else {
					if( random.Next(MARKET_RATIO)==0 )
						SubsidiaryMarket.BUY.add(this);
				}
			}
			registerClock();
		}

		private void registerClock() {
			World.world.clock.registerOneShot(
				new ClockHandler(clockHandler),
				TimeLength.random( TimeLength.fromDays(14), TimeLength.fromDays(28) ) );
		}
		private void unregisterClock() {
			World.world.clock.unregister(new ClockHandler(clockHandler));
		}


		private static readonly Random random = new Random();
		private const int MARKET_RATIO = 10;
	}
}
