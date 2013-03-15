using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;

namespace freetrain.world
{	
	/// <summary>
	/// A car can be in three states.
	/// 
	/// (1) inside a map
	///		direction!=null, and location has a valid value
	///	(2) not placed
	///		direction==null, location==UNPLACED
	///	(3) outside map
	/// </summary>
	[Serializable]
	public abstract class Car
	{
		private CarState _state = CarState.Unplaced.theInstance;

		/// <summary>
		/// Set the new state.
		/// </summary>
		/// <returns>the previous state</returns>
		protected CarState setState( CarState s ) {
			CarState oldState = _state;

			CarState.Inside ss = _state.asInside();
			if( ss!=null ) {
				Debug.Assert(ss.voxel.car==this);
				ss.voxel.car = null;
				World.world.onVoxelUpdated(ss.voxel);
			}
			
			_state = s;
			
			ss = s.asInside();
			if( ss!=null ) {
				Debug.Assert(ss.voxel.car==null);
				ss.voxel.car = this;
				World.world.onVoxelUpdated(ss.voxel);
			}

			return oldState;
		}

		/// <summary>
		/// Current location/direction of the car.
		/// </summary>
		public CarState state { get { return _state; } }

		/// <summary>
		/// 車両を配置する
		/// </summary>
		public void place( Location loc, Direction dir ) {
			Debug.Assert(state.isUnplaced);
			setState( new CarState.Inside(loc,dir) );
		}

		/// <summary>
		/// 車両を現在位置から撤去する
		/// </summary>
		public void remove() {
			Debug.Assert(!state.isUnplaced);
			setState(CarState.Unplaced.theInstance);
		}

		/// <summary>
		/// Called when a car is clicked.
		/// </summary>
		/// <returns>true if the click is processed and consumed</returns>
		public virtual bool onClick() { return false; }

		/// <summary>
		/// Draws the car into the specified location.
		/// </summary>
		public abstract void draw( DrawContext display, Point pt );

		/// <summary>
		/// Gets a car that occupies the specified place, if any. Or null otherwise.
		/// </summary>
		public static Car get( Location loc ) {
			TrafficVoxel v = TrafficVoxel.get(loc);
			if(v==null)		return null;
			else			return v.car;
		}

	}
}
