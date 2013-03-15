using System;
using System.Drawing;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.views;
using freetrain.views.map;
using freetrain.controllers;


namespace freetrain.world.structs.hv
{
	internal delegate void createCallback();
	/// <summary>
	/// ModalController that selects the half voxel region
	/// and do something with it.
	/// </summary>
	public class HVControllerImpl : ModalController, MapOverlay
	{

		/// <summary>Constant</summary>
		protected static readonly Location UNPLACED = world.Location.UNPLACED;
		static private readonly string cur_id = "{HALF-VOXEL-STRUCTURE-CURSOR-IMAGE}";
		static protected Sprite[] cursors = new Sprite[]{
			createCursorSprite(0,0), 
			createCursorSprite(32, 0), createCursorSprite(64, 0),
			createCursorSprite(96, 0), createCursorSprite(128, 0),
			createCursorSprite(32,16), createCursorSprite(64,16), 
			createCursorSprite(96,16), createCursorSprite(128,16)
		};
									
		static private Sprite createCursorSprite(int x, int y )
		{
			return new SimpleSprite(PictureManager.get(cur_id),new Point(0,0),new Point(x,y), new Size(32,16));
		}

		protected Location anchor = UNPLACED;
		protected Location currentPos = UNPLACED;
		protected Direction curSide;

		internal createCallback onCreated = null;

		protected readonly IControllerSite site;
		private readonly HalfVoxelContribution contrib;
		private readonly bool remover;
		
		public HVControllerImpl( HalfVoxelContribution _contrib, IControllerSite _site, bool _remove ) 
		{
			this.contrib = _contrib;
			this.site = _site;
			this.remover = _remove;

		}
		
		#region convenience methods
//		/// <summary>
//		/// North-west corner of the selected region.
//		/// </summary>
//		protected Location location1 
//		{
//			get 
//			{
//				Debug.Assert( anchor!=UNPLACED );
//				return new Location(
//					Math.Min( currentPos.x, anchor.x ),
//					Math.Min( currentPos.y, anchor.y ),
//					anchor.z );
//			}
//		}
//
//		/// <summary>
//		/// South-east corner of the selected region.
//		/// </summary>
//		protected Location location2 
//		{
//			get 
//			{
//				Debug.Assert( anchor!=UNPLACED );
//				return new Location(
//					Math.Max( currentPos.x, anchor.x ),
//					Math.Max( currentPos.y, anchor.y ),
//					anchor.z );
//			}
//		}
		
		protected PlaceSide currentSide 
		{
			get{ 
				HalfDividedVoxel v = World.world[anchor] as HalfDividedVoxel;
				if( v!=null)
				{
					ContributionReference[] refs = v.getReferences();
					if( remover )
					{
						// On remover mode, if the voxel has only one side occupied, select it.
						if( refs.Length==1 )
							return refs[0].placeSide;
						// Otherwise, select side in order to the cursor (in following code).
					}
					else
					{
						// On builder mode, there should be only one side remains empty.
						return (PlaceSide)(1-(int)refs[0].placeSide);
					}
				}
				
				// Select side according to the mouse cursor position.
				PlaceSide side = PlaceSide.Fore;
				if(front == Direction.NORTH || front == Direction.SOUTH)
				{
					if( curSide == Direction.NORTHEAST || curSide == Direction.SOUTHEAST )
						side = PlaceSide.Back;
				}
				else
				{				
					if( curSide == Direction.NORTHEAST || curSide == Direction.NORTHWEST )
						side = PlaceSide.Back;
				}
				return side; 
			}
		}

		private Direction front
		{
			get 
			{
				// Check if the front direction can be determined.
				if( currentPos==UNPLACED || currentPos.Equals(anchor) )
					throw new Exception("invalid call");
				
				HalfDividedVoxel v = World.world[anchor] as HalfDividedVoxel;
				// There is no restriction for empty voxel.
				if( v==null )
					return anchor.getDirectionTo(currentPos);
				
				//The X/Y alignment should be parallel to another side.
				ContributionReference[] refs = v.getReferences();
				if(refs[0].frontface.isParallelToY)
				{
					int dy = currentPos.y - anchor.y;
					if( dy<0 )
						return Direction.NORTH;
					else
						return Direction.SOUTH;
				}
				else
				{
					int dx = currentPos.x - anchor.x;
					if( dx<0 )
						return Direction.WEST;
					else
						return Direction.EAST;
				}			
			}
		}

		#endregion

		#region internal logic

		/// <summary>
		/// Called when the selection is completed.
		/// </summary>
		protected void onVoxelSelected( Location loc, Direction front, PlaceSide side )
		{
			if(remover)
			{
				contrib.destroy(loc,front,side);
			}
			else
			{
				contrib.create(loc,front,side);
				onCreated();
			}
		}

		/// <summary>
		/// Called when the selection is changed.
		/// </summary>
		protected void onVoxelUpdated( Location loc, Direction front, PlaceSide side ) 
		{
		}

