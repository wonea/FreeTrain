using System;
using System.Diagnostics;
using System.Drawing;
using freetrain.framework;
using freetrain.framework.graphics;

namespace freetrain.world.rail.signal
{
	/// <summary>
	/// Rail road with signal
	/// </summary>
	[Serializable]
	public class SignalRailRoad : SpecialPurposeRailRoad, TrainHarbor
	{
		public SignalRailRoad( TrafficVoxel v, RailSignalContribution _type, Direction _dir )
			: base(v,_dir) {
			
			this.direction = _dir;
			this.type = _type;
			pattern = RailPattern.get( direction, direction.opposite );
			Debug.Assert( dir1.isSharp );
			Debug.Assert( dir2.isSharp );
		}

		private readonly Direction direction;

		private readonly RailSignalContribution type;

		public override TimeLength getStopTimeSpan( Train tr, int callCount ) {
			if( Direction.angle( tr.head.state.asInside().direction, direction )<=1 )
				return tr.controller.getStopTimeSpan(tr,this,callCount);
			else
				return TimeLength.ZERO;
		}

		public override void drawAfter( DrawContext display, Point pt) {
			if(!type.needsToDrawAfter(direction))
				type.getSprite(direction).draw( display.surface, pt );
		}

		public override void drawBefore( DrawContext display, Point pt) {
			if(type.needsToDrawAfter(direction))
				type.getSprite(direction).draw( display.surface, pt );
			
			pattern.draw(display.surface,pt);
		}

		public void remove() {
			new SingleRailRoad( voxel, pattern );
		}

		public override object queryInterface( Type aspect ) {
			if( aspect==typeof(TrainHarbor) )
				return this;

			return base.queryInterface(aspect);
		}

		public static new SignalRailRoad get( Location loc ) {
			return RailRoad.get(loc) as SignalRailRoad;
		}
	}
}
