using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using freetrain.world;
using freetrain.util;
using freetrain.framework.plugin;

namespace freetrain.finance.stock
{
	public delegate void EconomyListener( Company com );
	/// <summary>
	/// The Economy
	/// </summary>
	[Serializable]
	public class Economy : ITrendTarget
	{
		[NonSerialized]
		static private Set files = new Set();

		private static Economy _theInstance = null;
		public static Economy theInstance { get { return _theInstance; 	} }

		#region ITrendTarget
		public string name { get{ return "Economy"; }}
		//! public string name { get{ return "経済"; }}
		public Trend trend { get{ return businessTrend;}}
		#endregion

			static private StockCompanyModule market { get { return StockCompanyModule.theInstance; } }
		static private Clock clock { get { return World.world.clock; } }
		static internal RandomEx rand { get { return _random; } }
		static private RandomEx _random = new RandomEx();
		// Events
		static public EventManager EconomicalEvents { get { return theInstance._events; } }
		private  EventManager _events;
		// Business Types
		static public BusinessTypes Businesses { get { return theInstance._businessTypes; } }
		private  BusinessTypes _businessTypes;
		// Companies
		static public ListedCompanies Companies { get { return theInstance._companies; } }		
		private ListedCompanies _companies;
		public Trend businessTrend;

		public static readonly int MajorUpdateIntervalHours = 6;
		public static readonly int MinorUpdateIntervalHours = 6;
		public static readonly int MajorUpdateStartingHour = 1;
		public static readonly int MinorUpdateStartingHour = 4;
		public static int UpdateTimesInADay 
		{
			get{ return 24/MajorUpdateIntervalHours+12/MinorUpdateIntervalHours; } 
		}
		private ClockHandler MajorUpdateEvent = null;
		private ClockHandler MinorUpdateEvent = null;
		private int phase1;
		private int phase2;

		[NonSerialized]
		static public EconomyListener onUpdate;

		static public void init(XmlElement e )
		{		
			// load data files.
			IEnumerator ie = e.ChildNodes.GetEnumerator();
			while( ie.MoveNext() )
			{
				XmlNode node = (XmlNode)ie.Current;
				if( node.Name.Equals("data") )
				{
					string filename = null;
					try	
					{
						filename = node.Attributes["file"].Value;
					} 
					catch	
					{
						throw new XmlException("missing file name ",null);
					}
					if( filename != null ) 
					{
						files.add(filename);
					}
				}
			}
			if( _theInstance == null ) 
			{
				_theInstance = new Economy();
			}
		}

		private void setClockHandlers() {
			//long today_h = TimeLength.pastMinutesToday().totalMinutes;
			long today_h = clock.hour;
			long today_m = today_h*Time.HOUR+clock.minutes;

			// MajorEvent occures both day and night. Generate trend making events.
			if(MajorUpdateEvent != null ) 
				clock.unregister(MajorUpdateEvent);
			MajorUpdateEvent = new ClockHandler(onMajorUpdate);
			long major = MajorUpdateStartingHour;
			while( major<today_h )
				major += MajorUpdateIntervalHours;
			clock.registerRepeated(MajorUpdateEvent, 
				TimeLength.fromMinutes(major*Time.HOUR-today_m), 
				TimeLength.fromHours(MajorUpdateIntervalHours));

			// MinorEvent dispached only in the business hours. Progresses trend only.
			if(MinorUpdateEvent != null ) 
				clock.unregister(MinorUpdateEvent);
			MinorUpdateEvent = new ClockHandler(onMinorUpdate);
			long minor = MinorUpdateStartingHour;
			while( minor<today_h )
				minor += MinorUpdateIntervalHours;
			clock.registerRepeated(MinorUpdateEvent, 
				TimeLength.fromMinutes(minor*Time.HOUR-today_m),
				TimeLength.fromHours(MinorUpdateIntervalHours));

		}

		// Load company data or business type data from xml document.
		private static void loadData(XmlNode node) 
		{
			IEnumerator ie = node.ChildNodes.GetEnumerator();
			while(ie.MoveNext())
			{
				XmlNode cn = (XmlNode)ie.Current;
				if(cn.Name.Equals("business")) _theInstance._businessTypes.Add(new BusinessType(cn));
				if(cn.Name.Equals("company")) _theInstance._companies.Add(new Company(cn));
			}
		}

