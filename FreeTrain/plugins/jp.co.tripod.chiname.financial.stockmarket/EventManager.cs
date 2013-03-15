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
	[Serializable]
	public class EventManager
	{
		//private Hashtable hash = new Hashtable();
		private ArrayList[] array = new ArrayList[8];
//		private static readonly int[] eventDaySpan
//			= new int[7]{0,30,20,10,8,4,2};
		private long[] nextX_Day = new long[8];

		public EventManager()
		{
			for(int i=0; i<8; i++ ) 
				array[i] = new ArrayList();
		}

		public void loadData(XmlNode node) 
		{
			for(int i=0; i<8; i++ ) 
				array[i].Clear();
			IEnumerator ie = node.ChildNodes.GetEnumerator();
			while(ie.MoveNext())
			{
				XmlNode cn = (XmlNode)ie.Current;
				if(cn.Name.Equals("define")) {
					string type = cn.Attributes["type"].Value;
					if( type.Equals("event") ) 
					{
						EconomicalEvent ee = EconomicalEvent.Parse(cn);
						array[ee.frequency].Add(ee);
					}
				}
			}
		}

		public void onNewWorld() 
		{
			for(int i=0; i<8; i++ ) 
				nextX_Day[i] =	World.world.clock.day+Economy.rand.Next(StockMarketConfig.eventDaySpan[i]);
		}

		public void update()
		{
			long day = World.world.clock.totalMinutes/Clock.DAY;
			for( int i=1; i<8; i++ ) {
				if(nextX_Day[i] <= day ) {
					// fire some event
					if( array[i].Count>0 ) {
						int j = Economy.rand.Next(array[i].Count);
						Debug.Write("###fireEvent:"+i+"/"+j+"###");
						EconomicalEvent e = (EconomicalEvent)array[i][j];
						e.execute();
						Debug.WriteLine(e.getLatestEventMessage());
						// determins next event date
						nextX_Day[i] += StockMarketConfig.eventDaySpan[i]
							+Economy.rand.Next2D(StockMarketConfig.eventDaySpan[i]/2);

						// fire followig event
						if( e.followedEvent != null )
						{
							e.followedEvent.execute();
							Debug.WriteLine("###fireChildEvent###");
							Debug.WriteLine(e.followedEvent.getLatestEventMessage());
						}
					}
				}
			}
		}
	}
}
