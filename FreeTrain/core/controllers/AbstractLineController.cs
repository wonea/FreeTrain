using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.contributions.common;
using freetrain.contributions.rail;
using freetrain.framework.plugin;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;

namespace freetrain.controllers
{
	/// <summary>
	/// Controller that places/removes lines, such as roads or rail roads.
	/// </summary>
	public abstract class AbstractLineController : AbstractControllerImpl, MapOverlay
	{
		public AbstractLineController( LineContribution _type ) {
			InitializeComponent();
			this.contrib = _type;
			this.Text = type.name;
			updateAfterResize(null,null);
			//updatePreview();
		}

		public override void updatePreview()
		{
			if(this.picture.Image!=null)
				this.picture.Image.Dispose();
			Bitmap bmp = type.previewBitmap;
			this.picture.Image = bmp;
			this.picture.BackColor = bmp.GetPixel(0,bmp.Size.Height-1);
		}

		private LineContribution contrib;
		protected virtual LineContribution type{ get{ return contrib; } }

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			
			this.picture.Image.Dispose();	// I'm not sure if I really need to do this or not.

			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		protected System.Windows.Forms.RadioButton buttonRemove;
		protected System.Windows.Forms.RadioButton buttonPlace;
		protected System.Windows.Forms.PictureBox picture;
		protected System.Windows.Forms.ToolTip toolTip;		
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		protected virtual void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.buttonRemove = new System.Windows.Forms.RadioButton();
			this.buttonPlace = new System.Windows.Forms.RadioButton();
			this.picture = new System.Windows.Forms.PictureBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.picture)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemove.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonRemove.Location = new System.Drawing.Point(58, 112);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(62, 29);
			this.buttonRemove.TabIndex = 7;
			this.buttonRemove.Text = "Remove";
			//! this.buttonRemove.Text = "撤去";
			this.buttonRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.buttonRemove.CheckedChanged += new System.EventHandler(this.modeChanged);
			// 
			// buttonPlace
			// 
			this.buttonPlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonPlace.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonPlace.Checked = true;
			this.buttonPlace.Location = new System.Drawing.Point(8, 112);
			this.buttonPlace.Name = "buttonPlace";
			this.buttonPlace.Size = new System.Drawing.Size(42, 29);
			this.buttonPlace.TabIndex = 6;
			this.buttonPlace.TabStop = true;
			this.buttonPlace.Text = "Place";
			//! this.buttonPlace.Text = "敷設";
			this.buttonPlace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.buttonPlace.CheckedChanged += new System.EventHandler(this.modeChanged);
			// 
			// picture
			// 
			this.picture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.picture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.picture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.picture.Location = new System.Drawing.Point(8, 9);
			this.picture.Name = "picture";
			this.picture.Size = new System.Drawing.Size(110, 97);
			this.picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.picture.TabIndex = 4;
			this.picture.TabStop = false;
			// 
			// AbstractLineController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(128, 147);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonPlace);
			this.Controls.Add(this.picture);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "AbstractLineController";
			this.Text = "Road construction";
			//! this.Text = "道路工事";
			((System.ComponentModel.ISupportInitialize)(this.picture)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private bool isPlacing { get { return buttonPlace.Checked; } }




		/// <summary>
		/// The first location selected by the user.
		/// </summary>
		private Location anchor = UNPLACED;

		/// <summary>
		/// Current mouse position. Used only when anchor!=UNPLACED
		/// </summary>
		private Location currentPos = UNPLACED;

		private static Location UNPLACED = freetrain.world.Location.UNPLACED;

		/// <summary>
		/// Aligns the given location to the anchor so that
		/// the location will become straight.
		/// </summary>
		private Location align( Location loc ) {
			loc.z = anchor.z;

			if( type.directionMode == SpecialRailContribution.DirectionMode.FourWay )
				return loc.align4To(anchor);

			if( type.directionMode == SpecialRailContribution.DirectionMode.EightWay )
				return loc.align8To(anchor);

			Debug.Assert(false);
			return UNPLACED;
		}





		public override void onMouseMove( MapViewWindow view, Location loc, Point ab ) {
			if( anchor!=UNPLACED && isPlacing && currentPos!=loc ) {
				if( currentPos!=UNPLACED )
					World.world.onVoxelUpdated(Cube.createInclusive(anchor,currentPos));
				currentPos = align(loc);
				World.world.onVoxelUpdated(Cube.createInclusive(anchor,currentPos));
			}
		}

		public override void onClick( MapViewWindow source, Location loc, Point ab ) {
			if(anchor==UNPLACED) {
				anchor = loc;
				sameLevelDisambiguator = new SameLevelDisambiguator(anchor.z);
			} else {
				loc = align(loc);
				if(anchor!=loc) {
					if(isPlacing) {
						if( type.canBeBuilt( anchor, loc ) )
							// build new railroads.
							type.build( anchor, loc );
					} else
						// remove existing ones
						type.remove( anchor, loc );
					World.world.onVoxelUpdated(Cube.createInclusive(anchor,loc));
				}
				anchor = UNPLACED;
			}
		}
		public override void onRightClick( MapViewWindow source, Location loc, Point ab ) {
			if( anchor==UNPLACED )
				Close();	// cancel
			else {
				// cancel the anchor
				if( currentPos!=UNPLACED )
					World.world.onVoxelUpdated(Cube.createInclusive(anchor,currentPos));
				anchor = UNPLACED;
			}
		}

		public override LocationDisambiguator disambiguator {
			get {
				// the 2nd selection must go to the same height as the anchor.
				if(anchor==UNPLACED)	return RailRoadDisambiguator.theInstance;
				else					return sameLevelDisambiguator;
			}
		}
		private LocationDisambiguator sameLevelDisambiguator;


		private void modeChanged( object sender, EventArgs e ) {
			anchor = UNPLACED;
		}

		protected virtual void updateAfterResize(object sender, System.EventArgs e){
			this.buttonPlace.Left = this.picture.Left;
			this.buttonPlace.Width = ((this.picture.Left + this.picture.Width)) / 2;
			this.buttonRemove.Left = (this.buttonPlace.Left + this.buttonPlace.Width);
			this.buttonRemove.Width = this.buttonPlace.Width;
			updatePreview();
		}




		private bool inBetween( Location loc, Location lhs, Location rhs ) {
			if( !loc.inBetween(lhs,rhs) )	return false;

			if(( lhs.x==rhs.x && rhs.x==loc.x )
			|| ( lhs.y==rhs.y && rhs.y==loc.y ) )	return true;

			if( Math.Abs(loc.x-lhs.x)==Math.Abs(loc.y-lhs.y) )	return true;

			return false;
		}


		public void drawBefore( QuarterViewDrawer view, DrawContextEx canvas ) {
			if( anchor!=UNPLACED && isPlacing )
				canvas.tag = type.canBeBuilt(anchor,currentPos);
		}

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {
			object tag = canvas.tag;

			if( tag!=null && (bool)tag && inBetween( loc, anchor, currentPos ) ) {
				Direction d = anchor.getDirectionTo(currentPos);
				draw( d, canvas, pt );
			}
		}

		public void drawAfter( QuarterViewDrawer view, DrawContextEx canvas ) {
		}

		/// <summary>
		/// Draw the preview on the given point.
		/// </summary>
		protected abstract void draw( Direction d, DrawContextEx canvs, Point pt );
	}
}
