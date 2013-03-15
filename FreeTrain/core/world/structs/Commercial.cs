using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using freetrain.contributions.structs;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.util;
using freetrain.world;
using freetrain.world.subsidiaries;

namespace freetrain.world.structs
{
	// TODO: value should be unified to use long, not int.

	/// <summary>
	/// Commercial structure.
	/// </summary>
	[Serializable]
	public class Commercial : PopulatedStructure, SubsidiaryEntity
	{
		private readonly new CommercialStructureContribution type;
		
		private readonly SubsidiaryCompany subsidiary;

		/// <summary>
		/// Creates a new commercial structurewith its left-top corner at
		/// the specified location.
		/// </summary>
		/// <param name="_type">
		/// Type of the structure to be built.
		/// </param>
		public Commercial( CommercialStructureContribution _type, WorldLocator wloc, bool initiallyOwned  )
			: base( _type, wloc ) {
			
			this.type = _type;
			if(wloc.world == World.world) {
				this.subsidiary = new SubsidiaryCompany(this,initiallyOwned);
			}
		}

		#region Entity implementation
		public override bool isSilentlyReclaimable { get { return false; } }
		public override bool isOwned { get { return subsidiary.isOwned; } }
		public override int entityValue { get { return (int)subsidiary.currentMarketPrice; } }
		#endregion

		public long structurePrice {
			get {
				return type.price;
			}
		}

		public long totalLandPrice {
			get {
				return World.world.landValue[ baseLocation + type.size/2 ]*type.size.x*type.size.y;
			}
		}

		public Location locationClue {
			get {
				return base.baseLocation + type.size/2;
			}
		}



		public new static bool canBeBuilt( Location loc, Distance sz, ControlMode cm ) {
			return Structure.canBeBuilt(loc,sz,cm) && Structure.isOnTheGround(loc,sz);
		}





		public override bool onClick() {
			return false;	// no associated action
			// TODO: do something if it's owned
		}
		public override string name { get { return type.name; } }

		internal protected override Color heightCutColor { get { return hcColor; } }
		private static Color hcColor = Color.FromArgb(179,115,51);

		/// <summary>
		/// Gets the station object if one is in the specified location.
		/// </summary>
		public static Commercial get( Location loc ) {
			return World.world.getEntityAt(loc) as Commercial;
		}

		public static Commercial get( int x, int y, int z ) { return get(new Location(x,y,z)); }
	}
}
