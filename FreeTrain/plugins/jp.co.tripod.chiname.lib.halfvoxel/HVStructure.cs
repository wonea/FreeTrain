using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using freetrain.contributions.common;
using freetrain.contributions.structs;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.util;
using freetrain.world;
using freetrain.world.subsidiaries;

namespace freetrain.world.structs.hv
{
	/// <summary>
	/// The half divided voxel structure.
	/// Consist of Back part and Fore part.
	/// </summary>
	[Serializable]
	public class HVStructure : Structure, SubsidiaryEntity
	{
		enum Orientation : int {XAxis,YAxis};
		/// <summary>
		/// The sprite to draw.
		/// </summary>
		public HVStructure( ContributionReference type, Location loc  ) 
		{
			this.baseLocation = loc;
			if(type.placeSide == PlaceSide.Back)
				this.back = type;
			else
				this.fore = type;
			
			// build voxels
			new HalfDividedVoxel( this, loc );
			subsidiary = new SubsidiaryCompany(this,false);
			if( type.population!=null )
				stationListener = new StationListenerImpl( type.population, loc );
		}

		#region add or remove half voxel.
		public bool add( ContributionReference type ) 
		{
			if(type.placeSide==PlaceSide.Back)
			{				
				if( back!=null ) return false; // already occupied!

				if( type.frontface.isParallelToX )
				{
					if( fore.frontface.isParallelToX )
					{
						back = type;
						return true;
					}
				}
				else // parallel to Y
				{
					if( fore.frontface.isParallelToY )
					{
						back = type;
						return true;
					}
				}
			}
			else // PlaceSide.Fore
			{
				if( fore!=null ) return false; // already occupied!

				if( type.frontface.isParallelToX )
				{
					if( back.frontface.isParallelToX )
					{
						fore = type;
						return true;
					}
				}
				else // parallel to Y
				{
					if( back.frontface.isParallelToY )
					{
						fore = type;
						return true;
					}
				}
			}
			World.world.onVoxelUpdated(baseLocation);
			return false;
		}

		public bool remove( PlaceSide side ) 
		{
			if(side==PlaceSide.Back)
			{
				if( back==null ) return false;
				back=null;
				if(fore==null)
					remove();
			}
			else // PlaceSide.Fore
			{
				if( fore==null ) return false;
				fore=null;
				if(back==null)
					remove();
			}
			World.world.onVoxelUpdated(baseLocation);
			return true;
		}

		protected ContributionReference fore;
		protected ContributionReference back;

		internal ContributionReference foreside { get{ return fore; } }
		internal ContributionReference backside { get{ return back; } }
		#endregion

		#region population related methods
		/// <summary>
		/// Station to which this structure sends population to.
		/// </summary>
		private readonly StationListenerImpl stationListener;

		public override object queryInterface( Type aspect ) 
		{
			// if type.population is null, we don't have any population
			if( aspect==typeof(rail.StationListener) )
				return stationListener;
			else
				return base.queryInterface(aspect);
		}
		#endregion

		/// <summary>
		/// north-west bottom corner of this structure.
		/// </summary>
		public readonly Location baseLocation;

		/// <summary>
		/// Obtains the color that will be used to draw when in the height-cut mode.
		/// </summary>
		internal protected Color heightCutColor { get{ return hcColor;} }
		private static Color hcColor = Color.FromArgb(146,94,42);
		/// <summary>
		/// Gets the distance to this location from the base location of this structure.
		/// </summary>
		protected int distanceTo( Location loc ) 
		{
			return baseLocation.distanceTo(loc);
		}

		public override bool onClick() 
		{
			return false;	// no associated action
		}
		
		public override event EventHandler onEntityRemoved;

		public override void remove() 
		{
			// just remove the voxels
			World world = World.world;			
			world.remove(baseLocation);

			if( onEntityRemoved!=null )
				onEntityRemoved(this,null);

			if( stationListener!=null )
				stationListener.onRemoved();
		}

//		public static new bool canBeBuilt( Location loc, Distance size ) 
//		{
//			if(!Structure.canBeBuilt(loc,size))
//				return false;
//
//			// make sure all the voxels are on the ground.
//			for( int y=0; y<size.y; y++ )
//				for( int x=0; x<size.x; x++ )
//					if( World.world.getGroundLevel(loc.x+x,loc.y+y)!=loc.z )
//						return false;
//			return true;
//		}

		#region Entity implementation
		public override bool isSilentlyReclaimable { get { return false; } }
		public override bool isOwned { get { return false; } }
		public override int entityValue { get { return (int)subsidiary.currentMarketPrice; } }
		#endregion

		#region SubsideryEntity implementation
		protected readonly SubsidiaryCompany subsidiary;
		public override string name{ 
			get{
				if(fore==null) return back.name;
				if(back==null) return fore.name;
				if(fore.name.Equals(back.name))
					return fore.name;
				return fore.name+"/"+back.name; 
			}
		}
		public long structurePrice 
		{
			get{
				long p = 0;
				if(fore!=null) p+=fore.price;
				if(back!=null) p+=back.price;
				return p;
			}
		}

		public long totalLandPrice 
		{	get{ return World.world.landValue[ baseLocation ]; }}

		public Location locationClue 
		{	get { return baseLocation;	}}
		#endregion

	}



	[Serializable]
	public  class HalfDividedVoxel : AbstractVoxelImpl
	{
		public HalfDividedVoxel( HVStructure _owner, Location _loc )
			: base(_loc) 
		{
			this.owner = _owner;
		}

		internal protected HalfDividedVoxel( HVStructure _owner, WorldLocator wloc): base(wloc){
			this.owner = _owner;
		}

		/// <summary>
		/// The structure object to which this voxel belongs.
		/// </summary>
		public readonly HVStructure owner;

		public override Entity entity { get { return owner; } }

		/// <summary>
		/// onClick event is delegated to the parent.
		/// </summary>
		public override bool onClick() 
		{
			return owner.onClick();
		}
		
		public override void draw( DrawContext display, Point pt, int heightCutDiff  ) 
		{			
			if( heightCutDiff >= 0)
				ResourceUtil.emptyChip.drawShape(display.surface,pt, owner.heightCutColor );
			else
				ResourceUtil.emptyChip.draw(display.surface,pt);
				// above line is needed when my(=477) patch is applied.

			if(owner.backside!=null)
				if( heightCutDiff<0 || owner.backside.height < heightCutDiff ) 
				{
					owner.backside.getSprite().draw(display.surface,pt);
					Sprite hls = owner.backside.getHighlightSprite();
					if( hls!= null ) hls.draw(display.surface,pt);
				}
			if(owner.foreside!=null)
				if( heightCutDiff<0 || owner.foreside.height < heightCutDiff )
				{
					owner.foreside.getSprite().draw(display.surface,pt);
					Sprite hls = owner.foreside.getHighlightSprite();
					if( hls!= null ) hls.draw(display.surface,pt);
				}
		}

		public bool hasSpace
		{
			get{ return (owner.backside==null || owner.foreside==null); }
		}

		public ContributionReference[] getReferences()
		{
			ContributionReference[] arr;
			if(hasSpace)
			{
				arr = new ContributionReference[1];
				if( owner.backside==null )
					arr[0] = owner.foreside;
				else
					arr[0] = owner.backside;
			}
			else
			{
				arr = new ContributionReference[2];
				arr[0] = owner.backside;
				arr[1] = owner.foreside;
			}
			return arr;
		}
	}
}