		// Prepare for the new world
		static public void onNewWorld(string key) 
		{		
			Plugin p = StockMarketPlugIn.theInstance.parent;
			Businesses.Clear();
			Companies.Clear();
			_theInstance._events.onNewWorld();
			IEnumerator ie = files.GetEnumerator();
			while(ie.MoveNext())
			{
				string filename = (string)ie.Current;
				using( Stream file = p.loadStream(filename) ) 
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(file);
					IEnumerator ie2 = doc.ChildNodes.GetEnumerator();
					while(ie2.MoveNext())
					{
						XmlNode cn = (XmlNode)ie2.Current;
						if(cn.Name.Equals("companies")) loadData(cn);
						else if(cn.Name.Equals("events")) _theInstance._events.loadData(cn);
					}
				}
			} 
			initTable();
			theInstance.setClockHandlers();
			World.world.otherObjects[key] = theInstance;
			World.world.otherObjects[key+"rand"] = _random;

		}
		
		// Deserialize instance of economy.
		static public void onWorldLoaded(string key) 
		{
			_theInstance = (Economy)World.world.otherObjects[key];
			_random = (RandomEx)World.world.otherObjects[key+"rand"];
			_theInstance._businessTypes.onWorldLoaded();
			_theInstance._companies.onWorldLoaded();
			initTable();
		}

		static private void initTable()
		{
		}

		private Economy() 
		{
			onUpdate += new EconomyListener(StocksListHelper.onUpdateMarket);
			_businessTypes = new BusinessTypes();
			_companies = new ListedCompanies();
			_events = new EventManager();
			//_random = new RandomEx();
			businessTrend = Trend.randomGenerate(500,500,90);
		}

		// MajorEvent occures both day and night. Generate trend making events.
		public void onMajorUpdate()
		{
			if( clock.hour == MajorUpdateStartingHour ) 
			{ // begining of the day.
				Companies.onNewDay(clock);
				_events.update();
				if( clock.day == 1 ) 
				{ // begining of the month.
					updateComAccounts();
				}
			}
			progressTrends();
			if( onUpdate != null )
				onUpdate(null);
		}

		// MinorEvent dispached only in the business hours. Progresses trend only.
		public void onMinorUpdate()	{
			progressTrends();
			if(StockMarketConfig.isBusinessHour) 
			{
				if( onUpdate != null )
					onUpdate(null);
			}
		}

		private void progressTrends() {
			businessTrend.progress();
			Businesses.progress();
			Companies.progress();
		}

		static public void runEconomy() {
			if( _theInstance == null )
				_theInstance = new Economy();
			_theInstance.mapCompanies();
			_theInstance.updateComAccounts();
		}

		// initialize sin phase of business trend.
		protected void initParams()
		{
			phase1 = rand.Next2D((int)(Clock.YEAR*10-Clock.DAY*29));
			phase2 = rand.Next2D((int)(Clock.YEAR*30-Clock.DAY*19));
		}

		protected void mapCompanies()
		{
			IEnumerator e = Companies.GetEnumerator();
			while(e.MoveNext()) 
			{
				Company com = (Company)e.Current;
				com.type.companies.add(com);
			}
		}

		// Recaluculate accounts of each comapnies.
		protected void updateComAccounts()
		{
			recalcSuppliers();
			IEnumerator e = Companies.GetEnumerator();
			while(e.MoveNext()) 
			{
				Company com = (Company)e.Current;
				com.calcAccounting();
			}
		}

		protected void recalcSuppliers()
		{
			Economy.Businesses.ResetSuppliers();
			IEnumerator e = Companies.GetEnumerator();
			while(e.MoveNext()) 
			{
				Company com = (Company)e.Current;
				com.type._mkt_supplied += com._salesIdeal;
				com.type._totalScore += (long)(com._salesIdeal*com.calcScore());
			}
		}

	}

	/// <summary>
	/// The Normal distribution like random generator 
	/// </summary>
	[Serializable]
	public class RandomEx : Random
	{
		public int Next2D(int halfRange)
		{
			halfRange++;
			return Next(halfRange)-Next(halfRange);
		}

		public int Next2DExt(int halfRange)
		{
			int r = Next2D(halfRange);
			int r2 = r;
			if( r < 0 )
				while( halfRange == (r = Math.Abs( Next2D(halfRange))) )
					r2 += r;
			else
				while( -halfRange == (r = Math.Abs( Next2D(halfRange))) )
					r2 -= r;
			return r2;
		}

		public double NextDouble2D(double halfWidth, int split)
		{
			return Next2D(split)*halfWidth/split;
		}
	}
}
