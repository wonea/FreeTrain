using System;
using freetrain.controllers;
using freetrain.controllers.structs;
using freetrain.contributions.common;
using freetrain.framework;
using freetrain.views.map;

namespace freetrain.world.soccerstadium
{
	/// <summary>
	/// PlacementController の概要の説明です。
	/// </summary>
	public class PlacementController : FixedSizeStructController
	{
		#region Singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new PlacementController();
			theInstance.Show();
			theInstance.Activate();
		}

		private static PlacementController theInstance;

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			base.OnClosing(e);
			theInstance = null;
		}
		#endregion
		
		private PlacementController() : base(StructureContributionImpl.groupGroup) {}
		


		/// <summary> LocationDisambiguator implementation </summary>
		public override bool isSelectable( Location loc ) {
			if(isPlacing) {
				// structures can be placed only on the ground
				return GroundDisambiguator.theInstance.isSelectable(loc);
			} else {
				return World.world.getEntityAt(loc) is StadiumStructure;
			}
		}

		/// <summary>
		/// Removes the structure from given location, if any.
		/// </summary>
		public override void remove(MapViewWindow view, Location loc) {
			StadiumStructure c = World.world.getEntityAt(loc) as StadiumStructure;
			if(c!=null)
				c.remove();
		}
	}
}
