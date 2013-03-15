using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using org.kohsuke.directdraw;
using freetrain.util;
using freetrain.world;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.controllers;

namespace freetrain.views
{
	/// <summary>
	/// Draw quarter view of the map and maintain them properly.
	/// </summary>
	public class QuarterViewDrawer : IDisposable, VoxelOutlookListener
	{//
		/// <summary>
		/// True to allow MapOverlay to update the surface.
		/// </summary>
		private bool _enableOverlay;

		private DirectDraw directDraw;

		/// <summary>
		/// Off-screen buffer that keeps the image of this window.
		/// </summary>
		private Surface offscreenBuffer;
		
		/// <summary>
		/// Drawing context that wraps <code>offscreenBuffer</code>
		/// </summary>
		private DrawContextEx drawContext;

		/// <summary>
		/// Maintains the dirty rect that needs to be updated.
		/// The coordinate is the (A,B) coordinates.
		/// </summary>
		private readonly DirtyRect dirtyRect = new DirtyRect();

		/// <summary>
		/// The position of the top-left pixel in (A,B) axis.
		/// </summary>
		private Point topLeft;

		/// <summary>
		/// Height-cut height. Voxels above this height
		/// will not be drawn.
		/// </summary>
		private int _heightCutHeight;


		/// <summary>
		/// Fired when the height-cut height is changed
		/// </summary>
		public event EventHandler OnHeightCutChanged;

		/// <summary>
		/// Fired when a surface is updated.
		/// </summary>
		public event EventHandler OnUpdated;

		private World world;


		/// <param name="initialView">
		///		the region that this object draws in the A,B axis.
		/// </param>
		public QuarterViewDrawer( World _world, DirectDraw directDraw, Rectangle initialView ) 
		{
			this.world = _world;
			_heightCutHeight = world.size.z-1;
			this.directDraw = directDraw;
			recreateDrawBuffer( initialView.Size, true );
			topLeft = new Point( initialView.X, initialView.Y );

			world.voxelOutlookListeners.Add(this);
			
			onUpdateAllVoxels();	// initially all the rects are dirty
			PictureManager.onSurfaceLost += new EventHandler(onSurfaceLost);
		}

		/// <summary>
		/// Size of the view in pixels.
		/// </summary>
		public Size size 
		{
			get 
			{
				if( offscreenBuffer!=null )		return offscreenBuffer.size;
				else							return new Size(0,0);
			}
			set 
			{
				recreateDrawBuffer(value,false);
			}
		}

		public Point origin 
		{
			get 
			{
				return topLeft;
			}
			set 
			{
				if( topLeft==value )	return;

				Rectangle shared = Rectangle.Intersect(
					new Rectangle( topLeft, size ),
					new Rectangle( value,   size ) );
				
				if( shared.Width*shared.Height*2 < size.Height*size.Width ) 
				{
					// not much area is shared. just update all the voxels
					topLeft = value;
					onUpdateAllVoxels();
					return;
				}

				// copy the reusable rect
				offscreenBuffer.resetClipRect();
				offscreenBuffer.blt(
					new Point( shared.X-value.X,   shared.Y-value.Y ),
					offscreenBuffer,
					new Point( shared.X-topLeft.X, shared.Y-topLeft.Y ),
					shared.Size );

				topLeft = value;

				// adjust Y
				if( value.Y < shared.Y ) 
				{	// scroll up
					dirtyRect.add( shared.X, value.Y,               shared.Width, size.Height-shared.Height );
				} 
				else 
				{ // scroll down
					dirtyRect.add( shared.X, value.Y+shared.Height, shared.Width, size.Height-shared.Height );
				}
				updateScreen();

				// adjust X
				if( value.X < shared.X ) 
				{	// scroll left
					dirtyRect.add( value.X,              value.Y, size.Width-shared.Width, size.Height );
				} 
				else 
				{ // scroll right
					dirtyRect.add( value.X+shared.Width, value.Y, size.Width-shared.Width, size.Height );
				}
				updateScreen();
			}
		}

