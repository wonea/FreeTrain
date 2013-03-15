using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Windows.Forms;

namespace freetrain.finance.stock
{
	/// <summary>
	/// ChartControl の概要の説明です。
	/// </summary>
	public delegate void DataUpdateListener(int idxStart, int idxEnd);
	public enum XAxisStyle{ NORMAL, YEARLY, MONTHLY, WEEKLY, DAILY, HOURLY }
	public enum YAxisStyle{ AUTORANGE, AUTOSCALE, MANUAL }
	public enum ChartStyle{ COLUMN, LINE, AREA, STOCKCANDLE }

	public interface IChartDataTable 
	{
		DataUpdateListener onUpdate{ get; set; }
		DataRange getDataRange(XAxisStyle scale);
		//long this[XAxisStyle scale, int index]{ get; }
	}

	public interface IStockDataTable : IChartDataTable 
	{
		IStockPrice this[XAxisStyle scale, int index]{ get; }
	}

	public interface ILongDataTable : IChartDataTable 
	{
		long this[XAxisStyle scale, int index]{ get; }
	}

	public interface IStockPrice 
	{
		int low { get; }
		int high { get; }
		int start { get; }
		int end { get; }
	}

	[Serializable]
	public struct DataRange 
	{
		public long max;
		public long min;
		public DataRange(long _min, long _max)
		{
			max = _max;
			min = _min;
		}

		public bool isMax( long _max )
		{
			if( _max > max ) 
			{
				max = _max;
				return true;
			}
			return false;
		}
		public bool isMin( long _min )
		{
			if( _min < min ) 
			{
				min = _min;
				return true;
			}
			return false;
		}

	}
	
	/// <summary>
	/// summary of the ChartControl
	/// </summary>
	public class ChartControl : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;
		public YAxisStyle ScaleTypeY;
		public XAxisStyle ScaleTypeX;
		// the data tables
		private ArrayList series = new ArrayList();
		public PlotArea area;

		//public Color background = Color.WhiteSmoke;

		public ChartControl()
		{
			// この呼び出しは、Windows.Forms フォーム デザイナで必要です。
			InitializeComponent();
			area = new PlotArea(this,30,15);
			onSizeChanged(this,null);

			// sample data for debug.
			//addDataSource(new DummyData(), Color.Red, ChartStyle.COLUMN );
		}

		public void addDataSource( IChartDataTable table, Color color, ChartStyle style ) 
		{
			Debug.Assert( style != ChartStyle.STOCKCANDLE );
			series.Add( new ChartData(this,table,color,style) );
		}

		public void addDataSource( IStockDataTable table, Color color ) 
		{
			series.Add( new ChartData(this,table,color,ChartStyle.STOCKCANDLE) );
		}

		public bool removeDataSourceAt( int index ) 
		{
			if( index < series.Count ) 
			{
				((ChartData)series[index]).release();					
				series.RemoveAt(index);
				return true;
			}
			else
				return false;
		}

		public void removeDataSource( IChartDataTable table ) 
		{
			IEnumerator e = series.GetEnumerator();
			while( e.MoveNext() ) 
			{
				ChartData tmp = (ChartData)e.Current;
				if( table == tmp.table ) 
				{
					tmp.release();					
					series.Remove( tmp );
					return;
				}
			}
		}

		// for stock price data only.
		public bool replaceTable( int index, IStockDataTable table )
		{
			ChartData tmp = (ChartData)series[index];
			if( tmp.style != ChartStyle.STOCKCANDLE )
				return false;
			tmp.replace(this,table);
			return true;
		}

		// for all data except for stock price.
		public bool replaceTable( int index, IChartDataTable table )
		{
			ChartData tmp = (ChartData)series[index];
			if( tmp.style == ChartStyle.STOCKCANDLE )
				return false;
			tmp.replace(this,table);
			return true;
		}

		private void onUpdateCalled( int start, int end ) 
		{
			calcRange();
			Invalidate();
		}

