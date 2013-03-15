using System;
using System.Diagnostics;

namespace freetrain.world.rail
{
	public delegate void TransportEvent(Station to, int amount);
	/// <summary>
	/// GlobalStationListener の概要の説明です。
	/// </summary>
	public class GlobalTrafficMonitor
	{
		static private GlobalTrafficMonitor theInstance = new GlobalTrafficMonitor();
		private GlobalTrafficMonitor(){}
		static internal GlobalTrafficMonitor TheInstance{ get{ return theInstance; } }			

		public TransportEvent OnPassengerTransported;

		internal void NotifyPassengerTransport(Station to, int amount){
			Debug.WriteLine(string.Format("Transport {0} passengers to [{1}].",amount,to));
			if(OnPassengerTransported!=null && amount!=0)
				OnPassengerTransported(to,amount);
		}
	}
}