		public bool enableOverlay 
		{
			get 
			{
				return _enableOverlay;
			}
			set 
			{
				if( _enableOverlay==value )	return;
				_enableOverlay = value;
				onUpdateAllVoxels();
			}
		}

		/// <summary>
		/// Obtain the visible rectangle in (A,B) coordinates.
		/// </summary>
		private Rectangle visibleRect 
		{
			get 
			{
				return new Rectangle( topLeft, size );
			}
		}

		/// <summary>
		/// Height-cut height. Voxels above this height
		/// will not be drawn.
		/// 
		/// Note that setting <code>world.size.z-1</code> will cause
		/// all the voxels to be drawn.
		/// </summary>
		public int heightCutHeight 
		{
			get 
			{
				return _heightCutHeight;
			}
			set 
			{
				if( _heightCutHeight!=value ) 
				{
					_heightCutHeight = value;

					if(OnHeightCutChanged!=null)
						OnHeightCutChanged(this,null);
					onUpdateAllVoxels();
				} 
				else
					_heightCutHeight = value;
			}
		}

		/// <summary>
		/// Recreates the offscreen drawing surface.
		/// </summary>
		/// <param name="forceRecreate">
		/// Set this flag to true to force the release of the surface.
		/// This is useful when you absolutely wants a fresh surface
		/// (such as when the current surface is lost)
		/// </param>
		private void recreateDrawBuffer( Size size, bool forceRecreate ) 
		{
			if(offscreenBuffer!=null ) 
			{
				if( size==offscreenBuffer.size && !forceRecreate )
					return;	// no need for re-allocation
				drawContext.Dispose();
				drawContext = null;
				offscreenBuffer.Dispose();
				offscreenBuffer = null;
			}

			if(size.Width>0 && size.Height>0) 
			{
				offscreenBuffer = directDraw.createOffscreenSurface( size );
				drawContext = new DrawContextEx(offscreenBuffer);
			}

			onUpdateAllVoxels();
		}



		/// <summary>
		/// Return true if the given voxel is visible.
		/// </summary>
		public bool isVisible( Location loc ) 
		{
			// find the bounding box in (A,B) axes
			return World.world.getBoundingBox(loc).IntersectsWith(this.visibleRect);
		}

		public void Dispose() 
		{
			if(world != null)
				world.voxelOutlookListeners.Remove(this);
			world = null;			
			PictureManager.onSurfaceLost -= new EventHandler(onSurfaceLost);
			if(offscreenBuffer!=null) 
			{
				offscreenBuffer.Dispose();
				offscreenBuffer = null;
			}
		}


		public void onUpdateVoxel( Location loc ) 
		{
			Rectangle boundingBox = world.getBoundingBox(loc);
			if( boundingBox.IntersectsWith(this.visibleRect) ) 
			{
				dirtyRect.add(boundingBox);
				if(OnUpdated!=null)		OnUpdated(this,null);
			}
		}

		public void onUpdateVoxel( Cube cube ) 
		{
			Rectangle r = cube.boundingABRect;
			r.Intersect(this.visibleRect);	// cut the rect by the visible rect
			if( !r.IsEmpty ) 
			{
				dirtyRect.add(r);
				if(OnUpdated!=null)		OnUpdated(this,null);
			}
		}

		/// <summary>
		/// Invalidate the entire visible region.
		/// </summary>
		public void onUpdateAllVoxels() 
		{
			dirtyRect.add(this.visibleRect);
			if(OnUpdated!=null)			OnUpdated(this,null);
		}




		
		/// <summary>
		/// Checks if we need to draw a ground surface.
		/// </summary>
		private bool shouldDrawGround( int h, int v, int z ) 
		{
			HoleVoxel hva;
			if(z==world.size.z-1)	hva = null;
			else		hva = world.voxelHVD(h,v,z) as HoleVoxel;
			
			HoleVoxel hvb;
			if(z==0)	hvb = null;
			else		hvb = world.voxelHVD(h,v,z-1) as HoleVoxel;

			if( hva!=null && !hva.drawGround(false) )
				return false;
			if( hvb!=null && !hvb.drawGround(true) )
				return false;

			return true;
		}

