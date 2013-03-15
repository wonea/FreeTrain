using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.util;
using freetrain.world;
using freetrain.world.rail;
using freetrain.views;
using freetrain.views.map;

namespace freetrain.controllers.rail
{
	/// <summary>
	/// Railroad construction dialog
	/// </summary>
	/// This controller has two states.
	/// In one state, we expect the user to select one voxel.
	/// In the other state, we expect the user to select next voxel,
	/// so that we can build railroads.
	public class RailRoadController : AbstractControllerImpl, MapOverlay
	{
		#region Singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new RailRoadController();
			theInstance.Show();
			theInstance.Activate();
		}

		private static RailRoadController theInstance;

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			base.OnClosing(e);
			theInstance = null;
		}
		#endregion
		


		private RailRoadController() {
			InitializeComponent();
			updatePreview();
		}

		public override void updatePreview()
		{
			using( PreviewDrawer drawer = new PreviewDrawer( picture.Size, new Size(1,10), 0 ) ) 
			{
				for( int i=0; i<10; i++ )
					drawer.draw( RailPattern.get( Direction.NORTH, Direction.SOUTH ), 0, i );
				if(picture.Image!=null) picture.Image.Dispose();
				picture.Image = drawer.createBitmap();
			}
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
					components.Dispose();
			base.Dispose( disposing );
		}

		/// <summary>
		/// Updates the message in the dialog box.
		/// </summary>
		private void updateDialog() {
			message.Text = anchor!=UNPLACED?
				"Select end point":"Select starting point";
				//! "終点を選んでください":"始点を選んでください";
		}

		/// <summary>
		/// The first location selected by the user.
		/// </summary>
		private Location anchor = UNPLACED;

		/// <summary>
		/// Current mouse position. Used only when anchor!=UNPLACED
		/// </summary>
		private Location currentPos = UNPLACED;

		private static Location UNPLACED = freetrain.world.Location.UNPLACED;

		private bool isPlacing { get { return buttonPlace.Checked; } }

		#region Windows Form Designer generated code
		private System.Windows.Forms.RadioButton buttonPlace;
		private System.Windows.Forms.RadioButton buttonRemove;
		private freetrain.controls.CostBox costBox;
		private System.Windows.Forms.Label message;
		private System.Windows.Forms.PictureBox picture;
		private System.ComponentModel.Container components = null;

		private void InitializeComponent()
		{
			this.picture = new System.Windows.Forms.PictureBox();
			this.message = new System.Windows.Forms.Label();
			this.buttonPlace = new System.Windows.Forms.RadioButton();
			this.buttonRemove = new System.Windows.Forms.RadioButton();
			this.costBox = new freetrain.controls.CostBox();
			((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
			this.SuspendLayout();
			// 
			// picture
			// 
			this.picture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.picture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.picture.Location = new System.Drawing.Point(8, 9);
			this.picture.Name = "picture";
			this.picture.Size = new System.Drawing.Size(113, 113);
			this.picture.TabIndex = 0;
			this.picture.TabStop = false;
			// 
			// message
			// 
			this.message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.message.Location = new System.Drawing.Point(8, 125);
			this.message.Name = "message";
			this.message.Size = new System.Drawing.Size(113, 26);
			this.message.TabIndex = 1;
			this.message.Text = "Click on two points on the map to place tracks";
			this.message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//! this.message.Text = "マップの２点をクリックして敷設";
			// 
			// buttonPlace
			// 
			this.buttonPlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonPlace.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonPlace.Checked = true;
			this.buttonPlace.Location = new System.Drawing.Point(8, 185);
			this.buttonPlace.Name = "buttonPlace";
			this.buttonPlace.Size = new System.Drawing.Size(48, 26);
			this.buttonPlace.TabIndex = 2;
			this.buttonPlace.TabStop = true;
			this.buttonPlace.Text = "Place";
			//!this.buttonPlace.Text = "敷設";
			this.buttonPlace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.buttonPlace.CheckedChanged += new System.EventHandler(this.modeChanged);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemove.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonRemove.Location = new System.Drawing.Point(56, 185);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(65, 26);
			this.buttonRemove.TabIndex = 3;
			this.buttonRemove.Text = "Remove";
			//! this.buttonRemove.Text = "撤去";
			this.buttonRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.buttonRemove.CheckedChanged += new System.EventHandler(this.modeChanged);
			// 
			// costBox
			// 
			this.costBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.costBox.cost = 0;
			this.costBox.label = "Cost:";
			this.costBox.Location = new System.Drawing.Point(8, 154);
			//! this.costBox.label = "費用：";
			this.costBox.Name = "costBox";
			this.costBox.Size = new System.Drawing.Size(113, 25);
			this.costBox.TabIndex = 4;
			// 
			// RailRoadController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(129, 220);
			this.Controls.Add(this.costBox);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonPlace);
			this.Controls.Add(this.message);
			this.Controls.Add(this.picture);
			this.Resize += new System.EventHandler(this.updateAfterResize);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "RailRoadController";
			this.Text = "Track construction";
			//! this.Text = "線路工事";
			((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		public override void onClick( MapViewWindow source, Location loc, Point ab ) {
			if(anchor==UNPLACED) {
				anchor = loc;
				sameLevelDisambiguator = new SameLevelDisambiguator(anchor.z);
			} else {
				if(anchor!=loc) {
					if(isPlacing) {
						// build new railroads.
						if(!SingleRailRoad.build( anchor, loc ))
							MainWindow.showError("There are obstacles");
							//! MainWindow.showError("障害物があります");
					} else
						// remove existing ones
						SingleRailRoad.remove( anchor, loc );
				}
				anchor = UNPLACED;
			}

			updateDialog();
		}
		public override void onRightClick( MapViewWindow source, Location loc, Point ab ) {
			if( anchor==UNPLACED )
				Close();	// cancel
			else {
				// cancel the anchor
				if(currentPos!=UNPLACED)
					World.world.onVoxelUpdated(Cube.createInclusive(anchor,currentPos));
				anchor = UNPLACED;
				updateDialog();
			}
		}

		protected virtual void updateAfterResize(object sender, System.EventArgs e){
			this.buttonPlace.Width = this.picture.Width / 2;
			this.buttonRemove.Left = (this.buttonPlace.Left + this.buttonPlace.Width);
			this.buttonRemove.Width = this.buttonPlace.Width;
			updatePreview();
		}
		
		public override void onMouseMove( MapViewWindow view, Location loc, Point ab ) {
			if( anchor!=UNPLACED && isPlacing && currentPos!=loc ) {
				// update the screen
				if( currentPos!=UNPLACED )
					World.world.onVoxelUpdated(Cube.createInclusive(anchor,currentPos));
				currentPos = loc;
				World.world.onVoxelUpdated(Cube.createInclusive(anchor,currentPos));
				
				int cost;
				SingleRailRoad.comupteRoute( anchor, currentPos, out cost );
				costBox.cost = cost;
			}
			if( anchor!=UNPLACED && !isPlacing ) {
				costBox.cost = SingleRailRoad.calcCostOfRemoving( anchor, loc );
			}
		}


		public override void onDetached() {
			anchor = UNPLACED;
		}

		public override LocationDisambiguator disambiguator {
			get {
				// the 2nd selection must go to the same height as the anchor.
				if(anchor==UNPLACED)	return RailRoadDisambiguator.theInstance;
				else					return sameLevelDisambiguator;
			}
		}
		
		private LocationDisambiguator sameLevelDisambiguator;



		protected override void OnVisibleChanged(System.EventArgs e) {
			updateDialog();
		}
		



		// "place" or "remove" button was clicked. reset the anchor
		private void modeChanged(object sender, EventArgs e) {
			anchor = UNPLACED;
			updateDialog();
		}



		public void drawBefore( QuarterViewDrawer view, DrawContextEx canvas ) {
			if( anchor!=UNPLACED && isPlacing ) {
				int cost;
				canvas.tag = SingleRailRoad.comupteRoute( anchor, currentPos, out cost );
				if( canvas.tag!=null )
					Debug.WriteLine( ((IDictionary)canvas.tag).Count );
			}
		}

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {
			IDictionary dic = (IDictionary)canvas.tag;
			if( dic!=null ) {
				RailPattern rp = (RailPattern)dic[loc];
				if( rp!=null ) {
					for( int j=World.world.getGroundLevel(loc); j<loc.z; j++ )
						// TODO: ground level handling
						BridgePierVoxel.defaultSprite.drawAlpha(
							canvas.surface,
							view.fromXYZToClient(loc.x,loc.y,j) );

					rp.drawAlpha( canvas.surface, pt );
				}
			}
		}

		public void drawAfter( QuarterViewDrawer view, DrawContextEx canvas ) {
		}
	}

}
