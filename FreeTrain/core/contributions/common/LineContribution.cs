using System;
using System.Drawing;
using freetrain.world;
using freetrain.framework.plugin;

namespace freetrain.contributions.common
{
	/// <summary>
	/// Base class for SpecialRailContribution and RoadContritbuion
	/// </summary>
	[Serializable]
	public abstract class LineContribution : Contribution
	{
		protected LineContribution( string type, string id ) : base(type,id) {}

		/// <summary>
		/// Returns true if rails can be built between the two given locations
		/// </summary>
		public abstract bool canBeBuilt( Location loc1, Location loc2 );

		/// <summary>
		/// Builds rail roads between the two given locations.
		/// This method will be called only when canBeBuilt(loc1,loc2) returns true.
		/// </summary>
		public abstract void build( Location loc1, Location loc2 );

		/// <summary>
		/// Removes this special rail road between the given two locations.
		/// It is not an error for some other kinds of rail to appear in between
		/// these two.
		/// </summary>
		public abstract void remove( Location loc1, Location loc2 );

		/// <summary>
		/// Gets the name of this special rail.
		/// </summary>
		public abstract string name { get; }

		/// <summary>
		/// Gets a one line description of this rail.
		/// </summary>
		public abstract string oneLineDescription { get; }

		/// <summary>
		/// Gets the bitmap that will be used in the construction dialog.
		/// Should reload a fresh copy every time this method is called.
		/// The caller should dispose the object if it becomes unnecessary.
		/// </summary>
		public abstract Bitmap previewBitmap { get; }
	
		public enum DirectionMode {
			FourWay,
			EightWay
		};

		/// <summary>
		/// Available directions
		/// </summary>
		public virtual DirectionMode directionMode { get { return DirectionMode.FourWay; } }
	}
}