		/// <summary>
		/// Redraw the specified region.
		/// Should be used only from the draw() method.
		/// </summary>
		/// <param name="updateRect">Rectangle in the (A,B) coordinates.</param>
		private void draw( Rectangle rectAB, MapOverlay overlay ) 
		{
			// the same rectangle in the client coordinates
			Rectangle rectClient = fromABToClient(rectAB);

			int waterLevel = world.waterLevel;
			bool noHeightCut = (heightCutHeight==world.size.z-1);

			Color waterSurfaceColor = waterSurfaceDayColor;
			if( world.viewOptions.useNightView )
				waterSurfaceColor = ColorMap.getNightColor(waterSurfaceColor);


			offscreenBuffer.clipRect = rectClient;	// set clipping

			Rectangle rectHV = fromABToHV(rectAB);	// determine the region to draw

			int Hmax = Math.Min( rectHV.Right, world.size.x-1 );

			int Zinit = noHeightCut ? (int)waterLevel : 0;	// no need to draw underwater unless in the height cut mode
			int Z = heightCutHeight;
			int Vmax = Math.Min( rectHV.Bottom + Z*2, world.size.y-1 );
			Sprite emptyChip = ResourceUtil.getGroundChip(world);
			Sprite waterChip = ResourceUtil.underWaterChip;
			for( int v=Math.Max(0,rectHV.Top); v<=Vmax; v++ ) 
			{
				for( int h=rectHV.Left; h<=Hmax; h++ ) 
				{

					int groundLevel = world.getGroundLevelFromHV(h,v);

					int zi = Zinit;
					if( Zinit<=groundLevel && !shouldDrawGround(h,v,Zinit) )
						zi = Math.Max(zi-1,0);	// if the surface is being cut, start from one smaller
					
					for( int z=zi; z<=Z; z++ ) 
					{
						Voxel voxel = world.voxelHVD(h,v,z);
						//						if(voxel!=null)
						//							Debug.Assert( voxel.location==world.toXYZ(h,v,z) );

						// point in the client coordinate to draw
						Point pt = fromHVZToClient(h,v,z);

						// draw the surface anyway.
						if( voxel == null || voxel.transparent )
						{
							if(z==groundLevel) 
							{
								if( shouldDrawGround(h,v,z)) 
								{
									if( waterLevel<=z ) 									
										emptyChip.draw( drawContext.surface, pt );									
									else
										waterChip.draw( drawContext.surface, pt );
								}
							} 
							else
								if(z==waterLevel && noHeightCut) 
							{								
								emptyChip.drawShape( drawContext.surface, pt, waterSurfaceColor );
							} 
							else
								if(z==Z && Z<groundLevel) 
							{
								// if the surface voxel is not drawn, draw the "under group" chip
								if( shouldDrawGround(h,v,z) )
									ResourceUtil.underGroundChip.draw( drawContext.surface, pt );
							}
						}
//					}
//				}
//			}
//
//			for( int v=Math.Max(0,rectHV.Top); v<=Vmax; v++ ) 
//			{
//				for( int h=rectHV.Left; h<=Hmax; h++ ) 
//				{
//					int groundLevel = world.getGroundLevelFromHV(h,v);
//
//					int zi = Zinit;
//					if( Zinit<=groundLevel && !shouldDrawGround(h,v,Zinit) )
//						zi = Math.Max(zi-1,0);	// if the surface is being cut, start from one smaller				
//					for( int z=zi; z<=Z; z++  )
//					{
//						Voxel voxel = world.voxelHVD(h,v,z);
//						//						if(voxel!=null)	Debug.Assert( voxel.location==world.toXYZ(h,v,z) );
//
//						if( z<groundLevel && z<heightCutHeight ) continue;
//						Point pt = fromHVZToClient(h,v,z);

						if(voxel!=null)
							voxel.drawVoxel( drawContext, pt, noHeightCut?-1:(Z-z+1) );
						if(overlay!=null)
							overlay.drawVoxel( this, drawContext, world.toXYZ(h,v,z),  pt );
					}
				}
			}
			
			if( Core.options.drawBoundingBox ) 
			{
				rectClient.Inflate(-1,-1);
				offscreenBuffer.drawBox(rectClient);
			}
		}

