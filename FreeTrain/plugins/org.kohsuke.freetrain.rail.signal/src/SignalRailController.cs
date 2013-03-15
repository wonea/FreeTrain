using System;
using System.Drawing;
using System.Windows.Forms;
using freetrain.controllers;
using freetrain.controllers.structs;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.rail;
using freetrain.controllers.rail;

namespace freetrain.world.rail.signal
{
	/// <summary>
	/// Places/removes signal rail.
	/// </summary>
	public class SignalRailController : AbstractControllerImpl, MapOverlay
	{
		#region Singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new SignalRailController();
			theInstance.Show();
			theInstance.Activate();
		}

		private System.Windows.Forms.PictureBox preview;
		private freetrain.controls.IndexSelector dirSelector;
		private System.Windows.Forms.ComboBox typeBox;

		private static SignalRailController theInstance;

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
			this.preview = new System.Windows.Forms.PictureBox();
			this.buttonPlace = new System.Windows.Forms.RadioButton();
			this.buttonRemove = new System.Windows.Forms.RadioButton();
			this.dirSelector = new freetrain.controls.IndexSelector();
			this.typeBox = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
			this.SuspendLayout();
			// 
			// preview
			// 
			this.preview.BackColor = System.Drawing.Color.White;
			this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.preview.Location = new System.Drawing.Point(8, 61);
			this.preview.Name = "preview";
			this.preview.Size = new System.Drawing.Size(104, 60);
			this.preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.preview.TabIndex = 0;
			this.preview.TabStop = false;
			// 
			// buttonPlace
			// 
			this.buttonPlace.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonPlace.Checked = true;
			this.buttonPlace.Location = new System.Drawing.Point(8, 127);
			this.buttonPlace.Name = "buttonPlace";
			this.buttonPlace.Size = new System.Drawing.Size(49, 26);
			this.buttonPlace.TabIndex = 4;
			this.buttonPlace.TabStop = true;
			this.buttonPlace.Text = "Place";
			//! this.buttonPlace.Text = "設置";
			this.buttonPlace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonRemove
			// 
			this.buttonRemove.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonRemove.Location = new System.Drawing.Point(56, 127);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(56, 26);
			this.buttonRemove.TabIndex = 5;
			this.buttonRemove.Text = "Remove";
			//! this.buttonRemove.Text = "撤去";
			this.buttonRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// dirSelector
			// 
			this.dirSelector.count = 4;
			this.dirSelector.current = 0;
			this.dirSelector.Cursor = System.Windows.Forms.Cursors.Default;
			this.dirSelector.dataSource = null;
			this.dirSelector.Location = new System.Drawing.Point(8, 35);
			this.dirSelector.Name = "dirSelector";
			this.dirSelector.Size = new System.Drawing.Size(104, 21);
			this.dirSelector.TabIndex = 6;
			this.dirSelector.indexChanged += new System.EventHandler(this.onDirChanged);
			// 
			// typeBox
			// 
			this.typeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.typeBox.Location = new System.Drawing.Point(8, 9);
			this.typeBox.Name = "typeBox";
			this.typeBox.Size = new System.Drawing.Size(104, 21);
			this.typeBox.TabIndex = 7;
			this.typeBox.SelectedIndexChanged += new System.EventHandler(this.onTypeChanged);
			// 
			// SignalRailController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(120, 161);
			this.Controls.Add(this.typeBox);
			this.Controls.Add(this.dirSelector);
			this.Controls.Add(this.buttonPlace);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.preview);
			this.Name = "SignalRailController";
			this.Text = "Signal";
			//! this.Text = "信号";
			((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		
		private RailSignalContribution[] signals;

		private SignalRailController() {
			InitializeComponent();
			World.world.viewOptions.OnViewOptionChanged+=new OptionChangedHandler(updatePreview);

			signals = (RailSignalContribution[])
				PluginManager.theInstance.listContributions(typeof(RailSignalContribution));

			typeBox.DataSource = signals;
		}

		protected override void Dispose( bool disposing ) {
			World.world.viewOptions.OnViewOptionChanged-=new OptionChangedHandler(updatePreview);
			if( disposing && components != null)
					components.Dispose();
			base.Dispose( disposing );
		}


		private void onTypeChanged(object sender, System.EventArgs e) {
			updatePreview();
		}

		private void onDirChanged(object sender, System.EventArgs e) {
			updatePreview();
		}

		public override void updatePreview() {
			RailPattern rp = RailPattern.get(currentDirection,currentDirection.opposite);

			using( PreviewDrawer drawer = new PreviewDrawer( preview.Size, new Size(1,1), 0 ) ) {
				// draw the rail
				for( int i=-5; i<5; i++ ) {
					if( currentDirection.isParallelToX )	drawer.draw( rp, i, 0 );
					else									drawer.draw( rp, 0, i );
				}

				// draw the signal
				drawer.draw( rp, 0,0 );
				drawer.draw( currentType.getSprite(currentDirection), 0, 0 );

				// draw the arrow
				currentDirection.drawArrow( drawer.surface,
					drawer.getPoint( -currentDirection.offsetX, -currentDirection.offsetY) );

				preview.Image = drawer.createBitmap();
			}
		}

		private RailSignalContribution currentType {
			get {
				return (RailSignalContribution)typeBox.SelectedItem;
			}
		}
		private Direction currentDirection {
			get {
				return Direction.get(dirSelector.current*2);
			}
		}

		private bool isPlacing { get { return buttonPlace.Checked; } }


		public override LocationDisambiguator disambiguator {
			get {
				return RailRoadDisambiguator.theInstance;
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
			if(isPlacing) {
				TrafficVoxel tv = TrafficVoxel.getOrCreate(loc);
				if(tv==null) {
					MainWindow.showError("There are obstacles");
					//! MainWindow.showError("障害物があります");
					return;
				}

				if(tv.railRoad==null || tv.railRoad is SingleRailRoad)
					new SignalRailRoad(tv,currentType,currentDirection);
				else
					MainWindow.showError("Can not place on this rail");
					//! MainWindow.showError("設置できない線路です");
			} else {
				SignalRailRoad srr = RailRoad.get(loc) as SignalRailRoad;
				if(srr!=null)
					srr.remove();
			}
		}
	
		public void drawVoxel( QuarterViewDrawer view, DrawContextEx dc, Location loc, Point pt ) {
			if( loc==baseLoc ) {
				RailPattern.get( currentDirection, currentDirection.opposite )
					.drawAlpha( dc.surface, pt );
				currentType.getSprite(currentDirection)
					.drawAlpha( dc.surface, pt );
			}
		}
		public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}
		public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}
	}
}
