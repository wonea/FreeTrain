using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Xml;
using freetrain.controls;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.framework.graphics;
using freetrain.util.command;
using freetrain.world;
using org.kohsuke.directdraw;

namespace PicturePreviewer
{
	public class Previewer : System.Windows.Forms.Form, SurfaceLoader
	{
		// 0:day 1:night
		private int dayNight =0;
		// season (0:spring - 3:winter)
		private int season =0;
		// zoom up ratio
		private int zoom =1;

		// bitmap file name
		private String fileName;

		// bitmap loaded as a picture
		private readonly Picture picture;

		// sprite strips each 32 pixel width is made into one sprite
		private Sprite sprite;

		// this sprite factory is used to create sprites
		private SpriteFactory spriteFactory = new SimpleSpriteFactory();

		private WindowedDirectDraw directDraw;
		private System.Windows.Forms.Panel spritePanel;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.TextBox xmlSpriteType;

		private readonly CommandManager cmdManager = new CommandManager();

		public Previewer() {
			InitializeComponent();

			// initializes the freetrain framework
			freetrain.framework.Core.init( new string[0], this, new MenuItem(), null, false );			
			
			// support file D&D
			new FileDropHandler( this, new FileDropEventHandler(onFileDrop) );

			// set up picture.
			SurfaceLoader[,] sl = new SurfaceLoader[4,2];
			sl[0,0] = this;
			picture = new Picture("{9FABD9AE-B223-4caf-B675-56F5C66AA2AF}",sl);

			// register command handlers
			MenuItem[] seasons = new MenuItem[]{ miSpring, miSummer, miFall, miWinter };
			for( int i=0; i<4; i++ ) {
				Command cmd = new Command(cmdManager);
				cmd.commandInstances.Add(seasons[i]);
				new IntCommandHandler( cmd, new IntGetter(getSeason), new IntSetter(setSeason), i );
			}

			MenuItem[] zooms = new MenuItem[]{ miZoom1, miZoom2, miZoom4, miZoom8, miZoom16, miZoom32 };
			for( int i=0; i<6; i++ ) {
				Command cmd = new Command(cmdManager);
				cmd.commandInstances.Add(zooms[i]);
				new IntCommandHandler( cmd, new IntGetter(getZoom), new IntSetter(setZoom), 1<<i );
			}

			MenuItem [] dayNights = new MenuItem[]{ miDay, miNight };
			for( int i=0; i<2; i++ ) {
				Command cmd = new Command(cmdManager);
				cmd.commandInstances.Add(dayNights[i]);
				new IntCommandHandler( cmd, new IntGetter(getDayNight), new IntSetter(setDayNight), i );
			}
		}

		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);
			// set a dummy world
			World.world = new World(new Distance(20,20,10),2);