		/// <summary>
		/// Update the surface by redrawing necessary parts.
		/// </summary>
		private void updateScreen() 
		{
			if( dirtyRect.isEmpty || offscreenBuffer==null)
				return;	// no need for draw.

			DateTime start = DateTime.Now;

			MapOverlay overlay = null;
			ModalController controller = MainWindow.mainWindow.currentController;
			if(controller!=null)
				overlay = controller.overlay;

			if(overlay!=null)
				overlay.drawBefore( this, drawContext );

			// draw the rect
			Rectangle dr = dirtyRect.rect;
			if( dr.Top<0 )	dr.Y=0;	// clipping. higher voxel on the northen edge could make top<0
			draw( dr, overlay );
			dirtyRect.clear();

			// allow MapOverlay to do the wrap-up
			if(overlay!=null)
				overlay.drawAfter( this, drawContext );

			if( Core.options.drawStationNames ) 
			{
				// REVISIT: I don't want these code inside this method.
				//  it needs to be extensible.
				Graphics graphics = drawContext.graphics;
				
				foreach( freetrain.world.rail.Station st in world.stations ) 
				{
					Point pt = fromXYZToClient( st.baseLocation );
					pt.Y -= 16;	// display the string above the station

					SizeF sz = graphics.MeasureString( st.name, drawFont );
					pt.X -= (int)sz.Width/2;

					graphics.DrawString( st.name, drawFont, drawBrush1, pt.X+1, pt.Y+1 );
					graphics.DrawString( st.name, drawFont, drawBrush2, pt.X  , pt.Y   );
				}
			}

			drawContext.tag = null;		// reset the attached tag

			Debug.WriteLine( "update took "+(DateTime.Now - start).TotalMilliseconds+"ms" );
		}

		/// <summary>
		/// Draw the view to the specified point of the given surface.
		/// </summary>
		public void draw( Surface target, Point pt ) 
		{
			try 
			{
				updateScreen();
			} 
			catch( COMException e ) 
			{
				if( DirectDraw.isSurfaceLostException(e) ) 
				{
					PictureManager.onSurfaceLost(this,null);
					updateScreen();	// and retry
				} 
				else
					throw e;	// unable to handle this exception
			}

			// just send the offscreen buffer to the primary surface
			// drawContext will be null if the client size is empty or the window is minimized.
			// no blitting necessary in that case.
			if( drawContext!=null )
				target.blt( pt, drawContext.surface );
		}

		/// <summary>
		/// Event handler of the onSurfaceLost event. Reallocate the back buffer
		/// and force redraw.
		/// </summary>
		private void onSurfaceLost( object sender, EventArgs ea ) 
		{
			// reallocate the buffer
			recreateDrawBuffer( size, true );
		}

		/// <summary>
		/// Obtains the image as a bitmap.
		/// </summary>
		public Bitmap createBitmap() 
		{
			updateScreen();
			return drawContext.surface.createBitmap();
		}

		//		/// <summary>
		//		/// Moves the view to display the specified location in the center
		//		/// </summary>
		//		public void moveTo( Location loc ) {
		//			Point pt = world.fromXYZToAB(loc);
		//			Size sz = this.size;
		//			sz.Width /= 2;
		//			sz.Height /= 2;
		//			pt -= sz;
		//
		//			this.origin = pt;
		//
		//			if(OnUpdated!=null)		OnUpdated(this,null);
		//		}


