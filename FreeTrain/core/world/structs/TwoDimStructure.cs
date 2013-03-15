using System;
using System.Drawing;
using System.Runtime.Serialization;
using freetrain.contributions.common;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.util;
using org.kohsuke.directdraw;

namespace freetrain.world.structs
{
	/// <summary>
	/// Two-dimensional structure with the fixed height of 1.
	/// </summary>
	[Serializable]
	public abstract class TwoDimStructure : Structure
	{
		/// <summary>
		/// Builds a new structure.
		/// </summary>
		/// <param name="_type">type of this structure</param>
		/// <param name="loc">location of the left-top corner</param>
		public TwoDimStructure( FixedSizeStructureContribution _type, Location loc ) {
			this.type = _type;
			
			int Y = type.size.y;
			int X = type.size.x;

			voxels = new StructureVoxel[ Y, X ];
			for( int y=0; y<Y; y++ )
				for( int x=0; x<X; x++ ) {
					Location l = loc;
					l.x+=x; l.y+=y;
					voxels[y,x] = new SimpleStructureVoxel( this, (byte)x, (byte)y, l );
				}
			this.baseLocation = loc;
		}


		protected readonly FixedSizeStructureContribution type;

		protected internal Sprite[,] sprites { get { return type.sprites; } }


		/// <summary> Voxels that belong to this structure. </summary>
		protected readonly StructureVoxel[,] voxels;

		/// <summary>
		/// Top-left corner of this structure.
		/// </summary>
		public readonly Location baseLocation;

		/// <summary>
		/// Gets the distance to this location from the base location of this structure.
		/// </summary>
		protected int distanceTo( Location loc ) {
			return baseLocation.distanceTo(loc);
		}


		/// <summary>
		/// Remove this structure.
		/// 
		/// This implementation removes all voxels that belong to
		/// this object.
		/// Usually the derived class needs to provide extra operations.
		/// </summary>
		public override void remove() {
			World world = World.world;
			foreach( StructureVoxel v in voxels ) {
				Location loc = v.location;
				world.remove(loc);
				world.onVoxelUpdated(loc);
			}
		}

		public override int entityValue { get { return type.price; } }


		/// <summary>
		/// Obtains the color that will be used to draw when in the height-cut mode.
		/// </summary>
		internal protected abstract Color heightCutColor { get; }


		/// <summary>
		/// StructureVoxel with default drawing mechanism.
		/// </summary>
		[Serializable]
		protected internal class SimpleStructureVoxel : StructureVoxel {
			protected internal SimpleStructureVoxel( TwoDimStructure _owner, byte _x, byte _y, Location _loc )
				: base(_owner,_loc) {

				this.x=_x;
				this.y=_y;
			}
			
			/// <summary>The offset of the sprite.</summary>
			private readonly byte x,y;

			public override void draw( DrawContext display, Point pt, int heightCutDiff  ) {
				TwoDimStructure o = (TwoDimStructure)owner;

				if( heightCutDiff<0 || heightCutDiff>=o.type.height )
					o.sprites[x,y].draw(display.surface,pt);
				else
					ResourceUtil.emptyChip.drawShape(display.surface,pt, o.heightCutColor );
			}
		}
	}
}
