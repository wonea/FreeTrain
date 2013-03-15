using System;

namespace RoadSimulator
{
	public enum BarrierType : int {Barrier,Building,Station};

	/// <summary>
	/// 
	/// </summary>	
	public class Barrier
	{
		//private int _population=0;
		private BarrierType _type;
		public BarrierType type { get{ return _type; } }

		public Barrier(BarrierType type)
		{
			_type = type;
		}
	}

	public class Station : Barrier
	{
		private int _level;
		public int level { get{ return _level; } }

		public Station(int level) : base(BarrierType.Station)
		{
			this._level = level;			
		}
	}
}
