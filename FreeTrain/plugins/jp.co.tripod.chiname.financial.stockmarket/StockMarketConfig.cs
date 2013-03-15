using System;
using System.Xml;
using freetrain.framework.plugin;
using freetrain.world;
using freetrain.world.accounting;

namespace freetrain.finance.stock
{
	/// <summary>
	/// Configuration parameters for Bank. Read from plugin.xml and
	/// remain constant across the game.
	/// </summary>
	internal sealed class StockMarketConfig
	{
		private static bool _debug;
		private static int _openHour = 9;
		private static int _closeHour = 17;
		public static bool debug { get{ return _debug; } }
		// The hour that business hour starts.
		public static int openHour { get{ return _openHour; } }
		// The hour that business hour ends.
		public static int closeHour { get{ return _closeHour; } }
		
		public static int[] eventDaySpan { get { return eventSpans; } }
		private static int[] eventSpans
			= new int[8]{0,30,20,10,8,4,2,1};

		// Returns true if today is holiday
		static public bool isHoliday(Time _time) { return _time.isWeekend; } 
		
		internal static void init( XmlElement e ) {
			XmlNode xdebug = null;
			try
			{
				xdebug = XmlUtil.selectSingleNode(e,"debug");
			}
			catch{}
			_debug = (xdebug!=null);

			XmlElement settings = (XmlElement)XmlUtil.selectSingleNode(e,"settings");
			foreach( XmlNode node in settings.ChildNodes ) {
				if( node.Name.Equals("businessHour") )
				{
					try 
					{
						XmlAttribute a;
						a = node.Attributes["open"];
						if( a != null ) 
						{
							_openHour = int.Parse(a.Value);
							if( _openHour<0 ) _openHour = 0;
							if( _openHour>24 ) _openHour = 24;
						}
						a = node.Attributes["close"];
						if( a != null ) 
						{
							_closeHour = int.Parse(a.Value);
							if( _closeHour<0 ) _closeHour = 0;
							if( _closeHour>24 ) _closeHour = 24;
						}
					}
					catch 
					{
						throw new XmlException("invalid argument in 'bussinesHour'",null);
					}
				}
				else if( node.Name.Equals("eventSpans") )
				{
					string s = node.InnerText.Trim();
					string[] terms = s.Split(',');
					int n = terms.Length-1;
					int i=1;
					while(n>=0&&i<7)
					{
						string t = terms[n--];
						int l = t.Length;
						int v = 1;
						switch( t[l-1] )
						{
							case 'y':
							case 'Y':
								v = 365;
								l--;
								break;
							case 'm':
							case 'M':
								v = 30;
								l--;
								break;
							case 'd':
							case 'D':
								l--;
								break;
						}
						eventSpans[i++]=int.Parse(t.Substring(0,l))*v;
					}
				}
			}
		}


		// Returns true if today is holiday 
		static public bool isBusinessHour 
		{
			get{
				if( isHoliday(clock) ) return false;
				int hour = clock.hour;
				return ( hour>openHour && hour<closeHour );
			} 
		}
	
		// The TimeLength to the nearest business hour. (zero if the bank opens now).
		static public TimeLength spanToOpen 
		{
			get {
				Time enter = clock;
				enter += TimeLength.fromHours(StockMarketConfig.openHour-clock.hour);
				enter += TimeLength.fromMinutes(-clock.minutes);

				int hour = World.world.clock.hour;
				if( hour>closeHour ) 
					enter += TimeLength.fromDays(1); // Wait for a day.
				while( isHoliday(enter) )
					enter += TimeLength.fromDays(1); // Wait for a day.
				return enter-clock; 
			}
		}

		static private AccountManager manager { get { return World.world.account; } }
		static private Clock clock { get { return World.world.clock; } }
	}
}
