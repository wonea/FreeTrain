using System;
using System.Drawing;

namespace freetrain.world.road
{
	/// <summary>
	/// Automobile Road
	/// </summary>
	[Serializable]
	public abstract class Road
	{
		protected Road( TrafficVoxel tv, RoadPattern pattern, RoadStyle style ) {
			this._style = style;
			this.voxel = tv;
			this._pattern = pattern;
			voxel.road = this;
			// voxel.roadを設定してからOnVoxelChangedイベントを投げないと
			// 地価の係数計算が正しくできない。(477)
			World.world.fireOnVoxelChanged(tv.location);
		}

		/// <summary>
		/// Detailed Attribute of road
		/// </summary>
		internal protected RoadStyle _style;
		public RoadStyle style { get{ return _style; } }

		/// <summary>
		/// Occupied voxel 
		/// </summary>
		public readonly TrafficVoxel voxel;

		/// <summary>
		/// Location of this road
		/// </summary>
		public Location location { get { return voxel.location; } }



		/// <summary>
		/// Called by the <c>TrafficVoxel</c> to invalidate
		/// voxels.
		/// </summary>
		public virtual void invalidateVoxel() {
			// by default, the occupied voxel is updated
			World.world.onVoxelUpdated(voxel);
		}

		/// <summary>
		/// Called when a road is clicked.
		/// </summary>
		/// <returns>true if the click is processed and consumed</returns>
		public virtual bool onClick() { return false; }


		/// <summary>
		/// Returns true if a car is already on this road.
		/// </summary>
		public bool isOccupied { get { return voxel.car!=null; }}
		
		/// <summary>
		/// Attachs another direction to this existing road.
		/// 
		/// If necessary, this method will create a junction.
		/// The caller needs to make sure <code>canAttach(newDir)==true</code>
		/// </summary>
		public abstract bool attach( Direction newDir );

		/// <summary>
		/// Returns true if a new road with the given direction can be attached.
		/// </summary>
		public abstract bool canAttach( Direction newDir );

		/// <summary>
		/// Detaches two directions from this RR.
		/// This method should remove itself or even the parent TrafficVoxel
		/// if this change would remove the entire road completely.
		/// Thus the caller shouldn't assume that any reference to this
		/// object or its parent TrafficVoxel would be valid after the method invocation.
		/// </summary>
		public abstract void detach( Direction d1, Direction d2 );

		private RoadPattern _pattern;
		public RoadPattern pattern {
			get {
				return _pattern;
			}
		}

		/// <summary>
		/// Returns true if this road is running toward the given direction.
		/// </summary>
		public bool hasRoad( Direction d ) {
			return pattern.hasRoad(d);
		}

		/// <summary>
		/// Draws a road. This method is called before the car is drawn.
		/// </summary>
		public virtual void drawBefore( DrawContext display, Point pt ) {
		}
		/// <summary>
		/// Draws a road. This method is called after the car is drawn.
		/// </summary>
		public virtual void drawAfter( DrawContext display, Point pt ) {
		}

		public virtual TimeLength getStopTimeSpan( Bus bus, int callCount ) {
			return TimeLength.ZERO;
		}


		/// <summary>
		/// Gets the Road object of the specified location, if any.
		/// Otherwise null.
		/// </summary>
		public static Road get( Location loc ) {
			TrafficVoxel v = World.world[loc] as TrafficVoxel;
			if(v==null)		return null;
			return v.road;
		}
	}
}