		private static Color waterSurfaceDayColor = Color.FromArgb(0,114,188);

		private static Font drawFont = new Font("MS PGothic", 10);
		//! private static Font drawFont = new Font("ＭＳ Ｐゴシック", 10);
		private static SolidBrush drawBrush1 = new SolidBrush(Color.Black);   
		private static SolidBrush drawBrush2 = new SolidBrush(Color.White);   
		
		
		#region coordinates conversion methods
		/// <summary>
		/// Convert the client coordinates to the (A,B) coordinates.
		/// </summary>
		public Point fromClientToAB( Point pt ) 
		{
			return new Point(
				pt.X + topLeft.X,
				pt.Y + topLeft.Y );
		}

		public Point fromABToClient( Point pt ) 
		{
			return new Point(
				pt.X - topLeft.X,
				pt.Y - topLeft.Y );
		}

		public Rectangle fromABToClient( Rectangle r ) 
		{
			return new Rectangle( fromABToClient(r.X,r.Y), r.Size );
		}

		public Point fromClientToAB( int x, int y ) 
		{
			return fromClientToAB(new Point(x,y));
		}

		public Point fromABToClient( int a, int b ) 
		{
			return fromABToClient(new Point(a,b));
		}

		/// <summary>
		/// Converts the (A,B) coordinates to (X,Y,Z) coordinates.
		/// </summary>
		public Location fromABToXYZ( int a, int b, ModalController controller ) 
		{
			int t = 2*b-16;
			
			int x = (a-t)>>5;
			int y = (a+t)>>5;

			x += (world.size.y-1)/2;

			// (x,y,0) is the base location. disambiguate the location.
			// TODO: use height-cut here to force the specified z-level
			if(controller!=null) 
			{
				LocationDisambiguator disambiguator = controller.disambiguator;
				for( int z=heightCutHeight; z>=0; z-- ) 
				{
					Location loc = new Location(x-z,y+z,z);
					if( disambiguator.isSelectable(loc) )
						return loc;
				}
			}

			return new Location( x, y, 0 );
		}

		public Location fromABToXYZ( Point pt, ModalController controller ) 
		{
			return fromABToXYZ( pt.X, pt.Y, controller );
		}

		/// <summary>
		/// Converts the mouse coordinate (which is client coordinate)
		/// to (X,Y) coordinates.
		/// </summary>
		public Location fromClientToXYZ( MouseEventArgs mea, ModalController controller ) 
		{
			return fromABToXYZ(fromClientToAB(mea.X,mea.Y),controller);
		}
		public Location fromClientToXYZ( int cx, int cy, ModalController controller ) 
		{
			return fromABToXYZ(fromClientToAB(cx,cy),controller);
		}

		public Point fromXYZToClient( int x, int y, int z ) 
		{
			return fromABToClient( world.fromXYZToAB(x,y,z) );
		}

		public Point fromXYZToClient( Location loc ) 
		{
			return fromABToClient( world.fromXYZToAB(loc) );
		}

		/// <summary>
		/// Obtain the bounding rectangle in the (H,V) coordinates
		/// that completely covers the given rect of the (A,B) coordinates.
		/// All the corners of the result is inclusive.
		/// </summary>
		private Rectangle fromABToHV( Rectangle r ) 
		{
			int h1 = (r.Left-16)/32;
			int v1 = r.Top   / 8-1;
			int h2 = r.Right /32;
			int v2 = r.Bottom/ 8;

			return new Rectangle( h1, v1, h2-h1, v2-v1 );
		}

		/// <summary>
		/// Converts the (H,V,Z) coordinates to the client coordinates.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public Point fromHVZToClient( int h, int v, int z ) 
		{
			v -= z*2;
			return fromABToClient( 16*( 2*h + (v&1) ), 8*v );
		}
		#endregion

	}
}
