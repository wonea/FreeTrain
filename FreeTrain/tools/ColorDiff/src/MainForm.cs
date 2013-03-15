using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Text;
using freetrain.controls;

namespace ColorDiff
{
	public class MainForm : System.Windows.Forms.Form
	{

		public MainForm() {
			InitializeComponent();

			// register drag&drop handler
			new FileDropHandler( pictureLeft,  new FileDropEventHandler(OnDropLeft) );
			new FileDropHandler( pictureRight, new FileDropEventHandler(OnDropRight) );
		}

		#region Windows Form Designer generated code
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.PictureBox pictureLeft;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.PictureBox pictureRight;
		private System.Windows.Forms.Button loadLeft;
		private System.Windows.Forms.Button loadRight;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.OpenFileDialog openFileDialog;

		
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.pictureLeft = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.textBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.loadRight = new System.Windows.Forms.Button();
			this.loadLeft = new System.Windows.Forms.Button();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.pictureRight = new System.Windows.Forms.PictureBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureLeft
			// 
			this.pictureLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.pictureLeft.Name = "pictureLeft";
			this.pictureLeft.Size = new System.Drawing.Size(264, 264);
			this.pictureLeft.TabIndex = 2;
			this.pictureLeft.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.textBox,
																				 this.label1,
																				 this.loadRight,
																				 this.loadLeft});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 268);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(554, 96);
			this.panel1.TabIndex = 3;
			// 
			// textBox
			// 
			this.textBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.textBox.Location = new System.Drawing.Point(8, 24);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(440, 64);
			this.textBox.TabIndex = 3;
			this.textBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Mapping: ";
			//! this.label1.Text = "マッピング：";
			// 
			// loadRight
			// 
			this.loadRight.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.loadRight.Location = new System.Drawing.Point(456, 36);
			this.loadRight.Name = "loadRight";
			this.loadRight.Size = new System.Drawing.Size(88, 24);
			this.loadRight.TabIndex = 1;
			this.loadRight.Text = "&Load Right...";
			//! this.loadRight.Text = "右ロード(&L)...";
			this.loadRight.Click += new System.EventHandler(this.OnLoadRight);
			// 
			// loadLeft
			// 
			this.loadLeft.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.loadLeft.Location = new System.Drawing.Point(456, 4);
			this.loadLeft.Name = "loadLeft";
			this.loadLeft.Size = new System.Drawing.Size(88, 24);
			this.loadLeft.TabIndex = 0;
			this.loadLeft.Text = "&Load Left...";
			//! this.loadLeft.Text = "左ロード(&L)...";
			this.loadLeft.Click += new System.EventHandler(this.OnLoadLeft);
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter1.Location = new System.Drawing.Point(0, 264);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(554, 4);
			this.splitter1.TabIndex = 4;
			this.splitter1.TabStop = false;
			// 
			// splitter2
			// 
			this.splitter2.Location = new System.Drawing.Point(264, 0);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(4, 264);
			this.splitter2.TabIndex = 5;
			this.splitter2.TabStop = false;
			// 
			// pictureRight
			// 
			this.pictureRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureRight.Location = new System.Drawing.Point(268, 0);
			this.pictureRight.Name = "pictureRight";
			this.pictureRight.Size = new System.Drawing.Size(286, 264);
			this.pictureRight.TabIndex = 6;
			this.pictureRight.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(554, 364);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pictureRight,
																		  this.splitter2,
																		  this.pictureLeft,
																		  this.splitter1,
																		  this.panel1});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "Image Diff Tool";
			//! this.Text = "画像比較ツール";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		protected override void Dispose( bool disposing ) {
			if( disposing && components != null) 
				components.Dispose();
			base.Dispose( disposing );
		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

		private void OnLoadLeft(object sender, System.EventArgs e) {
			if( openFileDialog.ShowDialog(this)==DialogResult.OK ) {
				OnDropLeft(openFileDialog.FileName);
			}
		}
		private void OnDropLeft( string fileName ) {
			pictureLeft.Image = LoadBitmap(fileName);
			diff();
		}

		private void OnLoadRight(object sender, System.EventArgs e) {
			if( openFileDialog.ShowDialog(this)==DialogResult.OK ) {
				OnDropRight(openFileDialog.FileName);
			}
		}
		private void OnDropRight( string fileName ) {
			pictureRight.Image = LoadBitmap(fileName);
			diff();
		}

		private Bitmap LoadBitmap( string fileName ) {
			using( FileStream fs = new FileStream(fileName,FileMode.Open,FileAccess.Read) ) {
				return new Bitmap(fs);
			}
		}

		private void diff() {
			textBox.Lines = compare().Split('\n');
		}
		private string compare() {
			Bitmap l = (Bitmap)pictureLeft.Image;
			Bitmap r = (Bitmap)pictureRight.Image;

			if(l==null || r==null)	return "Please load two images.";
			//! if(l==null || r==null)	return "２つの画像をロードしてください";
			if(l.Size!=r.Size)		return "The images have different dimensions ("+l.Size+" vs "+r.Size+")";
			//! if(l.Size!=r.Size)		return "画像のサイズが違います("+l.Size+" vs "+r.Size+")";

			StringBuilder output = new StringBuilder();

			// Color -> Color map
			IDictionary assoc = new Hashtable();
			IDictionary conflicts = new Hashtable();	// used as set.

			for( int y=0; y<l.Size.Height; y++ ) {
				for( int x=0; x<l.Size.Width; x++ ) {
					Color src = l.GetPixel(x,y);
					Color dst = r.GetPixel(x,y);

					if( assoc.Contains(src) && !assoc[src].Equals(dst) ) {
						if( !conflicts.Contains(src) ) {
							conflicts[src]=dst;	// avoid duplicating errors
							output.Append(
								string.Format("Inconsistent mapping: ({0})→({1}),({2})\n",
								//! string.Format("一貫性のないマッピング: ({0})→({1}),({2})\n",
									printColor(src),
									printColor(dst),
									printColor((Color)assoc[src]) ));
						}
					} else {
						if( !src.Equals(dst) && !assoc.Contains(src) )
							// first time this color is mapped.
							output.Append("<map from=\""+printColor(src)+"\" to=\""+printColor(dst)+"\"/>\n");
						
						// record the mapping, even for non-mapped colors
						assoc[src]=dst;
					}
				}
			}
			return output.ToString();
		}

		private string printColor( Color c ) {
			return string.Format("{0},{1},{2}", c.R, c.G, c.B );
		}
	}
}
