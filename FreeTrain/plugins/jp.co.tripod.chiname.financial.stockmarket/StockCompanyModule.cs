using System;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using freetrain.framework.plugin;
using freetrain.world;
using freetrain.world.accounting;
using freetrain.util;

namespace freetrain.finance.stock
{
	public delegate void MarketBusinessHourListener();
	public delegate void OwnershipListener( OwnershipStatus status, Company com );

	public enum MarketStatus : int { OPEN,CLOSE,HOLIDAY } 
	
	/// <summary>
	/// Bank.
	/// This object is part of the serialized game data. Parameters that
	/// shouldn't be serialized should go to BankConfig.
	/// </summary>
	[Serializable]
	public sealed class StockCompanyModule
	{

		static private Clock clock { get { return World.world.clock; } }

		static public readonly int numUNIT = 100; 
		static public readonly string strUNIT = "hundred shares"; 
		//! static public readonly string strUNIT = "百株"; 
		static private ListedCompanies companies { get { return Economy.Companies; } }
		[NonSerialized]
		internal Set retains = new Set(); 

		public IEnumerator getMarketStocks(){ return companies.GetEnumerator(); }
		public IEnumerator getRetainStocks(){ return retains.GetEnumerator(); }

		[NonSerialized]
		public MarketBusinessHourListener onBusinesStatusChanging;
		[NonSerialized]
		public OwnershipListener onTrading;

		private static StockCompanyModule _theInstance = null;
		public static StockCompanyModule theInstance { get { return _theInstance; } }
		
		// don't allow instanciation from external objects
		private StockCompanyModule() {
			// Set clock timer
			clock.registerRepeated(
				new ClockHandler(statusClockHandler),
				TimeLength.untilTomorrow(),
				TimeLength.ONEDAY );

			clock.registerRepeated(
				new ClockHandler(statusClockHandler),
				TimeLength.untilTomorrow()+
					TimeLength.fromHours(StockMarketConfig.openHour),
				TimeLength.ONEDAY );

			if( StockMarketConfig.openHour%24 != StockMarketConfig.closeHour%24 ) {
				clock.registerRepeated(
					new ClockHandler(statusClockHandler),
					TimeLength.untilTomorrow()+
						TimeLength.fromHours(StockMarketConfig.closeHour),
					TimeLength.ONEDAY );
			}
		}

		internal static void init() {
			World.onNewWorld += new EventHandler(onNewWorld);
		}

	
		private static void onNewWorld(object sender,EventArgs a) {
			MarketBusinessHourListener listener = null;
			if( _theInstance != null )
				listener = _theInstance.onBusinesStatusChanging;
			_theInstance = (StockCompanyModule)World.world.otherObjects["{227E053A-6667-43fe-8CE2-26EB55CE6A56}"];
			if( _theInstance==null )
			{
				World.world.otherObjects["{227E053A-6667-43fe-8CE2-26EB55CE6A56}"] = 
					_theInstance = new StockCompanyModule();
				Economy.onNewWorld("{227E053A-6667-43fe-8CE4-26EB55CE6A56}");
			}
			else 
			{
				Economy.onWorldLoaded("{227E053A-6667-43fe-8CE4-26EB55CE6A56}");
			}
			// restoring listener registrations (may be a BankbookWindow).
			if( listener != null )
				_theInstance.onBusinesStatusChanging = listener;
			_theInstance.rebuildRetainSet();
			// restore registrations for BankbookListHelper
			StocksListHelper.restoreData();
			_theInstance.onTrading = new OwnershipListener(StocksListHelper.onOwnershipStatusChanged);
			Economy.runEconomy();
		}

		private void rebuildRetainSet()
		{
			retains = new Set();
			IEnumerator e = companies.GetEnumerator();
			while(e.MoveNext())
			{
				Company com = (Company)e.Current;
				if( com.stockData._numOwn>0 )
					retains.add(com);
			}
		}

		public void statusClockHandler() 
		{
			if(onBusinesStatusChanging!=null)
				onBusinesStatusChanging();
		}
		
		/// <summary>
		/// Returns the current status of the bank.
		/// </summary>
		public MarketStatus status {
			get {
				if(StockMarketConfig.isHoliday(clock))
					return MarketStatus.HOLIDAY;
				if( StockMarketConfig.openHour <= clock.hour && clock.hour < StockMarketConfig.closeHour )
					return MarketStatus.OPEN;
				else
					return MarketStatus.CLOSE;
			}
		}

		public long calcTotalAssessedAmount()
		{
			IEnumerator	e = getRetainStocks();
			long sum = 0;
			while(e.MoveNext()) {
				sum += ((Company)e.Current).stockData.assessedAmount;
			}
			return sum;
		}

		public void buy(Company com, int amount)
		{
			bool r = com.stockData.buy(amount);
			if( onTrading != null ) {
				if(r) {
					retains.add(com);
					onTrading(OwnershipStatus.NEW_OWNERSHIP,com);
				}
				else
					onTrading(OwnershipStatus.UPDATE_OWNERSHIP,com);
			}
		}

		public void sell(Company com, int amount)
		{
			bool r = com.stockData.sell(amount);
			if( onTrading != null ) 
			{
				if(r) {
					retains.remove(com);
					onTrading(OwnershipStatus.LOST_OWNERSHIP,com);
				}
				else
					onTrading(OwnershipStatus.UPDATE_OWNERSHIP,com);
			}
		}

		static public int calcCommition( int price, int number )
		{
			return (int)((long)price)*number/100;
		}

		static public int calcBuyableStocks( StockData stock, long capital )
		{
			if( stock.currentPriceU == 0 ) return 0;
			long n = capital*99/stock.currentPriceU/100;
			if( n > stock.numInMarket ) n = stock.numInMarket;
			return (int)n;
		}
	}
}
