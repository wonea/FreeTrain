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
	/// Pseudo three-dimensional structure.
	/// 
	/// Sprites are only defined for the ground-level voxels,
	/// and other higher voxels will be occupied by invisible ones.
	/// </summary>
	[Serializable]
	public abstract class PThreeDimStructure : Structure
	{
		public PThreeDimStructure( FixedSizeStructureContribution type, WorldLocator wloc ) {
			this.baseLocation = wloc.location;
			this.type = type;
			
			// build voxels
			for( int z=0; z<type.size.z; z++ )
				for( int y=0; y<type.size.y; y++ )
					for( int x=0; x<type.size.x; x++ )
						CreateVoxel( new WorldLocator(wloc.world,baseLocation+new Distance(x,y,z)));
		}

		protected virtual StructureVoxel CreateVoxel( WorldLocator loc ){
			return new VoxelImpl( this, loc ); 
		}

		public readonly FixedSizeStructureContribution type;

		/// <summary>
		/// north-west bottom corner of this structure.
		/// </summary>
		public readonly Location baseLocation;

		public override int entityValue { get { return type.price; } }

		public Cube cube { get { return Cube.createExclusive(baseLocation,type.size); } }


		/// <summary>
		/// Obtains the color that will be used to draw when in the height-cut mode.
		/// </summary>
		internal protected abstract Color heightCutColor { get; }

		/// <summary>
		/// Gets the distance to this location from the base location of this structure.
		/// </summary>
		protected int distanceTo( Location loc ) {
			return baseLocation.distanceTo(loc);
		}


		public override string name { get { return type.name; } }


		[Serializable]
		protected class VoxelImpl : StructureVoxel, IDeserializationCallback {
			internal VoxelImpl( PThreeDimStructure _owner, WorldLocator wloc )
				: base(_owner,wloc) {
				setSprite();
			}
			//public override bool transparent { get { return true; } }

			private new PThreeDimStructure owner { get {
				return (PThreeDimStructure)base.owner;
			}}

			/// <summary>
			/// The sprite to draw, or null if the voxel
			/// is invisible.
			/// </summary>
			[NonSerialized]	// programatically recreatable.
			private Sprite sprite;

			public void OnDeserialization(object sender) {
				setSprite();
			}

			private void setSprite() {
				PThreeDimStructure o = owner;
				sprite = o.type.getSprite( location - o.baseLocation );
			}
			
			public override void draw( DrawContext display, Point pt, int heightCutDiff  ) {
				PThreeDimStructure o = owner;

				int zdiff = o.type.size.z - (this.location.z - o.baseLocation.z);

				if( heightCutDiff<0 || zdiff < heightCutDiff ) {
					// draw in a normal mode
					sprite.draw(display.surface,pt);
				} else {
					// drawing in the height cut mode
					if( this.location.z==o.baseLocation.z )
						ResourceUtil.emptyChip.drawShape(display.surface,pt, o.heightCutColor );
				}
			}
		}

		
		public override event EventHandler onEntityRemoved;

		public override void remove() {
			// just remove all the voxels
			World world = World.world;
			foreach( Voxel v in this.cube.voxels )
				world.remove(v);

			if( onEntityRemoved!=null )
				onEntityRemoved(this,null);
		}

		public static new bool canBeBuilt( Location loc, Distance size, ControlMode cm) {
			if(!Structure.canBeBuilt(loc,size, cm))
				return false;

			// make sure all the voxels are on the ground.
			for( int y=0; y<size.y; y++ )
				for( int x=0; x<size.x; x++ )
					if( World.world.getGroundLevel(loc.x+x,loc.y+y)!=loc.z )
						return false;
			return true;
		}
	}
}
