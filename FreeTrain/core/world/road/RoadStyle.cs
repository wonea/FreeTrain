using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using freetrain.framework;
using freetrain.framework.graphics;

namespace freetrain.world.road
{
	public enum MajorRoadType:byte{ unknown, footpath, street, highway }
	public enum SidewalkType:byte{ none, shoulder, pavement }

	/// <summary>
	/// Road style.
	/// </summary>
	[Serializable]
	public struct RoadStyle
	{
		public readonly MajorRoadType Type;
		public readonly SidewalkType Sidewalk;
		private readonly byte _lanes;
		public int CarLanes { get{ return _lanes; }}
		private readonly byte _option;
		public RoadStyle(MajorRoadType mt, SidewalkType st, int lanes )
		{
			Type = mt;
			Sidewalk = st;
			_lanes = (byte)(lanes&0xff);
			_option = 0;
		}
		public RoadStyle(MajorRoadType mt, SidewalkType st, int lanes, int option )
		{
			Type = mt;
			Sidewalk = st;
			_lanes = (byte)(lanes&0xff);
			_option = (byte)(option&0xff);
		}
		public override string ToString()
		{
			return string.Format("[type={0},sidewalk={1},lanes={2}]",Type,Sidewalk,CarLanes);
		}

		static public readonly RoadStyle NullStyle = new RoadStyle(0,0,0,0);
	}

}
