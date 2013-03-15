using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.accounting;
using freetrain.world.land;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.util;
using org.kohsuke.directdraw;

namespace freetrain.controllers.land
{
	/// <summary>
	/// Controller that allows the user buy/sell land properties.
	/// </summary>
	public class LandPropertyController : ControllerHostForm
	{
		#region Singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new LandPropertyController();
			theInstance.Show();
			theInstance.Activate();
		}


		private static LandPropertyController theInstance;

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			theInstance = null;
		}
		#endregion

		protected LandPropertyController() {
			InitializeComponent();
			// create preview
			updatePreview();
			this.currentController = new Logic(this);
		}

		public override void updatePreview()
		{
			using( PreviewDrawer drawer = new PreviewDrawer( preview.Size, new Size(3,3), 0 ) ) 
			{
				for( int x=0; x<3; x++ )
					for( int y=0; y<3; y++ )
						drawer.draw( LandPropertyVoxel.sprite, x, y );
				if( preview.Image!=null ) preview.Image.Dispose();
				preview.Image = drawer.createBitmap();
			}
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
			
			preview.Image.Dispose();
		}

		#region Designer generated code
		private System.Windows.Forms.PictureBox preview;
		private System.ComponentModel.IContainer components = null;
		private freetrain.controls.CostBox costBox;
		private System.Windows.Forms.RadioButton buttonRemove;
		private System.Windows.Forms.RadioButton buttonPlace;

		private void InitializeComponent()
		{
			this.preview = new System.Windows.Forms.PictureBox();
			this.costBox = new freetrain.controls.CostBox();
			this.buttonRemove = new System.Windows.Forms.RadioButton();
			this.buttonPlace = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// preview
			// 
			this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.preview.Location = new System.Drawing.Point(8, 8);
			this.preview.Name = "preview";
			this.preview.Size = new System.Drawing.Size(96, 80);
			this.preview.TabIndex = 1;
			this.preview.TabStop = false;
			// 
			// costBox
			// 
			this.costBox.cost = 0;
			this.costBox.label = "Cost:";
			//! this.costBox.label = "費用：";
			this.costBox.Location = new System.Drawing.Point(8, 88);
			this.costBox.Name = "costBox";
			this.costBox.Size = new System.Drawing.Size(96, 32);
			this.costBox.TabIndex = 7;
			// 
			// buttonRemove
			// 
			this.buttonRemove.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonRemove.Location = new System.Drawing.Point(56, 120);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(48, 24);
			this.buttonRemove.TabIndex = 6;
			this.buttonRemove.Text = "Sell";
			//; this.buttonRemove.Text = "売却";
			this.buttonRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonPlace
			// 
			this.buttonPlace.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonPlace.Checked = true;
			this.buttonPlace.Location = new System.Drawing.Point(8, 120);
			this.buttonPlace.Name = "buttonPlace";
			this.buttonPlace.Size = new System.Drawing.Size(48, 24);
			this.buttonPlace.TabIndex = 5;
			this.buttonPlace.TabStop = true;
			this.buttonPlace.Text = "Buy";
			//! this.buttonPlace.Text = "購入";
			this.buttonPlace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// LandPropertyController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(115, 150);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.costBox,
																		  this.buttonRemove,
																		  this.buttonPlace,
																		  this.preview});
			this.Name = "LandPropertyController";
			this.Text = "Trade Land";
			//! this.Text = "土地売買";
			this.ResumeLayout(false);

		}
		#endregion

		private bool isPlacing { get { return buttonPlace.Checked; } }


		/// <summary>
		/// Controller logic
		/// </summary>
		private class Logic : RectSelectorController, MapOverlay
		{
			protected readonly LandPropertyController owner;
			
			internal Logic( LandPropertyController owner ) : base(owner.siteImpl) {
				this.owner = owner;
			}


			protected override void onRectSelected( Location loc1, Location loc2 ) {
				if( owner.isPlacing )
					buy(loc1,loc2);
				else
					sell(loc1,loc2);
			}

			protected override void onRectUpdated( Location loc1, Location loc2 ) {
				if( owner.isPlacing )
					owner.costBox.cost = computePriceForBuy(loc1,loc2);
				else
					owner.costBox.cost = computePriceForSell(loc1,loc2);
			}

			public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}

			public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {
				if( loc.z != anchor.z )	return;
				
				if( anchor!=UNPLACED && loc.inBetween(anchor,currentLoc) ) {
					if( owner.isPlacing )
						LandPropertyVoxel.sprite.drawAlpha( canvas.surface, pt );
					else
						ResourceUtil.emptyChip.drawAlpha( canvas.surface, pt );
				}
			}

			public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}
		}


		/// <summary>
		/// Buys region [loc1,loc2] and turn them into the privately owned property.
		/// </summary>
		public static void buy( Location loc1, Location loc2 ) {
			Debug.Assert( loc1.z==loc2.z );
			int z = loc1.z;

			for( int x=loc1.x; x<=loc2.x; x++ ) {
				for( int y=loc1.y; y<=loc2.y; y++ ) {
					Voxel v = World.world[x,y,z];
					if(v!=null && !v.entity.isOwned && v.entity.isSilentlyReclaimable) {
						// remove the old structure if possible
						AccountGenre.SUBSIDIARIES.spend( v.entity.entityValue );
						v.entity.remove();
					}
					v = World.world[x,y,z];

					if(v==null) {
						// buy it
						AccountGenre.SUBSIDIARIES.spend( World.world.landValue[new Location(x,y,z)] );
						new LandPropertyVoxel( new Location(x,y,z) );
					}
				}
			}
		}

		public static int computePriceForBuy( Location loc1, Location loc2 ) {
			int r=0;
			Set s = new Set();
			int z = loc1.z;

			for( int x=loc1.x; x<=loc2.x; x++ ) {
				for( int y=loc1.y; y<=loc2.y; y++ ) {
					Voxel v = World.world[x,y,z];
					if(v!=null && !v.entity.isOwned && v.entity.isSilentlyReclaimable) {
						// cost for removing this structure
						if( s.add(v.entity) )
							r += v.entity.entityValue;
					}
					v = World.world[x,y,z];

					if(v==null)
						// cost for the land
						r += World.world.landValue[new Location(x,y,z)];
				}
			}
			return r;
		}

		/// <summary>
		/// Sells land properties of the region [loc1,loc2].
		/// </summary>
		public static void sell( Location loc1, Location loc2 ) {
			Debug.Assert( loc1.z==loc2.z );
			int z = loc1.z;

			for( int x=loc1.x; x<=loc2.x; x++ ) {
				for( int y=loc1.y; y<=loc2.y; y++ ) {
					LandPropertyVoxel v = World.world[x,y,z] as LandPropertyVoxel;
					if(v!=null) {
						AccountGenre.SUBSIDIARIES.earn( v.landPrice );
						v.remove();
					}
				}
			}
		}

		public static int computePriceForSell( Location loc1, Location loc2 ) {
			int r=0;
			int z = loc1.z;

			for( int x=loc1.x; x<=loc2.x; x++ ) {
				for( int y=loc1.y; y<=loc2.y; y++ ) {
					LandPropertyVoxel v = World.world[x,y,z] as LandPropertyVoxel;
					if(v!=null)
						r += v.landPrice;
				}
			}
			return r;
		}
	}
}

