using System;
using System.Windows.Forms;

namespace freetrain.world.soccerstadium
{
	/// <summary>
	/// A game of soccer and its record.
	/// </summary>
	[Serializable]
	internal class Game
	{
		/// <summary> opponent. </summary>
		public readonly OpponentTeam opponent;
		
		/// <summary> Date of the game </summary>
		public readonly Time date;
		
		/// <summary> Stadium where the game will be held. </summary>
		public readonly StadiumStructure stadium;
		
		/// <summary> Score of the game. </summary>
		private string _score;
		public string score { get { return _score; } }

		private int _audience;
		public int audience { get { return _audience; } }

		/// <summary>
		/// Schedule a new game.
		/// </summary>
		/// <param name="opponent">the opponent team</param>
		/// <param name="time">the date/time of the game</param>
		public Game( StadiumStructure _stadium, OpponentTeam _opponent, TimeLength _time ) {
			this.stadium = _stadium;
			this.opponent = _opponent;
			this.date = World.world.clock + _time;

			World.world.clock.registerOneShot( new ClockHandler(onGameDate), _time );
		}

		public Game( StadiumStructure _stadium, TimeLength _time )
			: this( _stadium, OpponentTeam.drawRandom(), _time ) {}


		public void onGameDate() {
			// the date of the game.
			// play the game!
			_score = stadium.playGame(this);
			_audience = Const.rnd.Next(10000)+20000+stadium.popularity*50;	// TODO: incorporate popularity

			// erase the record after two weeks.
			World.world.clock.registerOneShot( new ClockHandler(onTimeout),
				TimeLength.fromDays(Const.DAYS_RECORD_EFFECTIVE) );
		}

		public void onTimeout() {
			stadium.timeoutGame(this);
		}

		/// <summary>
		/// Creates a list item for the property dialog.
		/// </summary>
		public ListViewItem createListItem() {
			// TODO: should be moved to StadiumPropertyDialog?
			
			ListViewItem lvi = new ListViewItem( date.month+"/"+date.day );
			lvi.SubItems.Add(opponent.name);

			if( score!=null ) {
				// past games
				lvi.SubItems.Add(score);
				lvi.SubItems.Add(audience.ToString());
			}

			return lvi;
		}
	}
}
