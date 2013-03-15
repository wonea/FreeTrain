using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.contributions.common;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.structs;
using freetrain.contributions.rail;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.util;
using org.kohsuke.directdraw;

namespace freetrain.controllers.structs
{
	/// <summary>
	/// Controller that allows the user to
	/// place/remove structures.
	/// </summary>
	public abstract class StructPlacementController : AbstractControllerImpl, MapOverlay, LocationDisambiguator
	{
		private System.Windows.Forms.ComboBox structType;
		private System.Windows.Forms.PictureBox preview;
		protected System.Windows.Forms.RadioButton buttonRemove;
		protected System.Windows.Forms.RadioButton buttonPlace;
		private System.ComponentModel.IContainer components = null;
		private freetrain.controls.IndexSelector indexSelector;

		private Bitmap previewBitmap;

		/// <param name="types">Array of all structure types available to users</param>
		protected StructPlacementController( StructureGroupGroup groupGroup ) {
			InitializeComponent();
			World.world.viewOptions.OnViewOptionChanged+=new OptionChangedHandler(updatePreview);

			// load station type list
			structType.DataSource = groupGroup;
			structType.DisplayMember="name";
			updateAfterResize(null,null);
		}

		protected override void Dispose( bool disposing ) {
			World.world.viewOptions.OnViewOptionChanged-=new OptionChangedHandler(updatePreview);
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
			
			if( previewBitmap!=null )
				previewBitmap.Dispose();
			
			if(alphaSprites!=null)
				alphaSprites.Dispose();
		}

		public override LocationDisambiguator disambiguator { get { return this; } }

		public abstract bool isSelectable( Location loc );

		#region Designer generated code
		/// <summary>
		/// Designer サポートに必要なメソッドです。コード エディタで
		/// このメソッドのコンテンツを変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.structType = new System.Windows.Forms.ComboBox();
			this.preview = new System.Windows.Forms.PictureBox();
			this.buttonRemove = new System.Windows.Forms.RadioButton();
			this.buttonPlace = new System.Windows.Forms.RadioButton();
			this.indexSelector = new freetrain.controls.IndexSelector();
			((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
			this.SuspendLayout();
			// 
			// structType
			// 
			this.structType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.structType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.structType.Location = new System.Drawing.Point(8, 9);
			this.structType.Name = "structType";
			this.structType.Size = new System.Drawing.Size(130, 21);
			this.structType.Sorted = true;
			this.structType.TabIndex = 2;
			this.structType.SelectedIndexChanged += new System.EventHandler(this.onGroupChanged);
			// 
			// preview
			// 
			this.preview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.preview.Location = new System.Drawing.Point(8, 69);
			this.preview.Name = "preview";
			this.preview.Size = new System.Drawing.Size(130, 87);
			this.preview.TabIndex = 1;
			this.preview.TabStop = false;
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemove.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonRemove.Location = new System.Drawing.Point(82, 165);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(56, 26);
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
			this.buttonPlace.Location = new System.Drawing.Point(8, 165);
			this.buttonPlace.Name = "buttonPlace";
			this.buttonPlace.Size = new System.Drawing.Size(56, 26);
			this.buttonPlace.TabIndex = 0;
			this.buttonPlace.TabStop = true;
			this.buttonPlace.Text = "Build";
			//! this.buttonPlace.Text = "設置";
			this.buttonPlace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// indexSelector
			// 
			this.indexSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.indexSelector.count = 10;
			this.indexSelector.current = 0;
			this.indexSelector.dataSource = null;
			this.indexSelector.Location = new System.Drawing.Point(8, 39);
			this.indexSelector.Name = "indexSelector";
			this.indexSelector.Size = new System.Drawing.Size(130, 22);
			this.indexSelector.TabIndex = 3;
			this.indexSelector.indexChanged += new System.EventHandler(this.onTypeChanged);
			// 
			// StructPlacementController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(146, 195);
			this.Controls.Add(this.indexSelector);
			this.Controls.Add(this.buttonPlace);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.structType);
			this.Controls.Add(this.preview);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "StructPlacementController";
			this.Text = "Building construction";
			//! this.Text = "建物の工事(仮)";
			this.Resize += new System.EventHandler(this.updateAfterResize);
			((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		
		protected virtual void updateAfterResize(object sender, System.EventArgs e){
			this.buttonPlace.Width = ((this.preview.Left + this.preview.Width) - this.buttonPlace.Left - 5) / 2;
			this.buttonRemove.Left = (this.buttonPlace.Left + this.buttonPlace.Width) + 10;
			this.buttonRemove.Width = this.preview.Width - this.buttonPlace.Width - 10;
			updatePreview();
		}



		protected bool isPlacing { get { return buttonPlace.Checked; } }
		


		private Location baseLoc = world.Location.UNPLACED;
		public override void onMouseMove(MapViewWindow view, Location loc, Point ab ) {
			World w = World.world;

			if(baseLoc!=loc) {
				// update the screen
				baseLoc = loc;
				// TODO: we need to correctly update the screen
				w.onAllVoxelUpdated();
			}
		}

		
		public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {
			if(!isPlacing)	return;
			
			if( Cube.createExclusive( baseLoc, alphaSprites.size ).contains(loc) )
				alphaSprites.getSprite( loc-baseLoc ).drawAlpha( canvas.surface, pt );
		}

		public void drawAfter( QuarterViewDrawer view, DrawContextEx surface ) {}


		/// <summary>
		/// Currently selected structure contribution.
		/// </summary>
		protected StructureContribution selectedType {
			get {
				return (StructureContribution)indexSelector.currentItem;
			}
		}

		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);
			updateAlphaSprites();
		}

		private AlphaBlendSpriteSet alphaSprites;

		/// <summary>
		/// Re-builds an alpha-blending preview.
		/// </summary>
		protected void updateAlphaSprites() {
			if(alphaSprites!=null)
				alphaSprites.Dispose();

			// builds a new alpha blended preview
			alphaSprites = createAlphaSprites();
		}

		/// <summary>
		/// Implemented by the derived class to provide a sprite set used
		/// to draw a preview of this structure on MapView.
		/// </summary>
		protected abstract AlphaBlendSpriteSet createAlphaSprites();

		private void onGroupChanged(object sender, System.EventArgs e) {
			indexSelector.dataSource = (StructureGroup)structType.SelectedItem;
			onTypeChanged(null,null);
		}


		/// <summary>
		/// Called when a selection of the structure has changed.
		/// </summary>
		protected virtual void onTypeChanged(object sender, System.EventArgs e) {
			updatePreview();
		}

		public override void updatePreview()
		{
			using( PreviewDrawer drawer = selectedType.createPreview(preview.Size) ) 
			{

				if( previewBitmap!=null )	previewBitmap.Dispose();
				preview.Image = previewBitmap = drawer.createBitmap();
			}

			updateAlphaSprites();
		}
	}
}

