using System;
using System.Drawing;
using freetrain.framework;
using freetrain.framework.graphics;

namespace freetrain.world.terrain.terrace
{
	[Serializable]
	public class TerraceVoxel : AbstractVoxelImpl, Entity
	{
		/// <summary>
		/// Heights of the mountain that occupied this voxel
		/// before this terrace is created.
		/// </summary>
		private readonly byte[] heights;


		/// <param name="v">MountainVoxel to be replaced</param>
		public static TerraceVoxel create( MountainVoxel v ) {
			// back up heights
			byte[] heights = new byte[4];
			for( int i=0; i<4; i++ )
				heights[i] = (byte)v.getHeight(Direction.get(i*2+1));

			World.world.remove(v);

			// raise the ground level by one
			World.world.raiseGround( v.location );

			return new TerraceVoxel(v.location,heights);
		}

		private TerraceVoxel( Location loc, byte[] _heights ) : base(loc) {
			this.heights = _heights;
		}

		public override Entity entity { get { return this; } }

		public bool isSilentlyReclaimable { get { return false; } }
		public bool isOwned { get { return false; } }
		public int entityValue { get { return 0; } }

		/// <summary>
		/// Destruct this terrace and restore the mountain as it was.
		/// </summary>
		public void remove() {
			World.world.remove(this);
			World.world.lowerGround(this.location);
			if(onEntityRemoved!=null)	onEntityRemoved(this,null);
			new MountainVoxel( this.location, heights[0], heights[1], heights[2], heights[3] );
		}

		public event EventHandler onEntityRemoved;

		public override void draw( DrawContext display, Point pt, int heightCutDiff ) {
			if( heightCutDiff!=0 )
				image.draw( display.surface, pt );
		}

		public static readonly Sprite image;
		static TerraceVoxel() {
			Picture pic = new Picture(
				"{2A333E1B-7AA0-4A96-A054-DAEEEB84F953}",
				new Uri(TerraceContributionImpl.theInstance.baseUri, "terrace.bmp" ).LocalPath);
			image = new SimpleSprite( pic, new Point(0,7), new Point(0,1), new Size(32,23) );
		}
	}
}
