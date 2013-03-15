using System;
using System.Diagnostics;
using System.Drawing;
using org.kohsuke.directdraw;
using freetrain.framework.graphics;

namespace freetrain.world
{
	/// <summary>
	/// A block in the game world.
	/// 
	/// The voxel is the unit of the game world. The game world consists of a cube of
	/// voxels, and this is the base class of such voxels.
	/// </summary>
	[Serializable]
	public abstract class Voxel
	{
		public abstract Location location { get; }
		protected bool showFence = true;
		public virtual bool transparent { get { return false; } }

		/// <summary>
		/// Draws this voxel
		/// </summary>
		/// <param name="heightCutDiff">
		/// heightCut - Z.
		///	0 if this voxel is located to the "edge" of the height cut.
		///	negative value if the view is not in the height cut mode.
		///	positive values if this voxel is located below the cut height
		///	(the value will be the difference between the height of
		///	this voxel and the cut height.)
		/// </param>
		public abstract void draw( DrawContext display, Point pt, int heightCutDiff );

		public void drawVoxel( DrawContext display, Point pt, int heightCutDiff ) 
		{
			if( showFence )
			{
				// draw behind fence 
				drawBehindFence(display, pt);
				draw( display, pt, heightCutDiff );
				drawFrontFence(display, pt);
				//draw front fence 
			}
			else
				draw( display, pt, heightCutDiff );
		}

		public abstract void setFence( Direction d, Fence f );

		public abstract Fence getFence( Direction d );

		protected abstract void drawFrontFence(DrawContext display, Point pt);

		protected abstract void drawBehindFence(DrawContext display, Point pt);

		/// <summary>
		/// Processes a mouse click.
		/// </summary>
		/// <returns>true if a mouse click event is "consumed"</returns>
		public virtual bool onClick() { return false; }

		/// <summary>
		/// Query this voxel to return some "aspect" of it.
		/// 
		/// Aspect is usually a tear-off interface that allows
		/// voxels to be extended through compositions.
		/// 
		/// The queryInterface method of voxels should delegate to
		/// the queryInterface method of entities.
		/// </summary>
		/// <returns>null if the given aspect is not supported.</returns>
		public virtual object queryInterface( Type aspect ) { return null; }

		/// <summary>
		/// Calls immediately after the voxel is removed from the world.
		/// </summary>
		public virtual void onRemoved() {}
		// TODO: is this method necessary


		/// <summary>
		/// Short-cut to call the getLandPrice method of the World class.
		/// </summary>
		public int landPrice {
			get {
				return (int)World.world.landValue[location];
			}
		}

		/// <summary>
		/// Obtains a reference to the entity that includes this voxel.
		/// </summary>
		public abstract Entity entity { get; }

	}

	/// <summary>
	/// Partial implementation for most of the voxel.
	/// </summary>
	[Serializable]
	public abstract class AbstractVoxelImpl : Voxel
	{
		protected Fence[] fence = new Fence[4];

		protected AbstractVoxelImpl( int x, int y, int z ) : this(new Location(x,y,z)) 
		{
		}

		protected AbstractVoxelImpl( Location _loc){
			this.loc=_loc;
			World.world[loc] = this;
		}

		protected AbstractVoxelImpl(WorldLocator wloc) {
			this.loc=wloc.location;
			wloc.world[loc] = this;
		}

		private readonly Location loc;

		public override Location location { get { return loc; } }

		protected override void drawFrontFence(DrawContext display, Point pt) 
		{
			Fence f;
			f =fence[(Direction.SOUTH).index/2];
			if(f!=null)
				f.drawFence( display.surface,pt,Direction.SOUTH );
			f =fence[(Direction.WEST).index/2];
			if(f!=null)
				f.drawFence( display.surface,pt,Direction.WEST );
		}

		protected override void drawBehindFence(DrawContext display, Point pt) 
		{
			Fence f;
			f =fence[(Direction.NORTH).index/2];
			if(f!=null)
				f.drawFence( display.surface,pt,Direction.NORTH );
			f =fence[(Direction.EAST).index/2];
			if(f!=null)
				f.drawFence( display.surface,pt,Direction.EAST );
		}

		public override void setFence( Direction d, Fence f )
		{
			fence[d.index/2] = f;
		}

		public override Fence getFence( Direction d )
		{
			return fence[d.index/2];
		
		}
	
	}


	/// <summary>
	/// Voxel can additionally implement this interface to
	/// control the painting of the ground surface.
	/// 
	/// The drawing routine queries this interface for voxels
	/// that are directly above and below the surface.
	/// </summary>
	public interface HoleVoxel
	{
		/// <summary>
		/// Returns false to prevent the ground surface to be drawn.
		/// </summary>
		/// <param name="above">
		/// True if the callee is located directly above the ground,
		/// false if directly below the ground.
		/// </param>
		bool drawGround( bool above );
	}

	/// <summary>
	/// The interface called when the fence should be drawn.
	/// </summary>
	public interface Fence
	{
		/// <summary>
		/// called when the fehce should be drawn.
		/// </summary>
		/// <param name="d">one of the 4 directions (N,E,W,S)</param>
		void drawFence( Surface surface, Point pt, Direction d );

		string fence_id { get; }
	}
}