		/// <summary>
		/// Called when the user wants to cancel the modal controller.
		/// </summary>
		protected void onCanceled() 
		{
			site.close();
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

		/// <summary>
		/// Get direction from anchor to which mouse coursor pointed.
		/// Used as calcuration of front face direction.
		/// </summary>
		/// <param name="ab">Cursor position</param>
		/// <returns></returns>
		private Direction getSide( Point ab )
		{
			Point p = World.world.fromXYZToAB(currentPos);
			int x = ab.X-p.X-16;
			int y = (ab.Y-p.Y-8)*2;
			
			Direction d;
			if( y<0 )
			{
				if( x>0 && x>(-y) )
					d = Direction.SOUTHEAST;
				else if(x<0 && (-x)>(-y))
					d = Direction.NORTHWEST;
				else
					d = Direction.NORTHEAST;
			}
			else 
			{
				if( x>0 && x>y )
					d = Direction.SOUTHEAST;
				else if(x<0 && (-x)>y)
					d = Direction.NORTHWEST;
				else
					d = Direction.SOUTHWEST;
			}
			return d;
		}

		//		private void modeChanged( object sender, EventArgs e ) 
		//		{
		//			anchor = UNPLACED;
		//		}

		//		private bool inBetween( Location loc, Location lhs, Location rhs ) 
		//		{
		//			if( !loc.inBetween(lhs,rhs) )	return false;
		//
		//			if(( lhs.x==rhs.x && rhs.x==loc.x )
		//				|| ( lhs.y==rhs.y && rhs.y==loc.y ) )	return true;
		//
		//			if( Math.Abs(loc.x-lhs.x)==Math.Abs(loc.y-lhs.y) )	return true;
		//
		//			return false;
		//		}

		#endregion
		
		#region public methods	

		/// <summary> LocationDisambiguator implementation </summary>
		public bool isSelectable( Location loc ) 
		{
			if(anchor!=UNPLACED)
				return loc.z==anchor.z;
			else
				// lands can be placed only on the ground
				return GroundDisambiguator.theInstance.isSelectable(loc);
		}

		public void close() 
		{
			onCanceled();
		}

		public string name { get { return site.name; } }

		// can be overrided by a derived class to return another object.
		public MapOverlay overlay 
		{
			get 
			{
				// return this object if it implements MapOverlay by itself.
				return this as MapOverlay;
			}
		}

		public LocationDisambiguator disambiguator 
		{
			get 
			{
				// the 2nd selection must go to the same height as the anchor.
				if(anchor==UNPLACED)	return GroundDisambiguator.theInstance;
				else					return sameLevelDisambiguator;
			}
		}
		private LocationDisambiguator sameLevelDisambiguator;

		public virtual void onAttached() 
		{
		}
		public virtual void onDetached() 
		{
			// clear the remaining image
			if( currentPos!=Location.UNPLACED )
				World.world.onVoxelUpdated(currentPos);
		}


		#endregion

		#region mouse handlers

		public void onMouseMove( MapViewWindow view, Location loc, Point ab ) 
		{
			if( anchor!=UNPLACED ) 
			{
				currentPos = align(loc);
				curSide = getSide(ab);
				//if( !currentPos.Equals(anchor) )
					//onVoxelUpdated(anchor,front,currentSide);
				
				World.world.onVoxelUpdated(anchor);
			}
		}

		public void onClick( MapViewWindow source, Location loc, Point ab ) 
		{
			
			if(anchor==UNPLACED) 
			{
				if( remover )
				{
					if(null == World.world[loc] as HalfDividedVoxel)
						return;
				}
				else
				{
					if(!HalfVoxelContribution.canBeBuilt(loc))
						return;
				}
				anchor = loc;
				currentPos = loc;
				curSide = getSide(ab);
				sameLevelDisambiguator = new SameLevelDisambiguator(anchor.z);
			} 
			else 
			{
				if( !currentPos.Equals(anchor) )
					onVoxelSelected(anchor,front,currentSide);
				World.world.onVoxelUpdated(anchor);
				anchor = UNPLACED;
			}
		}
		public void onRightClick( MapViewWindow source, Location loc, Point ab ) 
		{
			if( anchor==UNPLACED )
				close();	// cancel
			else 
			{
				World.world.onVoxelUpdated(anchor);
				anchor = UNPLACED;
			}
		}

		#endregion

		#region drawing methods

		public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) 
		{
			if( loc != anchor )	return;
			if( anchor.Equals(currentPos) )
				cursors[0].draw(canvas.surface,pt);
			else 
			{
				//HalfDividedVoxel v = World.world[loc] as HalfDividedVoxel;
				int n,m,l;
				n = remover?5:1;
				m = front.isParallelToX?0:1;
				l = (currentSide==PlaceSide.Back)?0:2;
				cursors[n+m+l].draw(canvas.surface,pt);
				if(!remover)
				{
					contrib.getSprite(front,currentSide,contrib.currentColor).drawAlpha( canvas.surface, pt );
					Sprite hls = contrib.getHighLightSprite(front,currentSide,contrib.currentHighlight);
					if(hls!=null) hls.drawAlpha( canvas.surface, pt );
				}
			}
			
		}

		public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}

		#endregion
	}

}
