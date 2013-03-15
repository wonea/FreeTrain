using System;

namespace RoadSimulator
{
	/// <summary>
	/// 道路の芽
	/// </summary>
	internal class RoadBud
	{
		private int _level;
		private int _sleep;
		// 休眠中の芽？
		public bool sleeping{ get { return (_sleep>0); }}
		public int level { get { return _level; }}

		internal int[] lastBranch;
		// 芽吹き済み？
		internal bool sprouted;

		public RoadBud(int level)
		{
			this._level = level;
			_sleep = 0;
			sprouted = false;
			//lastBranch = new int[Configure.RoadLevelMax];
		}
		public RoadBud(int level, int sleep)
		{
			this._level = level;
			this._sleep = sleep;
			sprouted = false;
			//lastBranch = new int[Configure.RoadLevelMax];
		}
		internal void stepSleep() 
		{
			_sleep--;
		}
	}
}
