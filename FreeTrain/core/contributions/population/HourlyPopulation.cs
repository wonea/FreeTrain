using System;
using System.Diagnostics;
using freetrain.world;
using freetrain.world.structs;
using freetrain.framework.plugin;

namespace freetrain.contributions.population
{
	/// <summary>
	/// Population depends on hour of the day
	/// </summary>
	[Serializable]
	public class HourlyPopulation : Population
	{
		public HourlyPopulation( int basep, int[] weekdayHourTable, int[] weekendHourTable ) {
			Debug.Assert( weekdayHourTable.Length==24 || weekendHourTable.Length==24 );

			this.population = basep;
			this.weekdayHourTable = weekdayHourTable;
			this.weekendHourTable = weekendHourTable;
		}

		/// <summary>
		/// Ration of each hour in percentage
		/// </summary>
		private readonly int[] weekdayHourTable;
		private readonly int[] weekendHourTable;
		private readonly int population;

		public override int residents { get { return population; } }

		public override int calcPopulation( Time currentTime ) {
			if( currentTime.isWeekend )
				return population * weekendHourTable[currentTime.hour] / 100;
			else
				return population * weekdayHourTable[currentTime.hour] / 100;
		}

	}
}
