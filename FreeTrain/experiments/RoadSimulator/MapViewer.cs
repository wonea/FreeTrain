using System;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace RoadSimulator
{
	public enum EditMode : int {None,Erase,Road,Barrier,StationV,StationH};
	/// <summary>
	/// MapViewer の概要の説明です。
	/// </summary>
	public class MapViewer : System.Windows.Forms.PictureBox
	{
		enum TypeTable : int {Empty,Station,Barrier,Road,Struct};
		static Color[] colTable = new Color[]{
		Color.Blue,Color.Red,Color.DarkGreen,Color.White,Color.Green	
											 };
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;
		private World world;
		private Pen[] pens;	
		public EditMode mode = EditMode.None;
		public bool boxMode = false;

		public MapViewer(){
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();
		}

		public MapViewer(World w)
		{
			setWorld(w);
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();
			// TODO: InitForm を呼び出しの後に初期化処理を追加します。
		}

		public void setWorld(World w)
		{
			world = w;
			Bitmap bm = new Bitmap(w.xWidth,w.yWidth);
			this.Image = bm;
			updateMap(new Rectangle(0,0,w.xWidth,w.yWidth));
			w.onVoxelUpdated+=new VoxelAreaListener(updateMapView);			
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// MapViewer
			// 
			this.CausesValidation = false;
			this.Name = "MapViewer";
			this.Size = new System.Drawing.Size(288, 296);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MapViewer_MouseUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MapViewer_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MapViewer_MouseDown);

		}
		#endregion

		public void updateMapView(Rectangle bounds) 
		{
			updateMap(bounds);
			Invalidate(bounds);		
		}

		protected void updateMap(Rectangle bounds)
		{
			Rectangle whole = new Rectangle(0,0,world.xWidth,world.yWidth);
			if( !bounds.IsEmpty) 
				whole = Rectangle.Intersect(bounds, whole);
			Bitmap bm = (Bitmap)this.Image;
			for( int ix=whole.Left; ix<whole.Right; ix++ )
				for( int iy=whole.Top; iy<whole.Bottom; iy++ )
					bm.SetPixel(ix,iy, getColor(ix,iy));
		}

		#region color selector functions
		private Color getColor(int x, int y)
		{
			Voxel v = world[x,y];
			if(v==null)
				return colTable[(int)TypeTable.Empty];
			if(v.road!=null)
				return darken(colTable[(int)TypeTable.Road],v.road.getMinLevel()); 
			if(v.structure!=null)
			{
				if(v.structure is Barrier)
				{
					Barrier b = (Barrier)v.structure;
					int c = (int)TypeTable.Empty;
					switch(b.type)
					{
						case BarrierType.Station:
							c = (int)TypeTable.Station;
							break;
						case BarrierType.Barrier:
							c = (int)TypeTable.Barrier;
							break;
						case BarrierType.Building:
							c = (int)TypeTable.Struct;
							break;
					}					
					return colTable[c];
				}
				else
					return colTable[(int)TypeTable.Barrier];
			}
			return colTable[(int)TypeTable.Empty];
		}

		private Color darken(Color source, int level)
		{
			if( level == 0) return source;
			else 
			{
				return ControlPaint.Dark(source,level/10.0f);
			}
		}

		#endregion
		private Point start;
		private Point start0;
		private Point end;

		#region mouse event handlers
		private void MapViewer_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( e.Button != MouseButtons.Left ) return;
			start0 = new Point(e.X,e.Y);
			start = PointToScreen( start0 );			
			end = start;
			//Capture = true;
		}

		private void MapViewer_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( e.Button != MouseButtons.Left ) return;
			Point now = PointToScreen(new Point(e.X,e.Y));
			//Invalidate(new Rectangle(start, new Size(end.X-start.X,end.Y-start.Y)));
			switch( mode )
			{
				case EditMode.Road:
					resetReversed();
					if( boxMode )
					{
						Rectangle r;
						r = new Rectangle(start, new Size(now.X-start.X,now.Y-start.Y));
						ControlPaint.DrawReversibleFrame(r,Color.Black,FrameStyle.Dashed);
					}
					else 
					{
						int w = Math.Abs(now.X-start.X);
						int h = Math.Abs(now.Y-start.Y);
						if( w>h )
							now.Y = start.Y;
						else
							now.X = start.X;
						ControlPaint.DrawReversibleLine(start,now,Color.Black);
					}
					break;
				case EditMode.Barrier:
				case EditMode.Erase:
					resetReversed();
					if( boxMode )
					{
						Rectangle r;
						r = new Rectangle(start, new Size(now.X-start.X,now.Y-start.Y));
						ControlPaint.DrawReversibleFrame(r,Color.Black,FrameStyle.Dashed);
					}
					else
					{
						ControlPaint.DrawReversibleLine(start,now,Color.Black);
					}
					break;
			}		
			end  = now;
		}

		private void MapViewer_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( e.Button != MouseButtons.Left ) return;		
			Point p0 = start0;
			Point p1 = new Point(e.X,e.Y);
			switch( mode )
			{
				case EditMode.StationH:
					world.setStationH(Configure.stationLevel,e.X,e.Y);
					break;
				case EditMode.StationV:
					world.setStationV(Configure.stationLevel,e.X,e.Y);
					break;
				case EditMode.Barrier:
					resetReversed();
					if( boxMode )
						world.barrierBox(p0,p1);
					else
						world.barrierLine(p0,p1);
					break;
				case EditMode.Erase:
					resetReversed();
					if( boxMode )
						world.eraseBox(p0,p1);
					else
						world.eraseLine(p0,p1);
					break;
				case EditMode.Road:
					resetReversed();
					int lv = Configure.stationLevel;
					if( boxMode )
					{
						world.buildRoadRect(lv,new Rectangle(p0, new Size(p1.X-p0.X,p1.Y-p0.Y)));
					}
					else 
					{
						int w = Math.Abs(p1.X-p0.X);
						int h = Math.Abs(p1.Y-p0.Y);
						if( w>h ) 
							world.buildRoadH(lv,p0.X,p1.X,p0.Y);
						else
							world.buildRoadV(lv,p0.X,p1.Y,p0.Y);
					}
					break;
			}
//			updateMap(r);
//			Invalidate(r);		
		}
		#endregion

		private void resetReversed()
		{
			if( boxMode )
			{
				Rectangle r;
				r = new Rectangle(start, new Size(end.X-start.X,end.Y-start.Y));
				ControlPaint.DrawReversibleFrame(r,Color.Black,FrameStyle.Dashed);
			}
			else
			{
				ControlPaint.DrawReversibleLine(start,end,Color.Black);
			}
		}
	}
}
