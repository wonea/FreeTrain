using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.framework;
using freetrain.framework.graphics;

namespace freetrain.world.terrain
{
	/// <summary>
	/// Slopes used for mountains and under-waters.
	/// </summary>
	[Serializable]
	public class MountainVoxel : AbstractVoxelImpl, Entity
	{
		private Sprite[] patterns;
		private Sprite ground;
		private byte[] indices;
		private int treePrice;
		public override bool transparent { get { return heightData==0; } }

		public MountainVoxel( Location loc, byte hNE, byte hSE, byte hSW, byte hNW ) : base(loc) 
		{
			owned = false;
			Debug.Assert( 0<=hNE && hNE<=4 );
			Debug.Assert( 0<=hSE && hSE<=4 );
			Debug.Assert( 0<=hNW && hNW<=4 );
			Debug.Assert( 0<=hSW && hSW<=4 );

			heightData = (Int16)((hNE<<12)|(hSE<<8)|(hSW<<4)|hNW);
		}


		/// <summary> Height at the four corners. Encoded into 16bits by using 4bits for each. </summary>
		private Int16 heightData;

		/// <summary> Obtains the height at the specified corner (0-4). </summary>
		public int getHeight( Direction d ) 
		{
			Debug.Assert( !d.isSharp );
			return getHeight(d.index/2);
		}
		private int getHeight( int idx ) 
		{
			switch(idx) 
			{
				case 0:		return (heightData>>12);
				case 1:		return (heightData>> 8)&15;
				case 2:		return (heightData>> 4)&15;
				case 3:		return (heightData    )&15;
				default:	throw new ArgumentOutOfRangeException();
			}
		}
		public void setHeight( Direction d, int h ) 
		{
			Debug.Assert( !d.isSharp );
			Debug.Assert( 0<=h && h<=4 );
			switch(d.index/2) 
			{
				case 0:		heightData = (Int16)((heightData&0x0FFF)|(h<<12)); break;
				case 1:		heightData = (Int16)((heightData&0xF0FF)|(h<< 8)); break;
				case 2:		heightData = (Int16)((heightData&0xFF0F)|(h<< 4)); break;
				case 3:		heightData = (Int16)((heightData&0xFFF0)|(h    )); break;
				default:	throw new ArgumentOutOfRangeException();
			}
			World.world.onVoxelUpdated(location); // update this voxel
		}


		/// <summary>
		/// If the voxel is completely raised to the point that the ground-level
		/// needs to be raised, return true.
		/// </summary>
		public bool isSaturated 
		{
			get 
			{
				//				for( int i=0; i<4; i++ )
				//					if(getHeight(i)!=4)
				//						return false;
				//				return true;
				return heightData==0x4444;
			}
		}
		/// <summary>
		/// If the voxel is completely flattened to the point that this
		/// voxel is no longer necessary.
		/// </summary>
		public bool isFlattened 
		{
			get 
			{
				//				for( int i=0; i<4; i++ )
				//					if(getHeight(i)!=0)
				//						return false;
				//				return true;
				return heightData==0x0000;
			}
		}

		/// <summary>
		/// Gets 4*height + mountain height.
		/// 
		/// This method computes the height of the given location in fine scale.
		/// It returns -1 if the given location is out of the world.
		/// </summary>
		public static int getTotalHeight( Location loc, Direction d ) 
		{
			World w = World.world;
			
			if( w.isOutsideWorld(loc) )		return -1;

			int z = w.getGroundLevel(loc);
			MountainVoxel mv = MountainVoxel.get( new Location( loc.x, loc.y, z ) );
			if(mv==null)	return z*4;
			else			return z*4 + mv.getHeight(d);
		}

