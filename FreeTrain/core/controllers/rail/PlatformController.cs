using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using freetrain.contributions.common;
using freetrain.contributions.rail;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.util;
using freetrain.world;
using freetrain.world.rail;
using freetrain.views;
using freetrain.views.map;
using org.kohsuke.directdraw;
using freetrain.controls;


namespace freetrain.controllers.rail
{
	public class PlatformController : AbstractControllerImpl, MapOverlay, LocationDisambiguator
	{
		#region Singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new PlatformController();
			theInstance.Show();
			theInstance.Activate();
		}

		private freetrain.controls.IndexSelector indexSelector;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private freetrain.controls.IndexSelector indexSelector1;
		private System.Windows.Forms.Label label4;
		private freetrain.controls.IndexSelector indexSelector2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;

		private static PlatformController theInstance;

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			base.OnClosing(e);
			theInstance = null;
		}
		#endregion


		private Bitmap bitmapN,bitmapS,bitmapE,bitmapW;
		private Bitmap stationPreviewBitmap;

		public PlatformController() {
			// この呼び出しは Windows フォーム デザイナで必要です。
			InitializeComponent();
//			colorPickButton1.colorLibraries = new IColorLibrary[]{
//				(IColorLibrary)PluginManager.theInstance.getContribution("{COLORLIB-RAINBOW}"),
//				(IColorLibrary)PluginManager.theInstance.getContribution("{COLORLIB-STONES}")//,
////				(IColorLibrary)PluginManager.theInstance.getContribution("{COLORLIB-WOODS}"),
////				(IColorLibrary)PluginManager.theInstance.getContribution("{COLORLIB-METALS}"),
////				(IColorLibrary)PluginManager.theInstance.getContribution("{COLORLIB-BRICKS}"),
////				(IColorLibrary)PluginManager.theInstance.getContribution("{COLORLIB-DIRTS}"),
////				(IColorLibrary)PluginManager.theInstance.getContribution("{COLORLIB-PASTEL}"),
////				(IColorLibrary)PluginManager.theInstance.getContribution("{COLORLIB-COLPLATE}"),
////				(IColorLibrary)PluginManager.theInstance.getContribution("{COLORLIB-ROOF}")
//			};

			dirN.Tag = Direction.NORTH;
			dirE.Tag = Direction.EAST;
			dirS.Tag = Direction.SOUTH;
			dirW.Tag = Direction.WEST;

			// load pictures
			bitmapN=ResourceUtil.loadSystemBitmap("PlatformN.bmp");
			dirN.Image = bitmapN;

			bitmapE=ResourceUtil.loadSystemBitmap("PlatformN.bmp");
			bitmapE.RotateFlip( RotateFlipType.Rotate90FlipNone );
			dirE.Image = bitmapE;

			bitmapS=ResourceUtil.loadSystemBitmap("PlatformN.bmp");
			bitmapS.RotateFlip( RotateFlipType.Rotate180FlipNone );
			dirS.Image = bitmapS;

			bitmapW=ResourceUtil.loadSystemBitmap("PlatformN.bmp");
			bitmapW.RotateFlip( RotateFlipType.Rotate270FlipNone );
			dirW.Image = bitmapW;

			// load station type list
			stationType.DataSource = Core.plugins.stationGroup;
			stationType.DisplayMember="name";

			updateAfterResize(null,null);
			onDirChange(dirN,null);
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
					components.Dispose();
			base.Dispose( disposing );

			bitmapN.Dispose();
			bitmapS.Dispose();
			bitmapE.Dispose();
			bitmapW.Dispose();
			stationPreviewBitmap.Dispose();
			if(alphaSprites!=null)
				alphaSprites.Dispose();
		}

		public override LocationDisambiguator disambiguator { get { return this; } }

		/// <summary> LocationDisambiguator implementation </summary>
		public bool isSelectable( Location loc ) {
			if( currentMode==Mode.Station ) {
				return GroundDisambiguator.theInstance.isSelectable(loc);
			}

			if(isPlacing) {
				// align to RRs or the ground

				if( currentMode==Mode.FatPlatform )
					loc += direction.right90;

				if( GroundDisambiguator.theInstance.isSelectable(loc) )
					return true;

				RailRoad rr = RailRoad.get(loc);
				if(rr==null)	return false;
				return rr.hasRail(direction) && rr.hasRail(direction.opposite);
			} else {
				return Platform.get(loc)!=null;
			}
		}

		#region Designer generated code
		private System.Windows.Forms.TabPage stationPage;
		private System.Windows.Forms.ComboBox stationType;
		private System.Windows.Forms.PictureBox stationPicture;
		private System.Windows.Forms.TabPage platformPage;
		private System.Windows.Forms.PictureBox dirS;
		private System.Windows.Forms.PictureBox dirW;
		private System.Windows.Forms.PictureBox dirE;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown lengthBox;
		private System.Windows.Forms.PictureBox dirN;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.CheckBox checkSlim;
		private System.Windows.Forms.RadioButton buttonRemove;
		private System.Windows.Forms.RadioButton buttonPlace;
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Designer サポートに必要なメソッドです。コード エディタで
		/// このメソッドのコンテンツを変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonRemove = new System.Windows.Forms.RadioButton();
			this.buttonPlace = new System.Windows.Forms.RadioButton();
			this.stationPage = new System.Windows.Forms.TabPage();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.indexSelector = new freetrain.controls.IndexSelector();
			this.stationType = new System.Windows.Forms.ComboBox();
			this.stationPicture = new System.Windows.Forms.PictureBox();
			this.indexSelector1 = new freetrain.controls.IndexSelector();
			this.label4 = new System.Windows.Forms.Label();
			this.indexSelector2 = new freetrain.controls.IndexSelector();
			this.label5 = new System.Windows.Forms.Label();
			this.platformPage = new System.Windows.Forms.TabPage();
			this.checkSlim = new System.Windows.Forms.CheckBox();
			this.dirS = new System.Windows.Forms.PictureBox();
			this.dirW = new System.Windows.Forms.PictureBox();
			this.dirE = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lengthBox = new System.Windows.Forms.NumericUpDown();
			this.dirN = new System.Windows.Forms.PictureBox();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.stationPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.stationPicture)).BeginInit();
			this.platformPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dirS)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dirW)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dirE)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lengthBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dirN)).BeginInit();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonRemove.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonRemove.Location = new System.Drawing.Point(87, 210);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(76, 26);
			this.buttonRemove.TabIndex = 1;
			this.buttonRemove.Text = "Remove";
			//! this.buttonRemove.Text = "撤去";
			this.buttonRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonPlace
			// 
			this.buttonPlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonPlace.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonPlace.Checked = true;
			this.buttonPlace.Location = new System.Drawing.Point(7, 210);
			this.buttonPlace.Name = "buttonPlace";
			this.buttonPlace.Size = new System.Drawing.Size(74, 26);
			this.buttonPlace.TabIndex = 0;
			this.buttonPlace.TabStop = true;
			this.buttonPlace.Text = "Build";
			//! this.buttonPlace.Text = "設置";
			this.buttonPlace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// stationPage
			// 
			this.stationPage.BackColor = System.Drawing.SystemColors.Control;
			this.stationPage.Controls.Add(this.label3);
			this.stationPage.Controls.Add(this.label2);
			this.stationPage.Controls.Add(this.listView1);
			this.stationPage.Controls.Add(this.indexSelector);
			this.stationPage.Controls.Add(this.stationType);
			this.stationPage.Controls.Add(this.stationPicture);
			this.stationPage.Controls.Add(this.indexSelector1);
			this.stationPage.Controls.Add(this.label4);
			this.stationPage.Controls.Add(this.indexSelector2);
			this.stationPage.Controls.Add(this.label5);
			this.stationPage.Location = new System.Drawing.Point(4, 21);
			this.stationPage.Margin = new System.Windows.Forms.Padding(0);
			this.stationPage.Name = "stationPage";
			this.stationPage.Size = new System.Drawing.Size(193, 176);
			this.stationPage.TabIndex = 1;
			this.stationPage.Text = "Station";
			//! this.stationPage.Text = "駅舎";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 35);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(47, 17);
			this.label3.TabIndex = 6;
			this.label3.Text = "Design:";
			//! this.label3.Text = "デザイン：";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(294, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 17);
			this.label2.TabIndex = 5;
			this.label2.Text = "label2";
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
									this.columnHeader1,
									this.columnHeader2,
									this.columnHeader3,
									this.columnHeader4,
									this.columnHeader5,
									this.columnHeader6});
			this.listView1.Location = new System.Drawing.Point(206, 9);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(103, 156);
			this.listView1.TabIndex = 4;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			//! this.columnHeader1.Text = "名前";
			this.columnHeader1.Width = 120;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Scale";
			//! this.columnHeader2.Text = "規模";
			this.columnHeader2.Width = 54;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Material";
			//! this.columnHeader3.Text = "材質";
			this.columnHeader3.Width = 54;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Size";
			//! this.columnHeader4.Text = "サイズ";
			this.columnHeader4.Width = 64;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Construction cost";
			//! this.columnHeader5.Text = "建造費";
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Maintenance";
			//! this.columnHeader6.Text = "維持費";
			// 
			// indexSelector
			// 
			this.indexSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.indexSelector.count = 10;
			this.indexSelector.current = 0;
			this.indexSelector.dataSource = null;
			this.indexSelector.Location = new System.Drawing.Point(56, 35);
			this.indexSelector.Name = "indexSelector";
			this.indexSelector.Size = new System.Drawing.Size(132, 17);
			this.indexSelector.TabIndex = 3;
			this.indexSelector.indexChanged += new System.EventHandler(this.onStationChanged);
			// 
			// stationType
			// 
			this.stationType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.stationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.stationType.ItemHeight = 13;
			this.stationType.Location = new System.Drawing.Point(3, 9);
			this.stationType.Name = "stationType";
			this.stationType.Size = new System.Drawing.Size(185, 21);
			this.stationType.Sorted = true;
			this.stationType.TabIndex = 2;
			this.stationType.SelectedIndexChanged += new System.EventHandler(this.onGroupChanged);
			// 
			// stationPicture
			// 
			this.stationPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.stationPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.stationPicture.Location = new System.Drawing.Point(3, 54);
			this.stationPicture.Name = "stationPicture";
			this.stationPicture.Size = new System.Drawing.Size(185, 112);
			this.stationPicture.TabIndex = 1;
			this.stationPicture.TabStop = false;
			// 
			// indexSelector1
			// 
			this.indexSelector1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.indexSelector1.count = 10;
			this.indexSelector1.current = 0;
			this.indexSelector1.dataSource = null;
			this.indexSelector1.Location = new System.Drawing.Point(250, 106);
			this.indexSelector1.Name = "indexSelector1";
			this.indexSelector1.Size = new System.Drawing.Size(96, 17);
			this.indexSelector1.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(260, 106);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 17);
			this.label4.TabIndex = 6;
			this.label4.Text = "Direction:";
			//! this.label4.Text = "方向：";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// indexSelector2
			// 
			this.indexSelector2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.indexSelector2.count = 10;
			this.indexSelector2.current = 0;
			this.indexSelector2.dataSource = null;
			this.indexSelector2.Location = new System.Drawing.Point(250, 123);
			this.indexSelector2.Name = "indexSelector2";
			this.indexSelector2.Size = new System.Drawing.Size(96, 17);
			this.indexSelector2.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(260, 123);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 17);
			this.label5.TabIndex = 6;
			this.label5.Text = "Color:";
			//! this.label5.Text = "色：";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// platformPage
			// 
			this.platformPage.Controls.Add(this.checkSlim);
			this.platformPage.Controls.Add(this.dirS);
			this.platformPage.Controls.Add(this.dirW);
			this.platformPage.Controls.Add(this.dirE);
			this.platformPage.Controls.Add(this.label1);
			this.platformPage.Controls.Add(this.lengthBox);
			this.platformPage.Controls.Add(this.dirN);
			this.platformPage.Location = new System.Drawing.Point(4, 21);
			this.platformPage.Name = "platformPage";
			this.platformPage.Size = new System.Drawing.Size(187, 176);
			this.platformPage.TabIndex = 0;
			this.platformPage.Text = "Platform";
			//! this.platformPage.Text = "ホーム";
			// 
			// checkSlim
			// 
			this.checkSlim.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.checkSlim.Location = new System.Drawing.Point(8, 130);
			this.checkSlim.Name = "checkSlim";
			this.checkSlim.Size = new System.Drawing.Size(173, 17);
			this.checkSlim.TabIndex = 7;
			this.checkSlim.Text = "Slim platform";
			//!this.checkSlim.Text = "スリムなホーム";
			this.checkSlim.CheckedChanged += new System.EventHandler(this.onModeChanged);
			// 
			// dirS
			// 
			this.dirS.BackColor = System.Drawing.Color.White;
			this.dirS.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dirS.Location = new System.Drawing.Point(64, 69);
			this.dirS.Name = "dirS";
			this.dirS.Size = new System.Drawing.Size(48, 52);
			this.dirS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.dirS.TabIndex = 6;
			this.dirS.TabStop = false;
			this.dirS.Click += new System.EventHandler(this.onDirChange);
			// 
			// dirW
			// 
			this.dirW.BackColor = System.Drawing.Color.White;
			this.dirW.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dirW.Location = new System.Drawing.Point(8, 69);
			this.dirW.Name = "dirW";
			this.dirW.Size = new System.Drawing.Size(48, 52);
			this.dirW.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.dirW.TabIndex = 5;
			this.dirW.TabStop = false;
			this.dirW.Click += new System.EventHandler(this.onDirChange);
			// 
			// dirE
			// 
			this.dirE.BackColor = System.Drawing.Color.White;
			this.dirE.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dirE.Location = new System.Drawing.Point(64, 9);
			this.dirE.Name = "dirE";
			this.dirE.Size = new System.Drawing.Size(48, 52);
			this.dirE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.dirE.TabIndex = 4;
			this.dirE.TabStop = false;
			this.dirE.Click += new System.EventHandler(this.onDirChange);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(3, 147);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 20);
			this.label1.TabIndex = 2;
			this.label1.Text = "&Length:";
			//! this.label1.Text = "長さ(&L)：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lengthBox
			// 
			this.lengthBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.lengthBox.Location = new System.Drawing.Point(64, 149);
			this.lengthBox.Name = "lengthBox";
			this.lengthBox.Size = new System.Drawing.Size(117, 20);
			this.lengthBox.TabIndex = 3;
			this.lengthBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.lengthBox.Value = new decimal(new int[] {
									5,
									0,
									0,
									0});
			this.lengthBox.Validating += new System.ComponentModel.CancelEventHandler(this.validateLength);
			this.lengthBox.TextChanged += new System.EventHandler(this.onLengthChanged);
			// 
			// dirN
			// 
			this.dirN.BackColor = System.Drawing.Color.White;
			this.dirN.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dirN.Location = new System.Drawing.Point(8, 9);
			this.dirN.Name = "dirN";
			this.dirN.Size = new System.Drawing.Size(48, 52);
			this.dirN.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.dirN.TabIndex = 1;
			this.dirN.TabStop = false;
			this.dirN.Click += new System.EventHandler(this.onDirChange);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.stationPage);
			this.tabControl.Controls.Add(this.platformPage);
			this.tabControl.ItemSize = new System.Drawing.Size(42, 17);
			this.tabControl.Location = new System.Drawing.Point(-3, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(201, 201);
			this.tabControl.TabIndex = 0;
			this.tabControl.Click += new System.EventHandler(this.updateAfterResize);
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.onModeChanged);
			// 
			// PlatformController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(197, 239);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.buttonPlace);
			this.Controls.Add(this.buttonRemove);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "PlatformController";
			this.Text = "Station construction";
			//! this.Text = "駅工事";
			this.Resize += new System.EventHandler(this.updateAfterResize);
			this.stationPage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.stationPicture)).EndInit();
			this.platformPage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dirS)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dirW)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dirE)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lengthBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dirN)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary> The direction of the platform </summary>
		private Direction direction;

		private RailPattern railPattern { get { return RailPattern.get(direction,direction.opposite); } }

		/// <summary>
		/// Called when the direction of a platform is changed.
		/// </summary>
		private void onDirChange(object sender, System.EventArgs e) {
			updatePlatformBox(sender,dirN);
			updatePlatformBox(sender,dirS);
			updatePlatformBox(sender,dirE);
			updatePlatformBox(sender,dirW);
			updateAlphaSprites();
		}
		
		protected virtual void updateAfterResize(object sender, System.EventArgs e){
			this.buttonPlace.Width = ((this.tabControl.Left + this.tabControl.Width) - (buttonPlace.Left * 2)) / 2;
			this.buttonRemove.Left = (this.buttonPlace.Left + this.buttonPlace.Width);
			this.buttonRemove.Width = this.buttonPlace.Width;
			this.dirN.Width = (this.tabControl.Width - 30) / 2;
			this.dirN.Height = (this.checkSlim.Top - 20) / 2;
			this.dirE.Size = this.dirN.Size;
			this.dirS.Size = this.dirN.Size;
			this.dirW.Size = this.dirN.Size;
			this.dirE.Left = this.dirN.Left + this.dirN.Width + 5;
			this.dirS.Left = this.dirE.Left;
			this.dirS.Top = this.dirN.Top + this.dirN.Height + 5;
			this.dirW.Top = this.dirS.Top;
			updatePreview();
		}

		private void updatePlatformBox( object sender, PictureBox pic ) {
			if(pic==sender) {
				direction = (Direction)pic.Tag;
				pic.BorderStyle = BorderStyle.Fixed3D;
			} else {
				pic.BorderStyle = BorderStyle.None;
			}
		}

		private bool isPlacing { get { return buttonPlace.Checked; } }
		
		private enum Mode {
			Station,
			ThinPlatform,
			FatPlatform
		}

		/// <summary>
		/// Returns true if the current page is the station page.
		/// </summary>
		private Mode currentMode {
			get {
				if( tabControl.SelectedIndex==0 )	return Mode.Station;
				if( checkSlim.Checked )				return Mode.ThinPlatform;
				else								return Mode.FatPlatform;
			}
		}


		private Location baseLoc = world.Location.UNPLACED;
		public override void onMouseMove(MapViewWindow view, Location loc, Point ab ) {
			World w = World.world;

			if(baseLoc!=loc) {
				// update the screen
				updateVoxels();
				baseLoc = loc;
				updateVoxels();
			}
		}

		private void updateVoxels() {
			Location loc2 = baseLoc;
			if(currentMode==Mode.Station) {
				loc2 += selectedStation.size;
			} else {
				// platform
				loc2.x += direction.offsetX*length;
				loc2.y += direction.offsetY*length;
				loc2 += direction.right90;		// width 1 by default
				loc2.z++;
				if( currentMode==Mode.FatPlatform ) {
					loc2 += direction.right90;	// for the attached rail road, width is two
				}
			} 
			World.world.onVoxelUpdated(Cube.createExclusive(baseLoc,loc2));
		}

		public override void onClick(MapViewWindow view, Location loc, Point ab ) {
			switch( currentMode ) {
			case Mode.Station:
				if( isPlacing ) {
					if(!selectedStation.canBeBuilt(loc,ControlMode.player)) {
						MainWindow.showError("Can not build");
						//! MainWindow.showError("設置できません");
					} else {
						selectedStation.create(loc,true);
					}
				} else {
					Station s = Station.get(loc);
					if(s!=null)		s.remove();
				}
				return;

			case Mode.FatPlatform:
				if( isPlacing ) {
					if(!FatPlatform.canBeBuilt(loc,direction,length)) {
						MainWindow.showError("Can not build");
						//! MainWindow.showError("設置できません");
						return;
					}
					new FatPlatform(loc,direction,length);
				} else {
					Platform p = Platform.get(loc);
					if(p!=null) {
						if(p.canRemove)
							p.remove();
						else
							MainWindow.showError("Can not remove");
							//! MainWindow.showError("撤去できません");
					}
				}
				return;

			case Mode.ThinPlatform:
				if( isPlacing ) {
					if(!ThinPlatform.canBeBuilt(loc,direction,length)) {
						MainWindow.showError("Can not build");
						//! MainWindow.showError("設置できません");
						return;
					}
					new ThinPlatform(loc,direction,length);
				} else {
					Platform p = Platform.get(loc);
					if(p!=null) {
						if(p.canRemove)
							p.remove();
						else
							MainWindow.showError("Can not remove");
							//! MainWindow.showError("撤去できません");
					}
				}
				return;
			}
		}

		private void validateLength(object sender, CancelEventArgs e) {
			// only allow a value longer than 1
			try {
				e.Cancel = lengthBox.Value < 1;
			} catch( Exception ) {
				e.Cancel = true;
			}
		}

		/// <summary> Length of the platform to be built. </summary>
		private int length { get { return (int)lengthBox.Value; } }

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx dc, Location loc, Point pt ) {
			if( loc.z != baseLoc.z || !isPlacing)	return;

			Surface canvas = dc.surface;
			
			switch( this.currentMode ) {
			case Mode.Station:
				if( Cube.createExclusive( baseLoc, alphaSprites.size ).contains(loc) )
					alphaSprites.getSprite( loc-baseLoc ).drawAlpha( canvas, pt );
				break;

			case Mode.ThinPlatform:
				// adjustment
				if( direction==Direction.NORTH )	loc.y += length-1;
				if( direction==Direction.WEST )		loc.x += length-1;

				if( Cube.createExclusive( baseLoc, alphaSprites.size ).contains(loc) )
					alphaSprites.getSprite( loc-baseLoc ).drawAlpha( canvas, pt );
				break;

			case Mode.FatPlatform:
				// left-top corner of the platform to be drawn
				Location ptLT = baseLoc;
				switch(direction.index) {
				case 0:	// NORTH
					ptLT.y -= length-1;
					break;
				case 2: // EAST
					break;	// no adjustment
				case 4: // SOUTH
					ptLT.x -= 1;
					break;
				case 6: // WEST
					ptLT.x -= length-1;
					ptLT.y -= 1;
					break;
				}
					

				if( direction.isParallelToX ) {
					int y = ptLT.y;
					if( (loc.y==y || loc.y==y+1)
					&&  ptLT.x<=loc.x && loc.x<ptLT.x+length )
						alphaSprites.sprites[ loc.x-ptLT.x, loc.y-y, 0 ].drawAlpha( canvas, pt );
				} else {
					int x = ptLT.x;
					if( (loc.x==x || loc.x==x+1)
					&&  ptLT.y<=loc.y && loc.y<ptLT.y+length )
						alphaSprites.sprites[ loc.x-x, loc.y-ptLT.y, 0 ].drawAlpha( canvas, pt );
				}
				break;
			}
		}

		public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}
		public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}

		private void onGroupChanged(object sender, System.EventArgs e) {
			indexSelector.dataSource = (StructureGroup)stationType.SelectedItem;
			onStationChanged(null,null);
		}

		/// <summary>
		/// Called when a selection of the station has changed.
		/// </summary>
		private void onStationChanged( object sender, EventArgs e ) {
			// Builds a new preview bitmap and set it to the picture box
			PreviewDrawer drawer;
			
			drawer = new PreviewDrawer( stationPicture.ClientSize, selectedStation.size );
			drawer.drawCenter( selectedStation.sprites );

			if( stationPreviewBitmap!=null )	stationPreviewBitmap.Dispose();
			stationPicture.Image = stationPreviewBitmap = drawer.createBitmap();

			drawer.Dispose();

			updateAlphaSprites();
		}

		public override void updatePreview()
		{
			if( this.currentMode==Mode.Station )
				onStationChanged(null,null);
		}

		private StationContribution selectedStation { get { return (StationContribution)indexSelector.currentItem; } }

		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);
			onStationChanged(null,null);
			updateAlphaSprites();
		}

		public override void onDetached() {
			// TODO: update voxels correctly
			World.world.onAllVoxelUpdated();
		}

		private void onLengthChanged(object sender, EventArgs e) {
			// TODO: performance slow down when the length is very long. Check why.
			updateAlphaSprites();
		}


		private AlphaBlendSpriteSet alphaSprites;

		/// <summary>
		/// Re-builds an alpha-blending preview.
		/// </summary>
		private void updateAlphaSprites() {
			if(direction==null)		return;	// during the initialization, this method can be called in a wrong timing.
			if(alphaSprites!=null)
				alphaSprites.Dispose();


			Sprite[,,] alphas=null;

			switch( this.currentMode ) {
			case Mode.Station:
				// builds a new alpha blended preview
				alphas = selectedStation.sprites;
				break;

			case Mode.ThinPlatform:
				Sprite spr = ThinPlatform.getSprite( direction, false );

				// build sprite set
				// TODO: use the correct sprite
				if( direction==Direction.NORTH || direction==Direction.SOUTH ) {
					alphas = new Sprite[1,length,1];
					for( int i=0; i<length; i++ )
						alphas[0,i,0] = spr;
				} else {
					alphas = new Sprite[length,1,1];
					for( int i=0; i<length; i++ )
						alphas[i,0,0] = spr;
				}

				alphaSprites = new AlphaBlendSpriteSet(alphas);
				break;

			case Mode.FatPlatform:
				RailPattern rp = this.railPattern;


				// build sprite set
				if( direction==Direction.NORTH || direction==Direction.SOUTH ) {
					alphas = new Sprite[2,length,1];
					int j = direction==Direction.SOUTH?1:0;
					for( int i=0; i<length; i++ ) {
						alphas[j  ,i,0] = FatPlatform.getSprite(direction);
						alphas[j^1,i,0] = railPattern;
					}
				} else {
					alphas = new Sprite[length,2,1];
					int j = direction==Direction.WEST?1:0;
					for( int i=0; i<length; i++ ) {
						alphas[i,j  ,0] = FatPlatform.getSprite(direction);
						alphas[i,j^1,0] = railPattern;
					}
				}
				break;
			}

			alphaSprites = new AlphaBlendSpriteSet(alphas);
			World.world.onAllVoxelUpdated();	// completely redraw the window
		}

		private void onModeChanged(object sender, System.EventArgs e) {
			updateAlphaSprites();
		}
	}
}

