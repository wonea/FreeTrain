using System;
using System.Drawing;
using System.Windows.Forms;
using freetrain.controllers;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.rail;

namespace freetrain.world.terrain.terrace
{
	/// <summary>
	/// Places/removes terraces and cuts.
	/// </summary>
	internal class TerraceController : AbstractControllerImpl, MapOverlay
	{
		#region Singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new TerraceController();
			theInstance.Show();
			theInstance.Activate();
		}
		private System.Windows.Forms.RadioButton terrace;
		private System.Windows.Forms.RadioButton cut;
		private System.Windows.Forms.PictureBox preview;
		private System.Windows.Forms.Panel panel1;

		private static TerraceController theInstance;

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			base.OnClosing(e);
			theInstance = null;
		}
		#endregion

		#region Windows Form Designer generated code

		private System.Windows.Forms.RadioButton buttonPlace;
		private System.Windows.Forms.RadioButton buttonRemove;
		private System.ComponentModel.Container components = null;

		private void InitializeComponent() {
			this.buttonPlace = new System.Windows.Forms.RadioButton();
			this.buttonRemove = new System.Windows.Forms.RadioButton();
			this.preview = new System.Windows.Forms.PictureBox();
			this.terrace = new System.Windows.Forms.RadioButton();
			this.cut = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonPlace
			// 
			this.buttonPlace.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonPlace.Checked = true;
			this.buttonPlace.Location = new System.Drawing.Point(4, 113);
			this.buttonPlace.Name = "buttonPlace";
			this.buttonPlace.Size = new System.Drawing.Size(59, 26);
			this.buttonPlace.TabIndex = 4;
			this.buttonPlace.TabStop = true;
			this.buttonPlace.Text = "Place";
			//! this.buttonPlace.Text = "設置";
			this.buttonPlace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.buttonPlace.CheckedChanged += new System.EventHandler(this.onStrategyChanged);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.buttonRemove.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonRemove.Location = new System.Drawing.Point(63, 113);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(58, 26);
			this.buttonRemove.TabIndex = 5;
			this.buttonRemove.Text = "Remove";
			//! this.buttonRemove.Text = "撤去";
			this.buttonRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.buttonRemove.CheckedChanged += new System.EventHandler(this.onStrategyChanged);
			// 
			// preview
			// 
			this.preview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.preview.BackColor = System.Drawing.Color.White;
			this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.preview.Location = new System.Drawing.Point(4, 41);
			this.preview.Name = "preview";
			this.preview.Size = new System.Drawing.Size(117, 61);
			this.preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.preview.TabIndex = 0;
			this.preview.TabStop = false;
			// 
			// terrace
			// 
			this.terrace.Appearance = System.Windows.Forms.Appearance.Button;
			this.terrace.Checked = true;
			this.terrace.Location = new System.Drawing.Point(4, 9);
			this.terrace.Name = "terrace";
			this.terrace.Size = new System.Drawing.Size(59, 26);
			this.terrace.TabIndex = 6;
			this.terrace.TabStop = true;
			this.terrace.Text = "Terrace";
			//! this.terrace.Text = "雛壇";
			this.terrace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.terrace.CheckedChanged += new System.EventHandler(this.onStrategyChanged);
			// 
			// cut
			// 
			this.cut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cut.Appearance = System.Windows.Forms.Appearance.Button;
			this.cut.Location = new System.Drawing.Point(63, 9);
			this.cut.Name = "cut";
			this.cut.Size = new System.Drawing.Size(58, 26);
			this.cut.TabIndex = 7;
			this.cut.Text = "Cut";
			//! this.cut.Text = "切通";
			this.cut.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.cut.CheckedChanged += new System.EventHandler(this.onStrategyChanged);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.terrace);
			this.panel1.Controls.Add(this.cut);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(125, 35);
			this.panel1.TabIndex = 8;
			// 
			// TerraceController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(126, 146);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.buttonPlace);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.preview);
			this.Name = "TerraceController";
			this.Text = "Terraces & Cuts";
			//! this.Text = "雛壇と切り通し";
			((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		/// <summary> Current behavior of this controller. </summary>
		private Strategy strategy;

		private TerraceController() {
			InitializeComponent();
			onStrategyChanged(null,null);
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
					components.Dispose();
//			terracePreview.Dispose();
//			cutPreview.Dispose();
			base.Dispose( disposing );
		}

		private bool isPlacing { get { return buttonPlace.Checked; } }
		private bool isTerrace { get { return terrace.Checked; } }


		/// <summary>
		/// The user switch between terrace and cut.
		/// </summary>
		private void onStrategyChanged(object sender, System.EventArgs e) {
			if( isPlacing ) {
				if( isTerrace ) {
					strategy = new TerracePlacementStrategy();
				} else {
					strategy = new CliffPlacementStrategy();
				}
			} else {
				if( isTerrace ) {
					strategy = new TerraceRemovalStrategy();
				} else {
					strategy = new CliffRemovalStrategy();
				}
			}
			// TODO update preview
		}

		public override LocationDisambiguator disambiguator {
			get {
				return strategy.disambiguator;
			}
		}


		private Location baseLoc = world.Location.UNPLACED;
		public override void onMouseMove(MapViewWindow view, Location loc, Point ab) {
			World w = World.world;

			if(baseLoc!=loc) {
				// update the screen
				w.onVoxelUpdated(baseLoc);
				baseLoc = loc;
				w.onVoxelUpdated(baseLoc);
			}
		}

		public override void onClick(MapViewWindow view, Location loc, Point ab) {
			strategy.onClick(view,loc,ab);
		}
	
		public void drawVoxel( QuarterViewDrawer view, DrawContextEx dc, Location loc, Point pt ) {
			if( baseLoc==loc )
				strategy.drawVoxel(view,dc,loc,pt);
		}
		public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}
		public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}
	}
}