		/// <summary>
		/// Return true if the given corner of this mountain voxel is
		/// matched with three other surrounding voxels.
		/// </summary>
		public static bool isCornerMatched( Location loc, Direction d ) 
		{
			int h1 = getTotalHeight( loc, d );
			int h2 = getTotalHeight( loc+d, d.opposite );
			int h3 = getTotalHeight( loc+d.left, d.right90 );
			int h4 = getTotalHeight( loc+d.right, d.left90 );

			int r = h1;
			if( r==-1 )				r = h2;
			if( h2!=-1 && h2!=r )	return false;

			if( r==-1 )				r = h3;
			if( h3!=-1 && h3!=r )	return false;

			if( r==-1 )				r = h4;
			if( h4!=-1 && h4!=r )	return false;

			return true;
		}


		public override Entity entity { get { return this; } }
		
		#region Entity implementation
		public bool isSilentlyReclaimable { get { return false; } }
		protected bool owned;
		public bool isOwned { 
			get { return owned; }
			set { owned = value; }
		}

		public void remove() 
		{
			// TODO: not sure what to do.
			// you can't just remove one mountain voxel without affecting the terrain
		}

		public event EventHandler onEntityRemoved;

		public int entityValue { get { return 0 + treePrice; } }

		#endregion



		private Color mapColor( Color c ) 
		{
			if( World.world.viewOptions.useNightView ) 
				return ColorMap.getNightColor(c);
			else	
				return c;
		}

		private Color[] currentMountainColors 
		{
			get 
			{
				return mountainColors[(int)World.world.clock.season];
			}
		}

		public override void draw( DrawContext display, Point pt, int heightCutDiff ) 
		{
			heightCutDiff--;
			drawGround(display,pt,heightCutDiff);
			if( patterns != null && !Core.options.hideTrees)
				drawTrees(display,pt,heightCutDiff);
		}

		private void drawGround(DrawContext display, Point pt, int heightCutDiff ) 
		{
			Point basePt = pt;
			World world = World.world;

			if( heightCutDiff==0 ) 
			{
				ResourceUtil.emptyChip.drawShape( display.surface, pt, 
					isUnderWater ? waterColors[3] : currentMountainColors[3] );
				return;
			}

			if( isFlattened ) 
			{
				if( ground != null )
					ground.draw( display.surface, pt );
				return;
			}

			pt.Y -= getHeight(0)*4;	// apply offset

			// compute target colors
			Color[] dstColors = new Color[]{
											   selectColor(),
											   mapColor(isUnderWater?Color.Navy:currentMountainColors[0]) };

			int tdiff = (getHeight(0)-getHeight(2)+4);
			int umax = Math.Min(tdiff+2,6);

			Size sz = new Size(16,tdiff*4+2*4+1);
			
			// draw left half
			int ldiff = (getHeight(0)-getHeight(3)+2);
			bool vflip;

			if( ldiff < tdiff-ldiff ) 
			{
				vflip = true;
				ldiff = tdiff-ldiff;
			} 
			else
				vflip = false;
			int lidx = (umax-ldiff);

			display.surface.bltColorTransform( new Point(pt.X,pt.Y-(vflip?2*4:0)),
				images[tdiff], new Point(lidx*32,0), sz,
				srcColors, dstColors, vflip );

		{
			// left cliff
			Location neighbor = location+Direction.WEST;
			if(!(world[neighbor] is MountainVoxel)
				&& getHeight(2)+getHeight(3)>0
				&& world.getGroundLevel(neighbor) <= world.getGroundLevel(location) )
				cliff[0,getHeight(3),getHeight(2)].draw( display.surface, basePt );
		}


			// right half
			int rdiff = (getHeight(0)-getHeight(1)+2);
			if( rdiff < tdiff-rdiff ) 
			{
				vflip = true;
				rdiff = tdiff-rdiff;
			} 
			else 
				vflip = false;
			int ridx = (umax-rdiff);

			pt.X +=16;
			display.surface.bltColorTransform( new Point(pt.X,pt.Y-(vflip?2*4:0)),
				images[tdiff], new Point(ridx*32+16,0), sz,
				srcColors, dstColors, vflip );

		{
			basePt.X+=16;
			// right cliff
			Location neighbor = location+Direction.SOUTH;
			if(!(world[neighbor] is MountainVoxel)
				&& getHeight(2)+getHeight(1)>0
				&& world.getGroundLevel(neighbor) <= world.getGroundLevel(location) )
				cliff[1,getHeight(2),getHeight(1)].draw( display.surface, basePt );
		}
		}

