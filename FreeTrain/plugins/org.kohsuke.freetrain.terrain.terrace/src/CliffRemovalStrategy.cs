using System;
using System.Drawing;
using freetrain.framework;
using freetrain.controllers;
using freetrain.views;
using freetrain.views.map;

namespace freetrain.world.terrain.terrace
{
	/// <summary>
	/// Cliff removal mode
	/// </summary>
	internal class CliffRemovalStrategy : Strategy {
		public LocationDisambiguator disambiguator { get { return GroundDisambiguator.theInstance; } }

		public void onClick( MapViewWindow view, Location loc, Point ab ) {
			if( World.world[loc]!=null ) {
				MainWindow.showError("There are obstacles");
				//! MainWindow.showError("障害物があります");
				return;
			}
			// TODO: remove reclaimable voxels automatically
			
			int height = World.world.getGroundLevel(loc);
			
			byte[] h = new byte[4];
			int[] dx = new int[]{1,1,0,0};
			int[] dy = new int[]{-1,0,0,-1};
			for( int i=0; i<4; i++ )
				h[i] = (byte)( getHeight(new Location(loc.x+dx[i],loc.y+dy[i],loc.z)) - height*4 );

			if( h[0]==0 && h[1]==0 && h[2]==0 && h[3]==0 )
				return;

			new MountainVoxel( loc,
				h[0], h[1], h[2], h[3] );
		}

		/// <summary>
		/// Gets the fine-scale height of the south-west corner of the given voxel
		/// </summary>
		private int getHeight( Location loc ) {
			return max(
				getHeight( loc, Direction.SOUTHWEST ),
				getHeight( loc+Direction.SOUTH, Direction.NORTHWEST ),
				getHeight( loc+Direction.SOUTHWEST, Direction.NORTHEAST ),
				getHeight( loc+Direction.WEST, Direction.SOUTHEAST ) );
		}

		private int max( params int[] args ) {
			int r = args[0];
			for( int i=1; i<args.Length; i++ )
				r = Math.Max( r, args[i] );
			return r;
		}

		private int getHeight( Location loc, Direction d ) {
			MountainVoxel mv = MountainVoxel.get(loc);
			if(mv!=null) {
				return mv.getHeight(d) + loc.z*4;
			} else {
				return loc.z*4;
			}
		}

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx dc, Location loc, Point pt ) {
			if( World.world[loc]==null ) {
				ResourceUtil.emptyChip.drawShape( dc.surface, pt, Color.Red );
			}
		}
	}
}
