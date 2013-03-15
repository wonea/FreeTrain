using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.controls;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.contributions.train;
using freetrain.world;
using freetrain.util;

namespace TrainListBuilder.src
{
	public class MainForm : System.Windows.Forms.Form
	{
		public MainForm( string[] pluginDirs ) {
			InitializeComponent();
			new FileDropHandler( this, new FileDropEventHandler(onFileDrop) );

			// initializes the freetrain framework
//			freetrain.framework.Core.init( pluginDirs, this, new MenuItem(), null );
		}
		/// <summary>
		/// Run this application in automated mode.
		/// </summary>
		/// <param name="target">the target directory</param>
		public MainForm( string[] pluginDirs, string target ) : this(pluginDirs) {
			output.Text = target;
		}

		private void onFileDrop( string path ) {
			if( Directory.Exists(path) )
				output.Text = path;
		}

		protected override void OnLoad(System.EventArgs e) {
			base.OnLoad(e);

			// set a dummy world
			World.world = new World(new Distance(20,20,10),2);

			if( !output.Text.Equals("") ) {
				buttonOK_Click(null,null);
				Close();
			}
		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox output;
		private System.Windows.Forms.Button selectDir;
		private System.ComponentModel.Container components = null;

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.buttonOK = new System.Windows.Forms.Button();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			this.output = new System.Windows.Forms.TextBox();
			this.selectDir = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(208, 80);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(96, 24);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "Create";
			//! this.buttonOK.Text = "作成";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(8, 56);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(296, 16);
			this.progressBar.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Output directory:";
			//! this.label1.Text = "出力先：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// output
			// 
			this.output.Location = new System.Drawing.Point(72, 8);
			this.output.Name = "output";
			this.output.Size = new System.Drawing.Size(216, 19);
			this.output.TabIndex = 3;
			this.output.Text = "";
			// 
			// selectDir
			// 
			this.selectDir.Enabled = false;
			this.selectDir.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.selectDir.Location = new System.Drawing.Point(288, 8);
			this.selectDir.Name = "selectDir";
			this.selectDir.Size = new System.Drawing.Size(19, 19);
			this.selectDir.TabIndex = 4;
			this.selectDir.Text = "...";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(314, 110);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.selectDir,
																		  this.output,
																		  this.label1,
																		  this.progressBar,
																		  this.buttonOK});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.Text = "HTML Train List Generation";
			//! this.Text = "列車一覧HTML作成ツール";
			this.ResumeLayout(false);

		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			string dir = output.Text;

			if( !Directory.Exists(dir) ) {
				MessageBox.Show(this,"The output directory does not exist");
				//! MessageBox.Show(this,"指定された出力先は存在しません");
				return;
			}

			// initialize the progress bar
			progressBar.Minimum = 0;
			progressBar.Maximum = Core.plugins.trains.Length;
			progressBar.Value = 0;

			// prepare the XML writer
			TextWriter xml = new StreamWriter(
					new FileStream(Path.Combine(dir,"index.xml"),FileMode.Create));
			xml.WriteLine("<?xml version='1.0'?>");
			xml.WriteLine("<trainList>");

			xml.WriteLine("<companies>");
			foreach( string company in listCompanies() )
				writeElement( xml, company, "company" );
			xml.WriteLine("</companies>");

			foreach( TrainContribution contrib in Core.plugins.trains ) {
				using( PreviewDrawer pd = contrib.createPreview( new Size(150,75) ) ) {
					using( Bitmap bmp = pd.createBitmap() ) {
						string fileName = Path.Combine( dir, contrib.id+".png" );
						bmp.Save( fileName, ImageFormat.Png );
					}
				}
				outputManifest( contrib, xml );

				progressBar.Value++;
				Application.DoEvents();
			}

			xml.WriteLine("</trainList>");
			xml.Close();
		}

		private string[] listCompanies() {
			Set r = new Set();
			foreach( TrainContribution contrib in Core.plugins.trains )
				r.add( contrib.companyName );
			
			return (string[])r.toArray(typeof(string));
		}

		private void outputManifest( TrainContribution contrib, TextWriter xml ) {
			xml.WriteLine("<train id='{0}'>",contrib.id);
			writeElement( xml, contrib.name, "name" );
			writeElement( xml, contrib.author, "author" );
			writeElement( xml, contrib.companyName, "company" );
			writeElement( xml, contrib.description, "description" );
			writeElement( xml, contrib.nickName, "nickName" );
			writeElement( xml, contrib.speedDisplayName, "speed" );
			writeElement( xml, contrib.typeName, "type" );
			xml.WriteLine("</train>");
		}

		private void writeElement( TextWriter xml, object data, string tagName ) {
			xml.WriteLine("<{0}><![CDATA[{1}]]></{0}>", tagName, data );
		}


		[STAThread]
		static void Main(string[] args)  {
			ArrayList dirs = new ArrayList();
			string outputDir = null;

			for( int i=0; i<args.Length; i++ ) {
				if( args[i].Equals("-output")) {
					outputDir = args[++i];
					continue;
				}
				if( args[i].Equals("-plugin")) {
					dirs.Add(args[++i]);
					continue;
				}
				MessageBox.Show(
					"Usage:\n"+
					"  -output output directory\n"+
					"  -plugin extra plugin directory" );
					//! "使用方法：\n"+
					//! "  -output 出力ディレクトリ\n"+
					//! "  -plugin エクストラプラグインディレクトリ" );
				return;
			}

			string[] _dirs = (string[])dirs.ToArray(typeof(string));
			// just create a main window instance to keep the framework happy.
			new MainWindow(_dirs,false);

			// run
			MainForm frm;
			if( outputDir==null )	frm = new MainForm(_dirs);
			else					frm = new MainForm(_dirs,outputDir);
			Application.Run(frm);
		}
	}
}
