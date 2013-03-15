using System;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using freetrain.framework.plugin;
using freetrain.util;
using freetrain.world;

namespace freetrain.finance.stock
{
	[Serializable]
	public class ListedCompanies
	{
		private Hashtable hash = new Hashtable();
		[NonSerialized]
		private ArrayList array = new ArrayList();

		public void Add(Company com) {
			hash.Add(com.id,com);
			array.Add(com);
		}
		
		// progress trend calculation
		internal void progress() 
		{
			IEnumerator e = hash.Values.GetEnumerator();
			while(e.MoveNext()) 
			{
				Company com = (Company)e.Current;
				com.progress();
			}			
		}

		internal void onWorldLoaded() 
		{
			// reconstruct 'array' field because it is non-serialized.
			array = new ArrayList();
			array.AddRange(hash.Values);
			// reattach min-max listener for StockPriceLogger
			foreach( Company com in array )
			{
				com.stockData.onDeserialized();
			}
		}

		internal void onNewDay(Time date) 
		{
			IEnumerator e = hash.Values.GetEnumerator();
			while(e.MoveNext()) 
			{
				Company com = (Company)e.Current;
				com.stockData.onNewDay(date);
			}			
		}

		public Company this[string id] {
			get	{
				if( hash.ContainsKey(id) )
					return (Company)hash[id];
				else
					return Company.Null;
			}
		}

		public Company this[int index] 
		{
			get	
			{
				if( array.Count > index )
					return (Company)array[index];
				else
					return Company.Null;
			}
		}

		public IEnumerator GetEnumerator() {
			return hash.Values.GetEnumerator();
		}

		public int Count {get {return array.Count;} }

		public void Clear() {
			hash.Clear();
			array.Clear();
		}
	}

	/// <summary>
	/// Company の概要の説明です。
	/// </summary>
	[Serializable]
	public class Company : ITrendTarget
	{
		public readonly string id;
		public readonly string _name;
		internal readonly string _type;
		public StockData stockData;
		public BusinessType type { get { return Economy.Businesses[_type]; } }
		internal Trend _trend = new Trend();

		#region ITrendTarget
		public string name { get{ return _name; }}
		public Trend trend { get{ return _trend;}}
		#endregion

		internal long _capital;
		internal long _salesIdeal;
		[NonSerialized]
		internal long _salesReal;
		[NonSerialized]
		internal long _benefit;
		[NonSerialized]
		internal long _invest;
		[NonSerialized]
		internal long _fixedExpense;
		[NonSerialized]
		internal double _scoreNormal;
		[NonSerialized]
		internal double _scoreFuture;

		internal int _brand = 5;
		internal int _marketing = 5;
		internal int _quality = 5;
		internal int _rationality = 5;
		internal int _development = 5;

		private Company()
		{
			id = "";
			_name = "Invalid";
			//! _name = "無効";
			_type = "";
			_capital = _benefit = _salesIdeal = _salesReal =0;
			_scoreNormal = 0;
			stockData = new StockData();
		}

