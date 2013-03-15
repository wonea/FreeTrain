using System;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.structs;

namespace freetrain.controllers.structs
{
	/// <summary>
	/// CommercialStructPlacementController の概要の説明です。
	/// </summary>
	public class CommercialStructPlacementController : FixedSizeStructController
	{
		#region Singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new CommercialStructPlacementController();
			theInstance.Show();
			theInstance.Activate();
		}

		private static CommercialStructPlacementController theInstance;

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			base.OnClosing(e);
			theInstance = null;
		}
		#endregion
		
		private CommercialStructPlacementController() : base(Core.plugins.commercialStructureGroup) {}

		/// <summary> LocationDisambiguator implementation </summary>
		public override bool isSelectable( Location loc ) {
			if(isPlacing) {
				// structures can be placed only on the ground
				return GroundDisambiguator.theInstance.isSelectable(loc);
			} else {
				return Commercial.get(loc)!=null;
			}
		}

		/// <summary>
		/// Removes the structure from given location, if any.
		/// </summary>
		public override void remove(MapViewWindow view, Location loc) {
			Commercial c = Commercial.get(loc);
			if(c!=null) {
				if(c.isOwned)
					c.remove();
				else
					MainWindow.showError("Can not remove");
					//! MainWindow.showError("撤去できません");
			}
		}

	}
}
