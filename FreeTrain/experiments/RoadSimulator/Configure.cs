using System;

namespace RoadSimulator
{
	/// <summary>
	/// 環境設定
	/// </summary>
	public class Configure
	{
		//道路のレベル。ただし、0が一番太い道路。
		static public readonly int RoadLevelMax = 4;
		//駅前道路の駅からの平均距離。
		static public readonly int MeanDistanceFromStation = 4;
		//連続実行のウェイト。
		static public readonly int TimerInterval = 4;
		//枝をつくる最大道路レベル(これ以降のレベルの道路は枝分岐をしない)
		static public readonly int noTrunkLevel = 4;

		//駅のレベル＝駅の正面にできる道路のレベル。
		static private int stationLv = 1;
		static public int stationLevel 
		{
			get{ return stationLv; }
			set{ stationLv= Math.Max(0,Math.Min(RoadLevelMax,value)); }
		}

		private Configure()
		{
			// 
			// TODO: コンストラクタ ロジックをここに追加してください。
			//
		}
	}
}
