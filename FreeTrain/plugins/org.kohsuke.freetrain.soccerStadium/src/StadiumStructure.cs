using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using freetrain.contributions.structs;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.world.accounting;
using freetrain.world.structs;
using freetrain.util;

namespace freetrain.world.soccerstadium
{
	/// <summary>
	/// Structure object for a soccer stadium.
	/// </summary>
	[Serializable]
	public class StadiumStructure : PopulatedStructure
	{
//		/// <summary>
//		/// Stadium structure contribution.
//		/// </summary>
//		private readonly new StructureContributionImpl type;

		/// <summary>
		/// Strength of the team (0-100). A stronger team wins often.
		/// </summary>
		private int _strength = 30;

		/// <summary>
		/// Popularity of the team (0-100). A popular team attracts more people.
		/// </summary>
		private int _popularity = 30;
		
		/// <summary> Name of the stadium. </summary>
		public string stadiumName;

		/// <summary> Name of the team. </summary>
		public string teamName;

		/// <summary>
		/// Upcoming games with the earlier ones at the head of the array.
		/// Should be treated as a read-only object from outside.
		/// </summary>
		public readonly IList futureGames = new ArrayList();

		/// <summary>
		/// Past games with the earlier ones at the head of the array.
		/// Should be treated as a read-only object from outside.
		/// </summary>
		public readonly IList pastGames = new ArrayList();


		/// <summary>
		/// Creates a new commercial structurewith its left-top corner at
		/// the specified location.
		/// </summary>
		/// <param name="_type">
		/// Type of the structure to be built.
		/// </param>
		public StadiumStructure( StructureContributionImpl _type, WorldLocator wloc  ) : base( _type, wloc ) {
//			this.type = _type;

			// register once a month timer for the strength/popularity decay
			World.world.clock.registerRepeated( new ClockHandler(onClock), TimeLength.fromDays(30) );

			// schedule initial games
			// minutes to the next day midnight
			TimeLength b = TimeLength.fromMinutes(
				TimeLength.ONEDAY.totalMinutes
				- ( World.world.clock.totalMinutes % TimeLength.ONEDAY.totalMinutes ));
			// the first game is set to 14:00 that day.
			b += TimeLength.fromHours(14);

			for( int i=0; i<Const.SCHEDULE_DAYS; i+=7 ) {
				scheduleNewGame( b );
				scheduleNewGame( b+TimeLength.fromHours(24*3+5) );	// schdule a nighter

				b += TimeLength.fromDays(7);
			}
		}
		
		public override string name { get { return stadiumName; } }


		public int strength { get { return _strength; } }
		public int popularity { get { return _popularity; } }

		// height-cut color
		protected override Color heightCutColor { get { return hcColor; } }
		private static Color hcColor = Color.FromArgb(51,115,179);

		public override bool isOwned { get { return true; } }
		public override bool isSilentlyReclaimable { get { return false; } }

		public override void remove() {
			base.remove();
			World.world.clock.unregister( new ClockHandler(onClock) );
		}




		public override bool onClick() {
			using( StadiumPropertyDialog prop = new StadiumPropertyDialog(this) ) {
				prop.ShowDialog(MainWindow.mainWindow);
			}
			return true;
		}

		public void onClock() {
			// both parameters decline streadily
			_popularity = Math.Max(0,_popularity-2);
			_strength   = Math.Max(0,_strength-2);
		}

		public void reinforce() {
			_strength = Math.Min(100,_strength+10);
			// we will charge anyway even if we are the strongest.
			AccountManager.theInstance.spend( 100000, Const.GENRE );
		}

		public void doPR() {
			_popularity = Math.Min(100,_popularity+10);
			// we will charge anyway even if we are the strongest.
			AccountManager.theInstance.spend( 100000, Const.GENRE );
		}

		/// <summary>
		/// Schedule a new game in the future.
		/// </summary>
		private void scheduleNewGame( TimeLength timeToGame ) {
			futureGames.Add( new Game( this, timeToGame ) );
		}

	//
	// called by the Game object.
	//
		internal string playGame( Game game ) {
			Debug.Assert( futureGames[0]==game );
			futureGames.RemoveAt(0);

			string score;

			// decide the score.
			int s1 = (int)Math.Floor( 6.0*Math.Pow(Const.rnd.NextDouble(),1.4) );
			int s2 = (int)Math.Floor( 6.0*Math.Pow(Const.rnd.NextDouble(),1.4) );
			
			// make sure s1 >= s2
			if( s2 > s1 ) {
				int t=s1; s1=s2; s2=t;
			}

			if(s1==s2) {
				// draw game. no bonus
				score = s1+"-"+s2;
			} else {
				// decide who won. 25% - 75%
				if( Const.rnd.Next(100) < (this.strength-game.opponent.strength)/5 + 50 ) {
					score = s1+"-"+s2;
					// won
					_popularity = Math.Min(100,_popularity+1);
				} else {
					score = s2+"-"+s1;
					// lost
					_popularity = Math.Max(0,_popularity-1);
				}
			}

			// move it to the "past games" list
			pastGames.Add(game);
			
			// schedule another game one week later
			scheduleNewGame( TimeLength.fromDays(Const.SCHEDULE_DAYS) );

			return score;
		}

		/// <summary>
		/// Erase a game from the records.
		/// Should be called only from a Game object.
		/// </summary>
		internal void timeoutGame( Game game ) {
			Debug.Assert(pastGames[0]==game);
			pastGames.RemoveAt(0);
		}
	}
}