		#region methods for auto scale / range calculation.

		public void calcRange() 
		{
			if( series.Count < 1 ) return;
			switch( ScaleTypeY ) {
				case YAxisStyle.MANUAL :
					break;
				case YAxisStyle.AUTORANGE :
					adjustRange();
					break;
				case YAxisStyle.AUTOSCALE :
					adjustScale();
					break;
			}
		}

		private void adjustScale() 
		{
			long max = Math.Max(1,getMaximum());
			long min = getMinimum();
			// maintain current scale if possible.
			if( max < area.YTopRange && max*4 > area.YTopRange/4 ) 
				if( min < area.YTopRange*3/4 && min > area.YBottomRange ) 
					return;
			long v = max;
			while( v >= 10 )
				v /= 10;
			long n = max/v;
			Debug.WriteLine("rescale"+v+":"+n);
			if( v<1 ) 
			{
				if( n>10 )
					area.setYRange(0,n*15/10); //grid = 0.5,0.1,1.5
				else
					area.setYRange(0,n*3); //grid = 1,2,3
			}
			else if( v<3 ) area.setYRange(0,n*3); //grid = 1,2,3
			else if( v<6 ) area.setYRange(0,n*6); //grid = 2,4,6
			else area.setYRange(0,n*15); //grid = 5,10,15
		}

		private void adjustRange() 
		{
			long max = getMaximum();
			long min = getMinimum();
			// maintain current range if possible.
			if( max < area.YTopRange && min > area.YBottomRange ) 
				return;

			long w = max-min;
			long n = 10;
			while( w > n )
				n *= 10;
			n/=10;
			long v = max/n;
			long grid;
			if( v < 2 ) grid = n; //grid = 1,2,3
			else if( v<4) grid = n*2; //grid = 2,4,6
			else  grid = n*5; //grid = 5,10,15
			min = min/grid*(grid-1);
			max = min+n*3;
			area.setYRange(min,max);
		}

		private long getMinimum() 
		{
			long m = ((ChartData)series[0]).table.getDataRange(ScaleTypeX).min;
			for( int i = 1; i<series.Count; i++ ) {
				long n = ((ChartData)series[i]).table.getDataRange(ScaleTypeX).min;
				if( m > n ) m = n;
			}
			return m;
		}

		private long getMaximum() 
		{
			long m = ((ChartData)series[0]).table.getDataRange(ScaleTypeX).max;
			for( int i = 1; i<series.Count; i++ ) 
			{
				long n = ((ChartData)series[i]).table.getDataRange(ScaleTypeX).max;
				if( m < n ) m = n;
			}
			return m;
		}