			directDraw = new WindowedDirectDraw(this);
		}

		protected override void OnPaint(PaintEventArgs e) {
			// do the double buffering to properly honor clipping
			using( Surface s = directDraw.createOffscreenSurface(this.ClientSize) ) {
				// fill them by the background color
				s.fill( backgroundColor );
				if( sprite!=null )
					sprite.draw( s, new Point(0,0) );

				directDraw.primarySurface.blt( PointToScreen(new Point(0,0)), s );
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e) {
			; // don't draw the background
		}

		#region Windows Form Designer generated code

		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem miDay;
		private System.Windows.Forms.MenuItem miNight;
		private System.Windows.Forms.MenuItem miSpring;
		private System.Windows.Forms.MenuItem miSummer;
		private System.Windows.Forms.MenuItem miFall;
		private System.Windows.Forms.MenuItem miWinter;
		private System.Windows.Forms.MenuItem miZoom1;
		private System.Windows.Forms.MenuItem miZoom2;
		private System.Windows.Forms.MenuItem miZoom4;
		private System.Windows.Forms.MenuItem miZoom8;
		private System.Windows.Forms.MenuItem miZoom16;
		private System.Windows.Forms.MenuItem miZoom32;
		private System.ComponentModel.Container components = null;
		
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Previewer));
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.miDay = new System.Windows.Forms.MenuItem();
			this.miNight = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.miSpring = new System.Windows.Forms.MenuItem();
			this.miSummer = new System.Windows.Forms.MenuItem();
			this.miFall = new System.Windows.Forms.MenuItem();
			this.miWinter = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.miZoom1 = new System.Windows.Forms.MenuItem();
			this.miZoom2 = new System.Windows.Forms.MenuItem();
			this.miZoom4 = new System.Windows.Forms.MenuItem();
			this.miZoom8 = new System.Windows.Forms.MenuItem();
			this.miZoom16 = new System.Windows.Forms.MenuItem();
			this.miZoom32 = new System.Windows.Forms.MenuItem();
			this.spritePanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.xmlSpriteType = new System.Windows.Forms.TextBox();
			this.spritePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "bmp";
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem4,
																					  this.menuItem12});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2,
																					  this.menuItem3});
			this.menuItem1.Text = "&File";
			//! this.menuItem1.Text = "ファイル(&F)";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "&Open...";
			//! this.menuItem2.Text = "開く(&O)...";
			this.menuItem2.Click += new System.EventHandler(this.open);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Shortcut = System.Windows.Forms.Shortcut.F4;
			this.menuItem3.Text = "&Reload";
			//! this.menuItem3.Text = "再読み込み(&R)";
			this.menuItem3.Click += new System.EventHandler(this.reload);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 1;
			this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.miDay,
																					  this.miNight,
																					  this.menuItem7,
																					  this.miSpring,
																					  this.miSummer,
																					  this.miFall,
																					  this.miWinter});
			this.menuItem4.Text = "&Timer";
			//! this.menuItem4.Text = "時間(&T)";
			// 
			// miDay
			// 
			this.miDay.Index = 0;
			this.miDay.RadioCheck = true;
			this.miDay.Text = "&Day";
			//! this.miDay.Text = "昼(&D)";
			// 
			// miNight
			// 
			this.miNight.Index = 1;
			this.miNight.RadioCheck = true;
			this.miNight.Text = "&Night";
			//! this.miNight.Text = "夜(&N)";
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 2;
			this.menuItem7.RadioCheck = true;
			this.menuItem7.Text = "-";
			// 
			// miSpring
			// 
			this.miSpring.Index = 3;
			this.miSpring.RadioCheck = true;
			this.miSpring.Text = "&Spring";
			//! this.miSpring.Text = "春(&S)";
			// 
			// miSummer
			// 
			this.miSummer.Index = 4;
			this.miSummer.RadioCheck = true;
			this.miSummer.Text = "S&ummer";
			//! this.miSummer.Text = "夏(&U)";
			// 
			// miFall
			// 
			this.miFall.Index = 5;
			this.miFall.RadioCheck = true;
			this.miFall.Text = "&Fall";
			//! this.miFall.Text = "秋(&F)";
			// 
			// miWinter
			// 
			this.miWinter.Index = 6;
			this.miWinter.RadioCheck = true;
			this.miWinter.Text = "&Winter";
			//! this.miWinter.Text = "冬(&W)";
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 2;
			this.menuItem12.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.miZoom1,
																					   this.miZoom2,
																					   this.miZoom4,
																					   this.miZoom8,
																					   this.miZoom16,
																					   this.miZoom32});
			this.menuItem12.Text = "&Zoom";
			//! this.menuItem12.Text = "拡大(&Z)";
			// 
			// miZoom1
			// 
			this.miZoom1.Index = 0;
			this.miZoom1.RadioCheck = true;
			this.miZoom1.Text = "x&1";
			// 
			// miZoom2
			// 
			this.miZoom2.Index = 1;
			this.miZoom2.RadioCheck = true;
			this.miZoom2.Text = "x&2";
			// 
			// miZoom4
			// 
			this.miZoom4.Index = 2;
			this.miZoom4.RadioCheck = true;
			this.miZoom4.Text = "x&4";
			// 
			// miZoom8
			// 
			this.miZoom8.Index = 3;
			this.miZoom8.RadioCheck = true;
			this.miZoom8.Text = "x&8";
			// 
			// miZoom16
			// 
			this.miZoom16.Index = 4;
			this.miZoom16.RadioCheck = true;
			this.miZoom16.Text = "x1&6";
			// 
			// miZoom32
			// 
			this.miZoom32.Index = 5;
			this.miZoom32.RadioCheck = true;
			this.miZoom32.Text = "x3&2";
			// 
			// spritePanel
			// 
			this.spritePanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.xmlSpriteType,
																					  this.label1});
			this.spritePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.spritePanel.Location = new System.Drawing.Point(0, 167);
			this.spritePanel.Name = "spritePanel";
			this.spritePanel.Size = new System.Drawing.Size(472, 136);
			this.spritePanel.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(472, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Sprite:";
			//! this.label1.Text = "スプライト：";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(0, 163);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(472, 4);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 303);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(472, 24);
			this.statusBar.TabIndex = 2;
			// 
			// xmlSpriteType
			// 
			this.xmlSpriteType.AcceptsReturn = true;
			this.xmlSpriteType.AcceptsTab = true;
			this.xmlSpriteType.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.xmlSpriteType.Location = new System.Drawing.Point(0, 16);
			this.xmlSpriteType.Multiline = true;
			this.xmlSpriteType.Name = "xmlSpriteType";
			this.xmlSpriteType.Size = new System.Drawing.Size(472, 120);
			this.xmlSpriteType.TabIndex = 2;
			this.xmlSpriteType.Text = "Input the <spriteType> tag";
			//! this.xmlSpriteType.Text = "<spriteType>タグを入力してください";
			this.xmlSpriteType.TextChanged += new System.EventHandler(this.onSpriteXmlChanged);
			// 
			// Previewer
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(472, 327);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.splitter1,
																		  this.spritePanel,
																		  this.statusBar});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "Previewer";
			this.Text = "FreeTrain picture previewer";
			//! this.Text = "FreeTrain 画像プレビューア";
			this.spritePanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null) 
				components.Dispose();
			directDraw.Dispose();
			base.Dispose( disposing );
		}

		#endregion




		[STAThread]
		static void Main(string[] args)  {
			// just create a main window instance to keep the framework happy.
			new MainWindow(args,false);

			// set up all FreeTrain framework
			Application.Run(new Previewer());
		}


		private int getDayNight() {
			return dayNight;
		}
		private bool isDaytime { get { return dayNight==0; } }
		private void setDayNight(int value) {
			dayNight=value;
			reload(null,null);
		}
		private int getSeason() {
			return season;
		}
		private void setSeason(int value) {
			season = value;
			reload(null,null);
		}
		private int getZoom() {
			return zoom;
		}
		private void setZoom(int value) {
			zoom = value;
			reload(null,null);
		}


		/// <summary>
		/// Background color. Depends on the season and the time
		/// </summary>
		private Color backgroundColor {
			get {
				switch(season) {
				case 0:
				case 1:
				case 2:
					if( isDaytime )		return Color.FromArgb(222,195,132);
					else				return Color.FromArgb( 66, 56, 57);

				case 3:
					if( isDaytime )		return Color.FromArgb(239,235,239);
					else				return Color.FromArgb( 90, 89, 99);

				}

				throw new Exception();
			}
		}



		private void open(object sender, System.EventArgs e) {
			if( openFileDialog.ShowDialog(this) == DialogResult.OK ) {
				fileName = openFileDialog.FileName;
				reload(null,null);
			}
		}

		private void reload(object sender, System.EventArgs e) {
			reload();
		}





		private void onFileDrop( string name ) {
			this.fileName = name;
			reload(null,null);
		}
		
		public void reload() {
			if( fileName==null )	return;	// no file is opend yet

			picture.release();	// discard the current contents

			// configure the clock appropriately
			World.world.clock.setCurrentTime( 
				(5+season*3)*30*Time.DAY + (isDaytime?12*Time.HOUR:0) );
			
			// recreate sprites
			sprite = spriteFactory.createSprite( picture,
				new Point(0,0), new Point(0,0),
				picture.surface.size );

			// invalid the window to repaint
			Invalidate();
		}


		// load picture into this surface.
		public Color load(ref Surface surface) {
			using( Bitmap bmp = new Bitmap(fileName) ) {
				if(surface!=null)
					surface.Dispose();
				
				Size sz = bmp.Size;
				sz.Width *= zoom;
				sz.Height *= zoom;

				surface = ResourceUtil.directDraw.createOffscreenSurface( sz );

				using( GDIGraphics g = new GDIGraphics(surface) ) {
					g.graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
					g.graphics.PixelOffsetMode = PixelOffsetMode.Half;
					g.graphics.DrawImage( bmp, new Rectangle( new Point(0,0), sz ) );
				}
				return bmp.GetPixel(0,0);
			}
		}

		private void onSpriteXmlChanged(object sender, System.EventArgs e) {
			statusBar.Text = "";
			try {
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlSpriteType.Text);

				spriteFactory = SpriteFactory.getSpriteFactory( doc );
				reload();
			} catch( Exception ex ) {
				statusBar.Text = ex.Message;
			}
		}		
	}
}
