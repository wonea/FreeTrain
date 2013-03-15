using System;
using System.Drawing;
using System.Windows.Forms;
using freetrain.controllers;
using freetrain.framework;
using freetrain.views;
using freetrain.views.map;
using freetrain.world.rail;
using org.kohsuke.directdraw;

namespace freetrain.world.rail.tattc
{
	/// <summary>
	/// ModalController that configures a TATTrainController.
	/// </summary>
	internal class TATModalController : AbstractControllerImpl, LocationDisambiguator, MapOverlay
	{
		public TATModalController( TATTrainController controller ) {
			this.controller = controller;
			InitializeComponent();

			this.Text = string.Format("Diagram settings",controller.name);
			//! this.Text = string.Format("ダイヤ設定「{0}」",controller.name);

			label1.Text = string.Format("{0}\n\n{1}",controller.name, label1.Text );
			Show();
			Activate();

			// redraw the view so that arrows will be displayed
			World.world.onAllVoxelUpdated();
		}

		private readonly TATTrainController controller;

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}


		public override void onClick( MapViewWindow view, Location loc, Point ab ) {
			JunctionRailRoad jrr = JunctionRailRoad.get(loc);
			if( jrr!=null ) {
				Form dialog = new JunctionConfigDialog( controller.getOrCreateJunction(loc) );
				dialog.ShowDialog(MainWindow.mainWindow);
				return;
			}

			Voxel v = World.world[loc];
			if(v==null)		return;

			TrainHarbor harbor = (TrainHarbor)v.queryInterface(typeof(TrainHarbor));

			if(harbor!=null) {
				StationHandler handler = controller.getStationHandler(harbor);
				StationConfigDialog dialog = new StationConfigDialog( handler );
				dialog.ShowDialog(MainWindow.mainWindow);
				controller.setStationHandler( harbor, dialog.currentHandler );
				return;
			}
		}


		//
		// Disambiguator implementation
		//
		public override LocationDisambiguator disambiguator { get { return this; } }

		public bool isSelectable( Location loc ) {
			Voxel v = World.world[loc];
			if(v==null)		return false;

			// any junctions, platforms, stations are selectable
			return JunctionRailRoad.get(loc)!=null || v.queryInterface(typeof(TrainHarbor))!=null;
		}


		//
		// MapOverlay implementation
		//
		public void drawVoxel( QuarterViewDrawer view, DrawContextEx context, Location loc, Point pt ) {
			JunctionRailRoad jrr = JunctionRailRoad.get(loc);
			if( jrr==null )	return;

			// draw an arrow that indicates the direction to go
			JunctionRoute go; bool isSimple=true;
			
			Junction j = controller.getJunction(loc);
			if(j==null)
				go = JunctionRoute.Straight;
			else {
				go = j.defaultRoute;
				if( j.advancedRules.Count!=0 )
					isSimple = false;
			}

			// draw an arrow.
			pt.Y -= 2;
			jrr.getDirection(go).drawArrow( context.surface, pt, !isSimple );
		}

		public void drawBefore( QuarterViewDrawer view, DrawContextEx context ) {}
		public void drawAfter( QuarterViewDrawer view, DrawContextEx context ) {}

		private void buttonOK_Click(object sender, System.EventArgs e) {
			Close();
		}

		#region Designer generated code
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOK;

		/// <summary>
		/// Designer サポートに必要なメソッドです。コード エディタで
		/// このメソッドのコンテンツを変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(121, 89);
			this.label1.TabIndex = 0;
			this.label1.Text = "Click on a point, platform, or station to set diagram settings";
			//! this.label1.Text = "ポイント、ホーム、駅をクリックしてダイヤを設定してください";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(50, 101);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(72, 26);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "&Close";
			//! this.buttonOK.Text = "閉じる(&C)";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// TATModalController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(134, 134);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label1);
			this.Name = "TATModalController";
			this.Text = "Diagram settings";
			//! this.Text = "ダイヤ設定";
			this.ResumeLayout(false);

		}
		#endregion
	
	}
}