		public Company(XmlNode node)
		{
			try 
			{
				id = node.Attributes["id"].Value;
				_name = node.Attributes["name"].Value;
				Debug.WriteLine(id+":"+name);
				_type = node.Attributes["type"].Value;
			} 
			catch 
			{
				throw new XmlException("invalid company definition (in company tag) : "+id+name,null);
			}
			try 
			{
				XmlElement performance = (XmlElement)XmlUtil.selectSingleNode(node,"performance");
				_capital = long.Parse(performance.Attributes["capital"].Value);
				_salesIdeal = long.Parse(performance.Attributes["sales"].Value);
			}			 
			catch 
			{
				throw new XmlException("invalid company definition (in performance tag) : "+id+name,null);
			}

			try 
			{
				XmlElement capacity = (XmlElement)XmlUtil.selectSingleNode(node,"capacity");
				_brand = int.Parse(capacity.Attributes["brand"].Value);
				_marketing = int.Parse(capacity.Attributes["marketing"].Value);
				_quality = int.Parse(capacity.Attributes["quality"].Value);
				_rationality = int.Parse(capacity.Attributes["rationality"].Value);
				_development = int.Parse(capacity.Attributes["development"].Value);
			}
			catch 
			{
				throw new XmlException("invalid company definition (in capacity tag) : "+id+name,null);
			}
			try 
			{

				XmlElement stock = (XmlElement)XmlUtil.selectSingleNode(node,"stock");
				int total = int.Parse(stock.Attributes["total"].Value);
				int share = int.Parse(stock.Attributes["share"].Value);
				stockData = new StockData(total,share);
			}
			catch 
			{
				throw new XmlException("invalid company definition (in stock tag) : "+id+name,null);
			}
			//try 
			//{
			//	updateStockPrice();
			//	trend.setParams(0,5);			
			//}
			//catch 
			//{
			//	throw new XmlException("invalid company definition (invalid data) : "+id+name,null);
			//}
// FIX ME IMPORTANT
		}
		
		// update stock price;
		protected void updateStockPrice()
		{
			long p = stockData._properPriceU;
			//if( StockMarketConfig.debug )
			//	Debug.WriteLine(name+":"+trend.currentLevel.ToString()+"/"+type.trend.currentLevel.ToString()+"/"+Economy.theInstance.businessTrend.currentLevel.ToString());
			p = trend.apply(p);
			p = type.trend.apply(p);
			p = Economy.theInstance.businessTrend.apply(p,type._depend/10.0);
			stockData.currentPriceU = (int)p;
		}

		// the score is in the range between 0 to 1;
		internal double calcScore()
		{
			long s = 0;
			s += type.weightBrand*_brand*_brand;
			s += type.weightMarketing*_marketing*_marketing;
			s += type.weightQuality*_quality*_quality;
			_scoreNormal = ((double)s)/type.weightTotal/100;
			return _scoreNormal;
		}

		internal void calcAccounting()
		{
			int prevPrice = stockData.currentPriceU;
			if( type._mkt_scale > type._mkt_supplied ) 
			{
				_salesReal =(long)(_salesIdeal*(_scoreNormal+3.5)/4);
			}
			else 
			{
				_salesReal = (long)(_salesIdeal*(_scoreNormal+1.5)/2);
				_salesReal = (long)(type.marketScale*_scoreNormal*_salesReal)/type._totalScore;
				//if( _salesReal > _salesIdeal ) _salesReal = _salesIdeal;
			}
			_fixedExpense = (long)(_salesIdeal*type._acc_fixed);
			_benefit = (_salesReal - _fixedExpense )*_rationality/10 ;
			// set dividendU
			if(_benefit > 0) {
				double invrate = ((double)_development)/10.0;
				_invest = (long)(_benefit * invrate);
				_benefit -= _invest;
				stockData._dividendU = ((double)_benefit)/(100*stockData._numTotal);
			}
			else
				stockData._dividendU = 0;
			// evaluate future prospects.
			_scoreFuture = ((double)_scoreNormal*_development)/10;
			double scoreFuture = (_scoreFuture*type._progress + _scoreNormal*(10-type._progress))/10;
			double shareF = (scoreFuture*_salesIdeal)/type._totalScore;
			long salesFuture = (long)(type.marketScale*shareF);
			//_scoreFuture = salesFuture;
			stockData._properPriceU = (int)(salesFuture/(100*stockData._numTotal));
			updateStockPrice();
			int gap = stockData.currentPriceU-prevPrice;
			//Debug.WriteLine("GAP-"+name+":"+(gap));
			trend.setGap(gap,15);
		}

		// Recalculate trend and stock price.
		internal void progress() {
			trend.progress();
			updateStockPrice();
		}

		private static Company _Null = null;
		public static Company Null	
		{
			get 
			{
				if(_Null == null )
					_Null = new Company();
				return _Null;
			}
		}
	}
	
}
