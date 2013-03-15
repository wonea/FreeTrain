using System;
using freetrain.framework.plugin;
using freetrain.world.accounting;

namespace freetrain.world.soccerstadium
{
	internal class Const
	{
		internal static readonly Random rnd = new Random();

		/// <summary>
		/// Accounting genre for soccer team management.
		/// </summary>
		public static AccountGenre GENRE {
			get {
				return (AccountGenre)PluginManager.theInstance
					.getContribution("{2669F15B-66F2-4c77-8A48-1A20783EB9F5}");
			}
		}

		/// <summary>
		/// We will book games up to three weeks ahead.
		/// </summary>
		internal const int SCHEDULE_DAYS = 21;

		/// <summary>
		/// Keep records for about a month.
		/// </summary>
		internal const int DAYS_RECORD_EFFECTIVE = 28;
	}
}
