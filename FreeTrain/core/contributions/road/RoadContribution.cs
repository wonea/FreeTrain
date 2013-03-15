using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using freetrain.contributions.common;
using freetrain.framework.graphics;
using freetrain.world;
using freetrain.world.road;

namespace freetrain.contributions.road
{
	/// <summary>
	/// Road for cars/buses
	/// </summary>
	[Serializable]
	public abstract class RoadContribution : LineContribution
	{
		protected RoadContribution( XmlElement e ) : base("road",e.Attributes["id"].Value) {
			XmlNode nd = e.SelectSingleNode("style");
			if(nd==null)
				style = RoadStyle.NullStyle;
			else
			{
				MajorRoadType mt = MajorRoadType.unknown;
				XmlAttribute major = nd.Attributes["name"];
				if(major!=null)
					mt = (MajorRoadType)Enum.Parse(mt.GetType(),major.Value,true);
				SidewalkType st = SidewalkType.none;
				XmlAttribute  sidewalk = nd.Attributes["sidewalk"];
				if(sidewalk!=null)
					st = (SidewalkType)Enum.Parse(st.GetType(),sidewalk.Value,true);
				int l = 0;
				XmlAttribute lanes = nd.Attributes["lanes"];
				if(lanes!=null)
					l = int.Parse(lanes.Value);
				style = new RoadStyle(mt,st,l);
			}

		}

		public readonly RoadStyle style;
		
		private int previewPatternIdx = 0;
		public int PreviewPatternIdx
		{ 
			get{ return previewPatternIdx; }
			set{ previewPatternIdx=value%3; } 
		}
		public static readonly byte[,,] previewPattern = 
		{
			{
				{00,00,00,10,00,00,00,00,00,00},
				{00,00,00,10,00,00,00,00,00,00},
				{00,00,00,10,00,08,00,00,00,00},
				{00,00,00,06,05,11,00,00,00,00},
				{00,00,00,00,00,10,00,00,12,03},
				{00,00,00,00,00,10,00,12,03,00},
				{05,05,05,05,05,15,05,03,00,00},
				{00,00,00,00,00,02,00,00,00,00},
				{00,00,00,00,00,00,00,00,00,00},
				{00,00,00,00,00,00,00,00,00,00}
			},
			{
				{00,00,00,00,00,00,00,00,00,00},
				{00,00,00,00,00,00,00,00,00,00},
				{00,05,05,05,05,13,05,05,05,05},
				{00,00,00,00,00,10,00,00,00,00},
				{00,00,00,00,12,15,13,01,00,00},
				{00,00,00,00,14,15,11,00,00,00},
				{00,00,00,00,06,07,03,00,00,00},
				{00,00,00,00,00,00,00,00,00,00},
				{00,00,00,00,00,00,00,00,00,00},
				{00,00,00,00,00,00,00,00,00,00}
			},
			{
				{00,00,00,00,00,00,00,00,00,00},
				{00,00,00,00,00,00,00,00,00,00},
				{00,00,00,00,00,00,00,00,00,00},
				{00,00,00,00,00,00,08,00,00,00},
				{00,00,00,00,00,00,10,00,00,00},
				{00,00,00,04,05,01,10,00,00,00},
				{00,00,00,00,00,00,10,00,00,00},
				{00,00,00,00,00,00,02,00,00,00},
				{00,00,00,00,00,00,00,00,00,00},
				{00,00,00,00,00,00,00,00,00,00}
			}
		};		// roads are always 4-way.
		public override sealed DirectionMode directionMode 
		{
			get {
				return DirectionMode.FourWay;
			}
		}
	}
}
