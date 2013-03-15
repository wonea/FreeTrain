using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Heat
{
	public class Heat : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		public Heat()
		{
			InitializeComponent();
			for( int i=0; i<256; i++ )
				brushes[i] = new SolidBrush( Color.FromArgb(i,0,255-i) );
			m.source[50,50] = 2000;
			
//			for(int y=30;y<70;y++ )
//				if(y!=52)
//					m.alpha[40,y] = 0.0f;

//			for( int y=30;y<50;y++ )
//				m.alpha[49,y] = 1.4f;
//			for( int x=30;x<70;x++ )
//				m.alpha[x,30] = 1.4f;

			for(int i=40;i<=60;i++) {
				m.alpha[40,i] = m.alpha[i,40] = 0.0f;
				m.alpha[60,i] = m.alpha[i,60] = 0.0f;
			}
			m.alpha[40,53]=1.0f;
			m.alpha[40,52]=1.0f;
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 251);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(292, 22);
			this.statusBar.TabIndex = 0;
			this.statusBar.Text = "statusBar1";
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "snapshot";
			this.menuItem1.Click += new System.EventHandler(this.writeSnapshot);
			// 
			// Heat
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.statusBar});
			this.Menu = this.mainMenu1;
			this.Name = "Heat";
			this.Text = "Heat Propagation Experiment";
			//! this.Text = "熱伝導実験";
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new Heat());
		}

		private System.Windows.Forms.Timer timer;

		private readonly Material m = new Material(100,100);

		private Brush[] brushes = new Brush[256];

		private Bitmap bmp = new Bitmap(100,100);
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		
		private int tick;

		private Random rnd = new Random();
		
		private float f( float x ) {
			return (float)Math.Pow(x,0.3)*20f;
		}

		private void timer_Tick(object sender, System.EventArgs e) {
			for( int i=0; i<8; i++ )	m.next();
			float r = m.next();
			this.Text = "Heat Propagation Model "+(++tick)+":"+r;
			//! this.Text = "熱伝導モデル "+(++tick)+":"+r;
			for( int x=99; x>=0; x-- )
				for( int y=99; y>=0; y-- ) {
					int t = Math.Max(0,Math.Min((int)f(m.temp[x,y]),255));
					bmp.SetPixel(x,y, Color.FromArgb(t,0,255-t) );
				}
			m.source[50,50] = 1000 + (float)rnd.NextDouble()*2000;

			Invalidate();
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			Graphics g = e.Graphics;
			g.DrawImage( bmp, 0,0, 100*ZOOM,100*ZOOM );
		}

		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
		{
			;
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			int x = e.X/ZOOM;
			int y = e.Y/ZOOM;
			if( x<0 || x>=m.temp.GetLength(0) )	return;
			if( y<0 || y>=m.temp.GetLength(1) )	return;
			statusBar.Text = (m.temp[x,y]/m.temp[50,50]).ToString()+" - "+x+","+y;
		}

		private const int ZOOM = 4;

		private void writeSnapshot(object sender, System.EventArgs e) {
			FileStream o = new FileStream(@"c:\heat.txt",FileMode.Create);
			TextWriter w = new StreamWriter(o);
			for(int i=0;i<100;i++)
				w.WriteLine(m.temp[i,50]);
			w.Close();
		}
	}
}
