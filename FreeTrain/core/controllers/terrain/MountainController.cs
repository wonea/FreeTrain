using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.views.map;
using freetrain.util;
using freetrain.world;
using freetrain.world.terrain;
using freetrain.framework;

namespace freetrain.controllers.terrain
{
	/// <summary>
	/// Manipulates mountains
	/// </summary>
	public class MountainController : AbstractControllerImpl
	{
		#region Singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new MountainController();
			theInstance.Show();
			theInstance.Activate();
		}

		private Label label1;
		private GroupBox groupBox1;
		private freetrain.controls.IndexSelector selSize;

		private static MountainController theInstance;

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			base.OnClosing(e);
			theInstance = null;
		}
		#endregion
		private Bitmap previewBitmap;
		public MountainController() {
			InitializeComponent();
			previewBitmap = ResourceUtil.loadSystemBitmap("terrain.bmp");
			preview.Image = previewBitmap;
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.PictureBox preview;
		private System.Windows.Forms.RadioButton buttonUp;
		private System.Windows.Forms.RadioButton buttonDown;
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonUp = new System.Windows.Forms.RadioButton();
			this.buttonDown = new System.Windows.Forms.RadioButton();
			this.preview = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.selSize = new freetrain.controls.IndexSelector();
			((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonUp
			// 
			this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonUp.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonUp.Checked = true;
			this.buttonUp.Location = new System.Drawing.Point(4, 209);
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.Size = new System.Drawing.Size(56, 26);
			this.buttonUp.TabIndex = 2;
			this.buttonUp.TabStop = true;
			this.buttonUp.Text = "Raise";
			//! this.buttonUp.Text = "隆起";
			this.buttonUp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonDown
			// 
			this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDown.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonDown.Location = new System.Drawing.Point(59, 209);
			this.buttonDown.Name = "buttonDown";
			this.buttonDown.Size = new System.Drawing.Size(56, 26);
			this.buttonDown.TabIndex = 4;
			this.buttonDown.Text = "Lower";
			//! this.buttonDown.Text = "掘削";
			this.buttonDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// preview
			// 
			this.preview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.preview.InitialImage = null;
			this.preview.Location = new System.Drawing.Point(4, 5);
			this.preview.Name = "preview";
			this.preview.Size = new System.Drawing.Size(111, 81);
			this.preview.TabIndex = 3;
			this.preview.TabStop = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Location = new System.Drawing.Point(4, 93);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(111, 68);
			this.label1.TabIndex = 6;
			this.label1.Text = "Press SHIFT and move mouse to quickly modify terrain.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.selSize);
			this.groupBox1.Location = new System.Drawing.Point(4, 165);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(111, 38);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Target Size";
			this.groupBox1.Enabled = false;
			// 
			// selSize
			// 
			this.selSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.selSize.count = 10;
			this.selSize.current = 0;
			this.selSize.dataSource = null;
			this.selSize.Location = new System.Drawing.Point(6, 14);
			this.selSize.Name = "selSize";
			this.selSize.Size = new System.Drawing.Size(95, 17);
			this.selSize.TabIndex = 6;
			// 
			// MountainController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(119, 239);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonUp);
			this.Controls.Add(this.buttonDown);
			this.Controls.Add(this.preview);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "MountainController";
			this.Text = "Modify Terrain";
			//! this.Text = "地形操作";
			((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public override LocationDisambiguator disambiguator { get { return GroundDisambiguator.theInstance; } }


		private bool isRaising { get { return buttonUp.Checked ^ Keyboard.isControlKeyPressed; } }
		


		public override void onMouseMove(MapViewWindow view, Location loc, Point ab) {
			if( Keyboard.isShiftKeyPressed ) {
				loc = selectVoxel(view,loc,ab);
				raiseLowerLand(loc);
			}
		}

		public override void onClick(MapViewWindow view, Location loc, Point ab) {
			loc = selectVoxel(view,loc,ab);
			raiseLowerLand(loc);
		}

		private void raiseLowerLand(Location loc) {
			//int origLoc = loc.y;
			//for (int sizeX = 0; sizeX < (selSize.current + 1); sizeX++)
			//{
			//	for (int sizeY = 0; sizeY < (selSize.current + 1); sizeY++)
			//	{
			//		loc.y = origLoc + sizeY;
					if (isRaising) raise(loc);
					else lower(loc);
			//	}
			//	loc.x++;
			//}
		}

		/// <summary>
		/// Selects the south-western voxel of the point selected by the mouse.
		/// The loc parameter and the ab parameter should point to the same location.
		/// </summary>
		private Location selectVoxel( MapViewWindow view, Location loc, Point ab ) {
			// top-left corner of the selected location
			Point vxl = World.world.fromXYZToAB(loc);
			
			Point offset = new Point( ab.X-vxl.X, ab.Y-vxl.Y );

			if( offset.X< 8 )	loc.x--;
			else
			if( offset.X>=24)	loc.y++;
			else {
				MountainVoxel mv = MountainVoxel.get(loc);
				int h0 = (mv!=null)?(int)mv.getHeight(Direction.NORTHEAST):0;
				int h2 = (mv!=null)?(int)mv.getHeight(Direction.SOUTHWEST):0;

				if( offset.Y >= (16-(h0+h2)*4)/2 ) {
					loc.x--; loc.y++;
				}
			}

			return loc;
		}

		/// <summary>
		/// Checks the height agreement of the four corners adjacent to
		/// the north-eastern corner of the given location.
		/// </summary>
		/// <param name="loc"></param>
		/// <returns></returns>
		private bool isFourAdjacentCornersMatched( Location loc ) {
			Direction d = Direction.NORTH;

			for( int i=0; i<4; i++ ) {
				if( !MountainVoxel.isCornerMatched(loc,d.left) )
					return false;

				loc += d;
				d = d.right90;
			}

			return true;
		}

		// clean it up by using MountainVoxel.isCornerMatched

		/// <summary>
		/// Return true iff the north-eastern corner of the given location
		/// can be raised by a quarter height unit.
		/// </summary>
		private bool canBeRaised( Location loc ) {
			World w = World.world;

			if( !isFourAdjacentCornersMatched(loc) )	return false;


			Voxel baseVoxel = w[loc];
			int glevel = w.getGroundLevel(loc);

			if( loc.z != glevel )	return false;	//mountain can be placed only at the ground level

			// true if this ground level is too close to the roof.
			bool nearRoof = ( glevel==World.world.size.z-1 );

			for( int x=0; x<=1; x++ ) {
				for( int y=-1; y<=0; y++ ) {
					Location l = new Location( loc.x+x, loc.y+y, loc.z );
					Direction d = Direction.get( 1-x*2, -y*2-1 );	// corner to modify

					if( w.isOutsideWorld(l) )
						continue;	// it's OK if it's beyond the border
					
					Voxel v = w[l];

					if( glevel != w.getGroundLevel(l) )
						return false;	// different ground level

					if( v==null )
						continue;	// this voxel is unoccupied.

					if( v is MountainVoxel ) {
						int h = ((MountainVoxel)v).getHeight(d);
						if( h==4 )
							return false;	// corner saturated.
						if( nearRoof && h==3 )
							return false;	// too close to the roof

						continue;	// otherwise OK
					}

					return false;	// occupied for another purpose
				}
			}

			if( World.world.isOutsideWorld(loc) )
				return false;

			return true;
		}

		/// <summary>
		/// Raises the north-eastern corner of the specified voxel
		/// </summary>
		/// <returns>false if the operation was unsuccessful.</returns>
		private bool raise( Location loc ) {
			World w = World.world;

			// make sure that four surrounding voxels can be raised,
			// and the ground levels of them are the same
			if(!canBeRaised(loc))
				return false;
			
			// then actually change the terrain
			for( int x=0; x<=1; x++ ) {
				for( int y=-1; y<=0; y++ ) {
					Location l = new Location( loc.x+x, loc.y+y, loc.z );

					Voxel vx = w[l];
					if( vx is World.OutOfWorldVoxel )
						continue;	// this is beyond the border

					MountainVoxel v = vx as MountainVoxel;
					
					Direction d = Direction.get( 1-x*2, -y*2-1 );	// corner to modify

					if( v==null )
						v = new MountainVoxel( l, 0,0,0,0 );
					
					// raise the corner
					v.setHeight( d, v.getHeight(d)+1 );
					
					if( v.isSaturated ) {
						// if the voxel is saturated, raise the ground level
						w.raiseGround(l);
						w.remove(l);	// remove this voxel
					}
				}
			}
			
			return true;
		}


		// clean it up by using MountainVoxel.isCornerMatched
		private bool canBeLowered( ref Location loc ) {
			World world = World.world;

			if( !isFourAdjacentCornersMatched(loc) )	return false;

			MountainVoxel mvBase = MountainVoxel.get(loc);
			if( mvBase!=null ) {
				if( mvBase.getHeight(Direction.NORTHEAST)==0 )
					return false;	// other corners need to be lowered first.
			} else {
				int glevel = world.getGroundLevel(loc);
				if( glevel!=loc.z && glevel!=loc.z-1 )
					return false;
				if( loc.z==0 )
					return false;	// can't dig deeper
				loc.z--;
			}

			// check other voxels
			for( int x=0; x<=1; x++ ) {
				for( int y=-1; y<=0; y++ ) {
					Location l = new Location( loc.x+x, loc.y+y, loc.z );

					if( MountainVoxel.get(l)!=null )
						continue;	// if it's mountain, OK.
					
					// otherwise, make sure that nothing is on it.
					if( World.world[ l.x, l.y, l.z+1 ]!=null )
						return false;
					// and nothing is under it
					if( World.world[ l.x, l.y, l.z ]!=null )
						return false;
				}
			}

			if( World.world.isOutsideWorld(loc) )
				return false;

			return true;
		}

		/// <summary>
		/// Lowers the north-eastern corner of the specified voxel.
		/// </summary>
		/// <returns>false if the operation was unsuccessful.</returns>
		private bool lower( Location loc ) {

			World world = World.world;

			if(!canBeLowered(ref loc))	return false;


			// then actually change the terrain
			for( int x=0; x<=1; x++ ) {
				for( int y=-1; y<=0; y++ ) {
					Location l = new Location( loc.x+x, loc.y+y, loc.z );
					Direction d = Direction.get( 1-x*2, -y*2-1 );	// corner to modify

					MountainVoxel mv = MountainVoxel.get(l);
					if( mv==null ) {
						World.world.lowerGround(l);
						mv = new MountainVoxel( l, 4,4,4,4 );
					}
					
					mv.setHeight( d, mv.getHeight(d)-1 );

					if( mv.isFlattened )	// completely flattened
						world.remove(mv);
				}
			}

			return true;
		}
	}
}
