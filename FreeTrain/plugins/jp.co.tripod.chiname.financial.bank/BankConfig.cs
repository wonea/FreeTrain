using System;
using System.Xml;
using freetrain.framework.plugin;
using freetrain.world;
using freetrain.world.accounting;

namespace freetrain.views.bank
{
	/// <summary>
	/// Configuration parameters for Bank. Read from plugin.xml and
	/// remain constant across the game.
	/// </summary>
	internal sealed class BankConfig
	{
		private static int _openHour = 9;
		private static int _closeHour = 17;
		// The hour that business hour starts.
		public static int openHour { get{ return _openHour; } }
		// The hour that business hour ends.
		public static int closeHour { get{ return _closeHour; } }

		// Returns true if today is holiday
		static public bool isHoliday(Time _time) { return _time.isWeekend; } 

		private static bool _canDeposit = true;
		private static bool _canBorrow = true;
		private static bool _canRepay = false;
		private static bool _canCancel = false;

		public static bool canDeposit	{ get { return _canDeposit; } }
		public static bool canBorrow	{ get { return _canBorrow; } }
		public static bool canRepay		{ get { return _canRepay; } }
		public static bool canCancel	{ get { return _canCancel; } }
		
		internal static void init( XmlElement e ) {
			XmlElement settings = (XmlElement)XmlUtil.selectSingleNode(e,"settings");
			foreach( XmlElement node in settings.ChildNodes ) {
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
				else if( node.Name.Equals("debt"))
				{
					XmlAttribute a;
					a = node.Attributes["available"];
					if( a != null && a.Value.Equals("false") ) 
						_canBorrow = false;
					else {
						a = node.Attributes["repayable"];
						if( a != null && a.Value.Equals("true") ) 
							_canRepay = true;
					}
				}
				else if( node.Name.Equals("deposit"))
				{
					XmlAttribute a;
					a = node.Attributes["available"];
					if( a != null && a.Value.Equals("false") ) 
						_canDeposit = false;
					else {
						a = node.Attributes["cancelable"];
						if( a != null && a.Value.Equals("true") ) 
							_canCancel = true;
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
				enter += TimeLength.fromHours(BankConfig.openHour-clock.hour);
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
