using System;
using System.Windows.Forms;
using freetrain.controllers.structs;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.rail;

namespace freetrain.controllers.rail
{
	/// <summary>
	/// StationaryStructPlacementController の概要の説明です。
	/// </summary>
	public class StationaryStructPlacementController : FixedSizeStructController
	{
		#region Singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new StationaryStructPlacementController();
			theInstance.Show();
			theInstance.Activate();
		}

		private static StationaryStructPlacementController theInstance;

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			base.OnClosing(e);
			theInstance = null;
		}
		#endregion
		
		private StationaryStructPlacementController() : base(Core.plugins.railStationaryGroup) {}

		/// <summary> LocationDisambiguator implementation </summary>
		public override bool isSelectable( Location loc ) {
			if(isPlacing) {
				// structures can be placed only on the ground
				return GroundDisambiguator.theInstance.isSelectable(loc);
			} else {
				return RailStationaryStructure.get(loc)!=null;
			}
		}

		public override void remove(MapViewWindow view, Location loc) {
			RailStationaryStructure s = RailStationaryStructure.get(loc);
			if(s!=null) {
				s.remove();
			}
		}
	}
}
