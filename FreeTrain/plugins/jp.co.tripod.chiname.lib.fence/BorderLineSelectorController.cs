using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using freetrain.world;
using freetrain.views.map;

namespace freetrain.controllers
{
	/// <summary>
	/// ModalController that selects the rectangular region
	/// and do something with it.
	/// </summary>
	public abstract class BorderLineSelectorController : ModalController
	{
		/// <summary>Constant</summary>
		protected static readonly Location UNPLACED = world.Location.UNPLACED;

		protected Location anchor = UNPLACED;
		protected Location currentPos = UNPLACED;

		protected readonly IControllerSite site;
		
		public BorderLineSelectorController( IControllerSite _site ) {
			this.site = _site;
		}

	//
	// methods that can/should be overrided by derived classes
	//

		/// <summary>
		/// Called when the selection is completed.
		/// </summary>
		protected abstract void onLineSelected( Location loc1, Location loc2, Direction side );

		/// <summary>
		/// Called when the selection is changed.
		/// </summary>
		protected virtual void onLineUpdated( Location loc1, Location loc2, Direction side ) {}

		/// <summary>
		/// Called when the user wants to cancel the modal controller.
		/// </summary>
		protected virtual void onCanceled() {
			site.close();
		}

		public virtual string name { get { return site.name; } }

		// can be overrided by a derived class to return another object.
		public virtual MapOverlay overlay {
			get {
				// return this object if it implements MapOverlay by itself.
				return this as MapOverlay;
			}
		}


	//
	// convenience methods
		/// <summary>
		/// North-west corner of the selected region.
		/// </summary>
		protected Location location1 {
			get {
				Debug.Assert( anchor!=UNPLACED );
				return new Location(
					Math.Min( currentPos.x, anchor.x ),
					Math.Min( currentPos.y, anchor.y ),
					anchor.z );
			}
		}

		/// <summary>
		/// South-east corner of the selected region.
		/// </summary>
		protected Location location2 {
			get {
				Debug.Assert( anchor!=UNPLACED );
				return new Location(
					Math.Max( currentPos.x, anchor.x ),
					Math.Max( currentPos.y, anchor.y ),
					anchor.z );
			}
		}
		
		protected Direction curSide;
		protected Direction currentSide 
		{
			get{ return curSide; }
		}


	//
	// internal logic
	//



		/// <summary> LocationDisambiguator implementation </summary>
		public bool isSelectable( Location loc ) {
			if(anchor!=UNPLACED)
				return loc.z==anchor.z;
			else
				// lands can be placed only on the ground
				return GroundDisambiguator.theInstance.isSelectable(loc);
		}

		public virtual void close() {
			onCanceled();
		}



		/// <summary>
		/// Aligns the given location to the anchor so that
		/// the location will become straight.
		/// </summary>
		private Location align( Location loc ) 
		{
			loc.z = anchor.z;
			return loc.align4To(anchor);
		}


		public void onMouseMove( MapViewWindow view, Location loc, Point ab ) 
		{
			if( anchor!=UNPLACED ) 
			{
				if( currentPos!=UNPLACED )
					World.world.onVoxelUpdated(Cube.createInclusive(anchor,currentPos));
				currentPos = align(loc);
				curSide = getSide(currentPos,ab);
				onLineUpdated(anchor,currentPos,getSide(loc,ab));
				World.world.onVoxelUpdated(Cube.createInclusive(anchor,currentPos));
			}
		}

		public void onClick( MapViewWindow source, Location loc, Point ab ) 
		{
			
			if(anchor==UNPLACED) 
			{
				anchor = loc;
				currentPos = loc;
				curSide = getSide(loc,ab);
				sameLevelDisambiguator = new SameLevelDisambiguator(anchor.z);
			} 
			else 
			{
				loc = align(loc);
				onLineSelected(anchor,loc,curSide);
				//World.world.onVoxelUpdated(Cube.createInclusive(anchor,loc));
				anchor = UNPLACED;
			}
		}
		public void onRightClick( MapViewWindow source, Location loc, Point ab ) 
		{
			if( anchor==UNPLACED )
				close();	// cancel
			else 
			{
				World.world.onVoxelUpdated(Cube.createInclusive(anchor,currentPos));
				anchor = UNPLACED;
			}
		}

		public LocationDisambiguator disambiguator 
		{
			get 
			{
				// the 2nd selection must go to the same height as the anchor.
				if(anchor==UNPLACED)	return RailRoadDisambiguator.theInstance;
				else					return sameLevelDisambiguator;
			}
		}
		private LocationDisambiguator sameLevelDisambiguator;


//		private void modeChanged( object sender, EventArgs e ) 
//		{
//			anchor = UNPLACED;
//		}

		private Direction getSide( Location loc, Point ab )
		{
			Point p = World.world.fromXYZToAB(currentPos);
			int dx = Math.Abs(currentPos.x-anchor.x);
			int dy = Math.Abs(currentPos.y-anchor.y);
			int x = ab.X-p.X;
			int y = ab.Y-p.Y;
			Debug.WriteLine("diff=("+x+","+y+")");
			Direction d;
			if( dx+dy==0 )
			{
				if( x>15 )
				{
					if(y<8)
						d=Direction.EAST;
					else
						d=Direction.SOUTH;
				}
				else 
				{
					if(y<8)
						d=Direction.NORTH;
					else
						d=Direction.WEST;
				}
			}
			else
			{
				if(dx>dy) 
				{
					if(y<8)
						d=Direction.NORTH;
					else
						d=Direction.SOUTH;
				}
				else
				{
					if(x>15)
						d=Direction.EAST;
					else
						d=Direction.WEST;
				}
			}
			return d;
		}
		

		private bool inBetween( Location loc, Location lhs, Location rhs ) 
		{
			if( !loc.inBetween(lhs,rhs) )	return false;

			if(( lhs.x==rhs.x && rhs.x==loc.x )
				|| ( lhs.y==rhs.y && rhs.y==loc.y ) )	return true;

			if( Math.Abs(loc.x-lhs.x)==Math.Abs(loc.y-lhs.y) )	return true;

			return false;
		}

		public virtual void onAttached() 
		{
		}
		public virtual void onDetached() 
		{
			// clear the remaining image
			if( currentPos!=Location.UNPLACED )
				World.world.onVoxelUpdated(currentPos);
		}

	}
}

