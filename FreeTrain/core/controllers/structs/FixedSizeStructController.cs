using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.contributions.common;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.structs;

namespace freetrain.controllers.structs
{
	/// <summary>
	/// FixedSizeStructController の概要の説明です。
	/// </summary>
	public abstract class FixedSizeStructController : StructPlacementController
	{
		protected FixedSizeStructController( StructureGroupGroup groupGroup ) : base(groupGroup) {}

		public abstract void remove(MapViewWindow view, Location loc);
		// TODO: extend StructureContribution and Structure so that 
		// the this method can be implemented here.

		protected new FixedSizeStructureContribution selectedType {
			get {
				return (FixedSizeStructureContribution)base.selectedType;
			}
		}

		public override void onClick(MapViewWindow view, Location loc, Point ab ) {
			if( isPlacing ) {
				if(!selectedType.canBeBuilt(loc,ControlMode.player)) {
					MainWindow.showError("Can not build");
					//! MainWindow.showError("設置できません");
				} else {
					CompletionHandler handler = new CompletionHandler(selectedType,loc,true);
					new ConstructionSite( loc, new EventHandler(handler.handle), selectedType.size );
				}
			} else {
				remove(view,loc);
			}
		}

		[Serializable]
		private class CompletionHandler {
			internal CompletionHandler( FixedSizeStructureContribution contribution, Location loc, bool owned ) {
				this.contribution = contribution;
				this.loc = loc;
				this.owned = owned;
			}
			private readonly FixedSizeStructureContribution contribution;
			private readonly Location loc;
			private readonly bool owned;
			public void handle( object sender, EventArgs args ) {
				Structure s = contribution.create(loc,owned);
			}
		}


		protected override AlphaBlendSpriteSet createAlphaSprites() {
			return new AlphaBlendSpriteSet( selectedType.sprites );
		}
	}
}