		private void drawTrees(DrawContext display, Point pt, int heightCutDiff)
		{
			int h = getHeight(Direction.NORTHEAST)-getHeight(Direction.SOUTHWEST);
			//int w = getHeight(Direction.NORTHWEST)-getHeight(Direction.SOUTHEAST);
			if( h > 0)
			{
				for( int i=0; i<indices.Length; i+=3 )
					patterns[ indices[i+2] ].draw( display.surface,
						new Point( pt.X+indices[i+0], pt.Y+indices[i+1]*(h+4)/4-8 ) );
			}
			else
			{
				for( int i=0; i<indices.Length; i+=3 )
					patterns[ indices[i+2] ].draw( display.surface,
						new Point( pt.X+indices[i+0], pt.Y+indices[i+1]*(h+4)/4+2 ) );
			}
		}
		private static readonly Color[] srcColors = new Color[]{ Color.White, Color.Black };




		/// <summary>
		/// brushes to paint mountains
		/// </summary>
		private static Color[][] mountainColors = new Color[4][];

		/// <summary>
		/// brushes to paint under-waters
		/// </summary>
		private static Color[] waterColors;


		/// <summary>
		/// Loads color palette from XML.
		/// </summary>
		private static Color[] loadColors( XmlElement p ) 
		{
			XmlNodeList lst = p.SelectNodes("color");
			Color[] r = new Color[lst.Count];
			int idx=0;
			foreach( XmlElement e in lst ) 
			{
				string type = e.Attributes["type"].Value;
				string text = e.InnerText;

				string[] cs = text.Split(',');
				int x = int.Parse(cs[0]);
				int y = int.Parse(cs[1]);
				int z = int.Parse(cs[2]);

				if(type=="hsv")	r[idx++] = fromHSV(x,y,z);
				else			r[idx++] = Color.FromArgb(x,y,z);
			}
			return r;
		}


		/// <summary>
		/// Lighting vector.
		/// </summary>
		private const int nX=80, nY=-20, nZ=-80;
		// it should be big so that integer ops in the selectBrush operation 
		// would run with a small rounding error.

		private bool isUnderWater { get { return location.z < World.world.waterLevel; } }

		/// <summary>
		/// Returns the brush to draw this voxel.
		/// </summary>
		private Color selectColor() 
		{
			Color[] colors = isUnderWater?waterColors:currentMountainColors;

			int dV = getHeight(2) - getHeight(0);
			int dH = getHeight(1) - getHeight(3);

			int idx = (int)( colors.Length * Math.Abs( nX*(dH-dV) + nY*(dH+dV) + nZ*-32 ) /
				Math.Sqrt( (dH*dH + dV*dV + 32*16)*2*(nX*nX + nY*nY + nZ*nZ) ) );
			
			if(idx>=colors.Length)	idx = colors.Length-1;

			return mapColor(colors[idx]);
		}


		/// <summary> Parses (H,S,V) into a color object. </summary>
		/// <param name="H">[0,240)</param>
		/// <param name="S">[0,240)</param>
		/// <param name="V">[0,240)</param>
		private static Color fromHSV( int H, int S, int V ) 
		{ 
			// TODO: optimization
			float h = ((float)H)/40.0f;
			float s = ((float)S)/240.0f;
			float v = ((float)V)/240.0f;

			int i = H/40;
			float f = h - i;  
			if ( (i&1)==0 ) f = 1 - f; // if i is even  
			
			int m = (int)( 256 * v * (1 - s));  
			int n = (int)( 256 * v * (1 - s * f));  
			int o = (int)( 256 * v );

			switch (i) 
			{  
				case 6:  
				case 0: return Color.FromArgb(255, o, n, m);  
				case 1: return Color.FromArgb(255, n, o, m);  
				case 2: return Color.FromArgb(255, m, o, n);
				case 3: return Color.FromArgb(255, m, n, o);  
				case 4: return Color.FromArgb(255, n, m, o);  
				case 5: return Color.FromArgb(255, o, m, n);  
			}

			Debug.Assert(false);
			return Color.Empty;
		} 


