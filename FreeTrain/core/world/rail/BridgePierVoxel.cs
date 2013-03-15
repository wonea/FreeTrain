using System;
using System.Drawing;
using org.kohsuke.directdraw;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.util;

namespace freetrain.world.rail
{
	/// <summary>
	/// Voxel that is used to support raised railroad.
	/// 
	/// A derived class needs to provide the surface object.
	/// </summary>
	[Serializable]
	public abstract class BridgePierVoxel : AbstractVoxelImpl//, Entity
	{
		// TODO: not sure if this class should implement Entity
		public override bool transparent { get { return true; } }

//		public static void electBridgeSupport( Location loc ) {
//			electBridgeSupport( loc, typeof(DefaultImpl) );
//		}
//
//		public static void electBridgeSupport( Location loc, Type bridgeType ) {
//			electBridgeSupport( loc, bridgeType, bridgeType );
//		}

		public static void electBridgeSupport( Location loc, Entity owner ) {
			electBridgeSupport( loc, typeof(DefaultImpl), owner );
		}

		public static void electBridgeSupport( Location loc, Type bridgeType, Entity owner ) {
			electBridgeSupport( loc, bridgeType, bridgeType, owner );
		}

		public static void electBridgeSupport( Location loc, Type topBridgeType, Type otherBridgeType, Entity owner ) {
			// check if a support is buildable
			// TODO: start from the surface level
			for( int z=0; z<loc.z; z++ )
				if(World.world[loc.x,loc.y,z]!=null)
					return;
			
			// if we can, do it
			for( int z=World.world.getGroundLevel(loc); z<loc.z; z++ ) {
				Activator.CreateInstance(
					(z==loc.z-1)?topBridgeType:otherBridgeType,
					new object[]{ loc.x, loc.y, z, owner });
			}
		}

		/// <summary>
		/// Elects a bridge support from the surface level to the given location,
		/// if it can be done.
		/// </summary>
		/// <param name="loc">The location of the elevated RR.</param>
//		public static void electBridgeSupport( Location loc, Type topBridgeType, Type otherBridgeType ) {
//			
//			// check if a support is buildable
//			// TODO: start from the surface level
//			for( int z=0; z<loc.z; z++ )
//				if(World.world[loc.x,loc.y,z]!=null)
//					return;
//			
//			// if we can, do it
//			for( int z=World.world.getGroundLevel(loc); z<loc.z; z++ ) {
//				Activator.CreateInstance(
//					(z==loc.z-1)?topBridgeType:otherBridgeType,
//					new object[]{ loc.x, loc.y, z });
//			}
//		}

		/// <summary>
		/// Tears down a bridge support if any.
		/// </summary>
		public static void teardownBridgeSupport( Location loc, Entity owner ) {
			for( int z=0; z<loc.z; z++ ) {
				BridgePierVoxel v = World.world[loc.x,loc.y,z] as BridgePierVoxel;
				if(v != null)
					World.world.remove(loc.x,loc.y,z);
			}
		}
		
//		protected BridgePierVoxel( int x, int y, int z ) : this(x,y,z,null) {
//		}

		protected BridgePierVoxel( int x, int y, int z, Entity owner ) : base(x,y,z) {
			this.owner = owner;
		}
		
		protected Entity owner;
		public override Entity entity {	get { return owner; }}
		#region Entity implementation
		/*
		public bool isSilentlyReclaimable { get { return false; } }
		public bool isOwned { get { return true; } }

		public void remove() {
			Location loc = this.location;
			World.world.remove(this);
			if(onEntityRemoved!=null)	onEntityRemoved(this,null);
		}

		// TODO: value?
		public int entityValue { get { return 0; } }

		public event EventHandler onEntityRemoved;
		*/
		#endregion



		protected abstract Sprite sprite { get; }

		public override void draw( DrawContext display, Point pt, int heightCutDiff ) {
			// draw the pier in alpha if in the height cut mode
			if( heightCutDiff==0 )		sprite.drawAlpha( display.surface, pt );
			else						sprite.draw     ( display.surface, pt );
		}



		private static readonly Picture theImage = ResourceUtil.loadSystemPicture("BridgePier.bmp");
		public static readonly Sprite defaultSprite;
		public static readonly Sprite slopeNESprite;
		static BridgePierVoxel() {
			defaultSprite = new SimpleSprite(theImage, new Point(0,16), new Point( 0,0), new Size(32,32) );
			slopeNESprite = new SimpleSprite(theImage, new Point(0,16), new Point(32,0), new Size(32,32) );
		}




		[Serializable]
		public class DefaultImpl : BridgePierVoxel {
			public DefaultImpl( int x, int y, int z, Entity owner ) : base(x,y,z, owner) {}
			protected override Sprite sprite { get { return defaultSprite; } }
		}

		[Serializable]
		public class SlopeNEImpl : BridgePierVoxel {
			public SlopeNEImpl( int x, int y, int z, Entity owner ) : base(x,y,z, owner) {}
			protected override Sprite sprite { get { return slopeNESprite; } }
		}
	}
}
