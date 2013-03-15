using System;
using freetrain.world;
using freetrain.world.accounting;

namespace freetrain.finance.stock
{
	public enum OwnershipStatus :int { NEW_OWNERSHIP, LOST_OWNERSHIP, UPDATE_OWNERSHIP }
	/// <summary>
	/// Stock の概要の説明です。
	/// </summary>
	[Serializable]
	public class StockData : IStockDataTable
	{
		static public readonly int LOGSIZE=30;
		static private StockCompanyModule market { get { return StockCompanyModule.theInstance; } }
		static private AccountManager manager { get { return World.world.account; } }
		static private int unit { get { return StockCompanyModule.numUNIT; } }

		public StockData()
		{
			_currentPriceU = 0;
			_dividendU = 0;
			_numTotal = _numInMarket = 1;
		}

		public StockData(int total, int share)
		{
			if( share > total ) share = total;
			_numTotal = total;
			_numInMarket = total - share;
		}
		// total money player spend to bought.
		private long moneySpend = 0;

		internal int _numTotal = 10000;
		internal int _numInMarket = 3000;
		internal int _numOwn = 0;
		internal double _dividendU = 10;
		private int _currentPriceU = 10000;
		[NonSerialized]
		internal int _properPriceU = 10000;

		public int numTotal { get { return _numTotal; } }
		public int numInMarket { get { return _numInMarket; } }
		public int numOwn  { get { return _numOwn; } }
		public double dividend  { get { return _dividendU/unit; } }
		public double dividendU  { get { return _dividendU; } }
		public int priceChange { get { return sp_today.end-this[XAxisStyle.DAILY,1].end; } }
		public long assessedAmount { get { return ((long)currentPriceU)*numOwn; } }

		protected StockPrice sp_today = new StockPrice(0);
		protected StockPriceLogger log = new StockPriceLogger(LOGSIZE);

		public int currentPrice	{ get {	return _currentPriceU/unit; } }
		
		// the trading unit price
		public int currentPriceU 
				   {
			get {
				return _currentPriceU; 
			}
			set {
				_currentPriceU = value;
				sp_today.merge(value/unit);
			}
		}

		public void onNewDay(Time date) {			
			int start = sp_today.end;
			log.daylyExit(sp_today);
			// for new day
			sp_today.init(start);
			log.daylyEnter(date,start);
		}

		public int averageBoughtPrice {	get { return averageBoughtPriceU/unit; } }

		public int averageBoughtPriceU {
			get {
				if (numOwn==0) 
					return 0;
				else
					return (int)(moneySpend / numOwn);
			}
		}

		public int benefitPerStock { get { return benefitPerUnit/unit; } }

		public int benefitPerUnit {
			get {
				if (numOwn==0) 
					return 0;
				else
					return currentPriceU - averageBoughtPriceU 
							- StockCompanyModule.calcCommition(currentPriceU,1);
			}
		}

		public double dividendRatio {
			get {
				return ((double)dividendU)/currentPriceU;
			}
		}

		//public long this[XAxisStyle scale, int index]{ get{return 0;} }
		public IStockPrice this[XAxisStyle scale, int index]
		{			
			get { 
				StockPrice prev = (StockPrice)log[scale,index];				
				// merge latest price today
				if( index == 0 ) prev.merge( sp_today );

				return prev; 
			}
		}

		public DataUpdateListener onUpdate { 
			get { return log.onUpdate; } 
			set { log.onUpdate = value; } 
		}

		public DataRange getDataRange(XAxisStyle scale) {
			((StockPrice)log[scale,0]).merge( sp_today );
			return log.getDataRange(scale);
		}


		internal bool buy(int amount)
		{
			if( amount > numInMarket )
				amount = numInMarket;
			if( amount <= 0 ) return false;

			int c = StockCompanyModule.calcCommition(currentPriceU,amount);
			long v = amount*currentPriceU+c;
			manager.spend(v,AccountGenre.OTHERS);

			bool r = ( _numOwn == 0 );
			moneySpend += v;
			_numOwn += amount;
			_numInMarket -= amount;
			// raise the price according to the bought amount
			currentPriceU += _currentPriceU*amount/numTotal;
			return r;
		}

		internal bool sell(int amount)
		{
			if( amount > numOwn )
				amount = numOwn;
			if( amount <= 0 ) return false;

			int c = StockCompanyModule.calcCommition(currentPriceU,amount);
			long v = amount*currentPriceU-c;
			manager.earn(v,AccountGenre.OTHERS);
			int prev = averageBoughtPriceU;
			_numOwn -= amount;
			_numInMarket += amount;
			bool r = ( _numOwn == 0 );
			// lower the price according to the bought amount
			currentPriceU -= _currentPriceU*amount/numTotal;
			moneySpend = prev*_numOwn;
			return r;
		}

		// should be called when StockData deserialized.
		internal void onDeserialized()
		{
			log.restoreListener();
		}

	}

}