		public static MountainVoxel get( Location loc ) 
		{
			return World.world[loc] as MountainVoxel;
		}
		
		public void setTrees(Sprite ground, Sprite[] trees, byte[] indices, int price) 
		{
			this.ground = ground;
			patterns = trees;
			this.indices = indices;
			treePrice = price;
		}

		public void removeTrees()
		{
			ground = null;
			patterns = null;
			indices = null;
			treePrice = 0;
			owned = false;
		}

		#region cliff
		private static Sprite[,,] cliff = new Sprite[2/*0:S,1:W*/,5/*lHeight*/,5/*rHeight*/];
		/// <summary> Load sprites for cliffs. </summary>
		private static void initCliffSprites() 
		{
			Picture picture = ResourceUtil.loadSystemPicture("Cliff.bmp");
			for( int side=0; side<2; side++ ) 
			{
				for( int l=0; l<=4; l++ ) 
				{
					for( int r=0; r<=4; r++ ) 
					{
						cliff[side,l,r] = new SimpleSprite( picture,
							new Point(0,8),
							new Point( side*16+r*32, l*24 ),
							new Size(16,24));
					}
				}
			}
		}
		#endregion

		#region drawing
		private static Surface[] images = new Surface[9];
		static MountainVoxel() 
		{
			initCliffSprites();

			// pre-draw pictures

			Pen blackPen = new Pen(Color.Black);
			Brush whiteBrush = new SolidBrush(Color.White);
			Brush magentaBrush = new SolidBrush(Color.Magenta);

			for( int tdiff=0; tdiff<=8; tdiff++ ) 
			{// tdiff= difference between top and bottom
				int u = Math.Min(tdiff+2,6);

				images[tdiff] = ResourceUtil.directDraw.createOffscreenSurface(
					new Size(32*(5-(Math.Abs(tdiff-4)+1)/2), tdiff*4+1 +2*4) );
				images[tdiff].sourceColorKey = Color.Magenta;

				using( GDIGraphics g = new GDIGraphics(images[tdiff]) ) 
				{
					// clearing the surface at the beginning will leave dots since
					// adjacent pictures sometimes overlap.
					//					g.graphics.FillRectangle(magentaBrush,
					//						new Rectangle( new Point(0,0), images[tdiff].size )); // clear the surface


					int idx=0;
					
					for( ; u>=tdiff-u; u--,idx++ ) 
					{
						int offset = idx*32;

						// clear the sprite
						g.graphics.FillRectangle(magentaBrush,
							new Rectangle( new Point(offset,0), new Size(32, images[tdiff].size.Height) ));

						Point[] pts = new Point[]{
													 new Point(offset+16,   0/*1*/),
													 new Point(offset+32, u    *4),
													 new Point(offset+16, tdiff*4),
													 new Point(offset+15, tdiff*4),
													 new Point(offset   , u    *4),
													 new Point(offset+15,   0/*1*/)};
						g.graphics.FillPolygon( whiteBrush, pts );
						g.graphics.DrawLines( blackPen, pts );
					}
				}
			}

			blackPen.Dispose();
			whiteBrush.Dispose();
			magentaBrush.Dispose();

			// load color palettes
			XmlDocument doc = new XmlDocument();
			doc.Load(ResourceUtil.findSystemResource("mountainPalette.xml"));
			mountainColors[0] = loadColors( (XmlElement)doc.SelectSingleNode("/*/spring") );
			mountainColors[1] = loadColors( (XmlElement)doc.SelectSingleNode("/*/summer") );
			mountainColors[2] = loadColors( (XmlElement)doc.SelectSingleNode("/*/autumn") );
			mountainColors[3] = loadColors( (XmlElement)doc.SelectSingleNode("/*/winter") );
			waterColors		  = loadColors( (XmlElement)doc.SelectSingleNode("/*/water") );
		}
		#endregion
	}
}