		#endregion

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
			// ChartControl
			// 
			this.Name = "ChartControl";
			this.Size = new System.Drawing.Size(120, 166);
			this.SizeChanged += new System.EventHandler(this.onSizeChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.onPaint);

		}
	#endregion

		private void onPaint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			//e.Graphics.Clear(Color.LemonChiffon);
			area.drawGrid(e.Graphics);
			for( int i = 0; i<series.Count; i++ )
				drawData(e.Graphics,(ChartData)series[i]);
			using( SolidBrush brush = new SolidBrush(Color.Black))
			{
				using( Font fnt = new Font("MS UI Gothic",7)) 
				//! using( Font fnt = new Font("UI ゴシック",7)) 
				{
					area.drawXAxis(e.Graphics,fnt,brush);
					area.drawYAxis(e.Graphics,fnt,brush);
				}
			}
		}

		private void drawData(Graphics g, ChartData cd) 
		{
			drawData(g,cd,0,area.XGrids);
		}

		private void drawData(Graphics g, ChartData cd, int start, int end) 
		{
			if( cd.style == ChartStyle.STOCKCANDLE ) 
			{
				IStockDataTable table = (IStockDataTable)cd.table;
				int b = area.XPixelsPerGrids/2;
				using (SolidBrush brush = new SolidBrush(cd.color)) 
				{
					using( Pen pen = new Pen(cd.color )) 
					{
						for( int x = start; x<= end; x++ ) 
						{							
							IStockPrice price = table[ScaleTypeX,x];

							int h = price.end - price.start;
							int x2 = area.xpics(x)+b;
							if( h > 0 )	
							{
								g.DrawLine(pen,
									x2,area.ypics(price.high),
									x2,area.ypics(price.end));
								g.DrawLine(pen,
									x2,area.ypics(price.start),
									x2,area.ypics(price.low));
								g.DrawRectangle(pen,area.rect(x,price.end,1,h));
							}
							else 
							{
								g.DrawLine(pen,
									x2,area.ypics(price.low),
									x2,area.ypics(price.high));
								g.FillRectangle(brush,area.rect(x,price.start,1,-h));
							}		
						}// for(x)
					}// using (pen)
				}// using (brush)
			}
			else 
			{
				ILongDataTable table = (ILongDataTable)cd.table;
				switch (cd.style) 
				{
					case ChartStyle.COLUMN:
						using (Pen pen = new Pen(cd.color))
						{
							Rectangle[] colm = new Rectangle[end-start+1];
							int i = 0;
							for( int x = start; x<= end; x++ ) 
							{
								Rectangle r = area.rect(x,0,1,table[ScaleTypeX,x]);
								r.Inflate(-1,0);
								colm[i++] = r;
							}
							g.DrawRectangles(pen,colm);
							break;
						}
					case ChartStyle.AREA:
						using (SolidBrush brush = new SolidBrush(cd.color) ) 
						{
							Point[] poly = new Point[end-start+3];
							int i = 0;
							for( int x = start; x<= end; x++ )
								poly[i++] = area.point(x,table[ScaleTypeX,x]);
							poly[i++] = area.point(end,0);
							poly[i++] = area.point(start,0);								
							g.FillPolygon(brush,poly);
							break;
						}
					case ChartStyle.LINE:
						using (Pen pen = new Pen(cd.color))
						{
							Point[] poly = new Point[end-start+1];
							int i = 0;
							for( int x = start; x<= end; x++ )
								poly[i++] = area.point(x,table[ScaleTypeX,x]);
							g.DrawLines(pen,poly);
							break;
						}
				}// switch
			}// else
		}

		private void onSizeChanged(object sender, System.EventArgs e) 
		{
			area.resize(this.Size);
			this.Invalidate();
		}

		#region dummy data class for debug
		private class DummyData : ILongDataTable
		{
			private long[] array = new long[]{ 5,4,2,8,11,16,12,14,15,15,10,8,7,11,13,15,19,28,33,46,64,79,50,40,25,32,27,31,8,10,10};
			public DataUpdateListener onUpdate{ get{return null;} set{} }
			public DataRange getDataRange(XAxisStyle scale){ return new DataRange(2,79); } 
			public long this[XAxisStyle scale, int index]{ get { return array[index]; } }
		}
		#endregion

		/// <summary>
		/// chart data struct (for a sequence of data)
		/// </summary>
		protected struct ChartData
		{
			public IChartDataTable table;
			public Color color;
			public ChartStyle style;
			public DataUpdateListener listener;

			public ChartData(ChartControl chart, IChartDataTable _table, Color _color, ChartStyle _style )
			{
				Debug.Assert(_table != null );
				table = _table;
				color = _color;
				style = _style;
				listener = new DataUpdateListener(chart.onUpdateCalled);
				table.onUpdate += listener;
			}

			public void replace(ChartControl chart, IChartDataTable _table )
			{
				release();
				table = _table;
				listener = new DataUpdateListener(chart.onUpdateCalled);
				table.onUpdate += listener;
			}

			public void release()
			{
				table.onUpdate -= listener;
			}			
		}
	}

	/// <summary>
	/// The area data are plotted
	/// </summary>
	public class PlotArea
	{
		public readonly int MarginLeft;
		public readonly int MarginBottom;
		public int Left;
		public int Right;
		public int Top { get { return 0; } }
		public int Bottom;
		public int Width { get { return Right-Left; } }
		public int Height { get { return Bottom; } }

		// the data of the table is drawed right to left
		public bool startRight = true;
		// tick (=array index) counts of the axis
		public int XMajorGrids = 3;
		public int YMajorGrids = 3;
		public int XMinorGrids = 10;
		public int YMinorGrids = 10;
		public int XGrids { get { return XMajorGrids*XMinorGrids; } }
		public int YGrids { get { return YMajorGrids*YMinorGrids; } }
		public int XPixelsPerGrids { get { return _XPics; } }
		public int YPixelsPerGrids { get { return _YPics; } }
		private int _XPics;
		private int _YPics;
		private int _XPicsG;
		private int _YPicsG;
		private long _YScaleUnit = 1;
		private long _YBottomRange = 0;
		private long _YTopRange = 30;
		public long YTopRange { get { return _YTopRange; } }
		public long YBottomRange { get { return _YTopRange; } }
		public long YRangeWidth { get { return _YTopRange-_YBottomRange; } }
		public double YLabelBase = 1;

		public Pen penMajorG = Pens.DarkGray;
		public Pen penMinorG = Pens.Silver;

		public PlotArea(UserControl ctrl, int marginLeft, int marginBottom)
		{
			MarginBottom = marginBottom;
			MarginLeft = marginLeft;
			resize(ctrl.ClientSize);
		}

		public void setGrid(int x_major, int y_major, int x_minor, int y_minor )
		{
			XMajorGrids = x_major;
			XMinorGrids = x_minor;
			YMajorGrids = y_major;
			YMinorGrids = y_minor;
		}

		public void resize(Size sz) 
		{
			//Top = 0;
			Right = sz.Width;
			int width = sz.Width-MarginLeft;
			int height = sz.Height-MarginBottom;
			_XPics = width/XGrids;
			_YPics = height/YGrids;
			_XPicsG = _XPics*XMinorGrids;
			_YPicsG = _YPics*YMinorGrids;
			width = _XPics*XGrids;
			height = _YPics*YGrids;
			Bottom = height;
			Left = Right-width;
		}

		public void setYRange( long min, long max )	
		{
			_YBottomRange = min;
			_YTopRange = max;
			_YScaleUnit = YRangeWidth/YGrids;
		}

		public int xpics(long x) 
		{
			if( startRight )
				return (int)(Right-x*_XPics);
			else
				return (int)(x*_XPics+Left);
		}

		public int ypics(long y) 
		{
			y = (y-_YBottomRange)*Height/YRangeWidth;
			return Bottom-(int)y;
		}

		public int hpics(long h) 
		{
			return (int)(h*Height/YRangeWidth);			
		}

		public Rectangle rect( long x, long y, long w, long h ) 
		{
			return new Rectangle(xpics(x),ypics(y),(int)w*_XPics,hpics(h)+1);
		}

		public Point point(long x, long y) 
		{
			return new Point(xpics(x),ypics(y));
		}

		public Size mimimumSize() 
		{
			return new Size(MarginLeft+XGrids*3,MarginBottom*YGrids*3);
		}

		public void drawGrid(Graphics g) 
		{
			int xmx,xmn;
			int ymx,ymn;

			//minor grid
			if( _XPics > 0 && _YPics > 0) 
			{
				xmx = ((int)g.ClipBounds.Right)/_XPics*_XPics;
				if( xmx > Right ) xmx = Right;
				xmn = (((int)g.ClipBounds.Left)/_XPics+1)*_XPics;
				if( xmn < Left ) xmn = Left;
				ymx = ((int)g.ClipBounds.Bottom)/_YPics*_YPics;
				if( ymx > Bottom ) ymx = Bottom;
				ymn = (((int)g.ClipBounds.Top)/_YPics+1)*_YPics;
				if( ymn < Top ) ymn = Top;

				for(int x=xmn; x<=xmx; x+=_XPics)
					g.DrawLine(penMinorG,x,ymn,x,ymx);
				for(int y=ymn; y<=ymx; y+=_YPics)
					g.DrawLine(penMinorG,xmn,y,xmx,y);
			}
			//major grid
			if( _XPicsG > 0 &&_YPicsG > 0 )
			{
				xmx = ((int)g.ClipBounds.Right)/_XPicsG*_XPicsG;
				if( xmx > Right ) xmx = Right;
				xmn = (((int)g.ClipBounds.Left)/_XPicsG+1)*_XPicsG;
				if( xmn < Left ) xmn = Left;
				ymx = ((int)g.ClipBounds.Bottom)/_YPicsG*_YPicsG;
				if( ymx > Bottom ) ymx = Bottom;
				ymn = (((int)g.ClipBounds.Top)/_YPicsG+1)*_YPicsG;
				if( ymn < Top ) ymn = Top;

				for(int x=xmn; x<=xmx+_XPicsG; x+=_XPicsG)
					g.DrawLine(penMajorG,x,ymn,x,ymx);
				for(int y=ymn; y<=ymx+_YPicsG; y+=_YPicsG)
					g.DrawLine(penMajorG,xmn,y,xmx,y);
			}
		}

		public void drawXAxis(Graphics g, Font fnt, Brush brush) 
		{
			float y = 4+Bottom;
			if( g.ClipBounds.Bottom < y ) return;

			if( startRight ) 
			{
				g.DrawString("0",fnt,brush,Right-fnt.Size,y);
				for( int x=0; x<=20; x+=10 ) 
					g.DrawString(x.ToString(),fnt,brush,Right-x*_XPics-fnt.Size,y);
				g.DrawString("30",fnt,brush,Left,y);
			}
			else 
			{
				g.DrawString("0",fnt,brush,Left,y);
				for( int x=10; x<=20; x+=10 ) 
					g.DrawString(x.ToString(),fnt,brush,Left+x*_XPics-fnt.Size,y);
				g.DrawString("30",fnt,brush,Right-fnt.Size*2,y);
			}
		}

		public void drawYAxis(Graphics g, Font fnt, Brush brush) 
		{
			if( g.ClipBounds.Left > Left ) return;
			RectangleF rect = new RectangleF(0,0,Left-4,fnt.Size*2);
			StringFormat fmt = new StringFormat();
			fmt.Alignment = StringAlignment.Far;
			long step = YRangeWidth/3;
			g.DrawString(makeLabel(_YTopRange),fnt,brush,rect,fmt);

			rect.Offset(0,Bottom-fnt.Size);
			for( long i=0; i<3; i++ ) 
			{
				long y = _YBottomRange+i*step;
				g.DrawString(makeLabel(y),fnt,brush,rect,fmt);
				rect.Offset(0,-10*_YPics);
			}
		}

		private string makeLabel(double v) 
		{
			v *= YLabelBase;
			if( v < 1000) 
			{
				if( v< 10 )
					return v.ToString("F1");
				else
					return v.ToString("F0");
			}			
			if( v < 1000000)
			{
				v /= 1000;
				if( v< 10 )
					return v.ToString("F1")+"K";
				else
					return v.ToString("F0")+"K";
			}
			if( v < 1000000000) 
			{
				v /= 1000000;
				if( v< 10 )
					return v.ToString("F1")+"M";
				else
					return v.ToString("F0")+"M";
			}
			if( v < 1000000000000) 
			{
				v /= 1000000000;
				if( v< 10 )
					return v.ToString("F1")+"G";
				else
					return v.ToString("F0")+"G";
			}
			v /= 1000000000000;
			if( v< 10 )
				return v.ToString("F1")+"T";
			else
				return v.ToString("F0")+"T";
		}
	}
}