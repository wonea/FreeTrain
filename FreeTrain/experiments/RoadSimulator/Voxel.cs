using System;

namespace RoadSimulator
{
	/// <summary>
	/// 
	/// </summary>
	public class Voxel
	{
		public Voxel()
		{
			// 
			// TODO: コンストラクタ ロジックをここに追加してください。
			//
		}

		public int price;
		private Road _road = null;
		private Object _structure = null;
		public bool canBuildRoad{ get{ return (_structure == null); } }
		public bool canBuildStructure{ get{ return (_road == null); } }
		public Object structure{ get{ return _structure; } }
		public Road road{ get{ return _road; } }
		
		public void erase()
		{
			_road = null;
			_structure = null;
		}

		public void buildRoad(int v, Direction from, Direction to)
		{
			_road = new Road();
			_road.setLevel(from,v);
			_road.setLevel(to,v);
		}

		public void buildBarrier()
		{
			_structure = new Barrier(BarrierType.Barrier);
		}
		public void buildStation(int level)
		{
			_structure = new Station(level);
		}
		public void buildBuilding()
		{
			_structure = new Barrier(BarrierType.Building);
		}
	}
}
