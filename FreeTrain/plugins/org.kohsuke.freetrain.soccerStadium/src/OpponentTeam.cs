using System;

namespace freetrain.world.soccerstadium
{
	/// <summary>
	/// Opponent team.
	/// </summary>
	[Serializable]
	public class OpponentTeam
	{
		/// <summary> Name of the team. </summary>
		public readonly string name;
		/// <summary> Strength of the team. </summary>
		public readonly int strength;

		private OpponentTeam( string _name, int _strength ) {
			this.name = _name;
			this.strength = _strength;
		}

		public static readonly OpponentTeam[] OPPONENTS = new OpponentTeam[]{
			new OpponentTeam("Chiba Knights",0),
			new OpponentTeam("Sunflash Hiroshima",10),
			new OpponentTeam("Yokohama Sailors",20),
			new OpponentTeam("Nagoya Dolphins",30),
			new OpponentTeam("Kashima Juniors",40),
			new OpponentTeam("Oita Trumpets",50),
			new OpponentTeam("Iwata Jewels",60),
			new OpponentTeam("Shimizu Spurs",70),
			new OpponentTeam("Osaka Gophers",80),
			new OpponentTeam("Kawasaki Front",90),
			new OpponentTeam("Urawa Mets",100)
			//! new OpponentTeam("ジェフユーナイトヘッド千葉",0),
			//! new OpponentTeam("サンフラッシュ広島",10),
			//! new OpponentTeam("横浜Ｆマリネーズ",20),
			//! new OpponentTeam("名古屋グランパズエイティ",30),
			//! new OpponentTeam("鹿島アントールダーズ",40),
			//! new OpponentTeam("大分トリニート",50),
			//! new OpponentTeam("ショベル磐田",60),
			//! new OpponentTeam("清水エスプラス",70),
			//! new OpponentTeam("ナンバ大阪",80),
			//! new OpponentTeam("川崎フラクターレ",90),
			//! new OpponentTeam("浦和メッツ",100)
		};


		/// <summary>
		/// Select one team randomly.
		/// </summary>
		public static OpponentTeam drawRandom() {
			return OPPONENTS[Const.rnd.Next(OPPONENTS.Length)];
		}
	}
}
