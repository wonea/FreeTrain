using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.util.command;
using freetrain.framework;
using freetrain.world;

namespace freetrain.tools.terrainloader
{
	public class LoadDialog : System.Windows.Forms.Form
	{
		public LoadDialog() {
			InitializeComponent();

			commands = new CommandManager();

			new Command( commands )
				.addUpdateHandler( new CommandHandler(updateOKButton) )
				.commandInstances.AddAll( buttonOK );
		}

		private CommandManager commands;


		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox fileName;
		private System.Windows.Forms.Button buttonSelectFile;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox sizeX;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox sizeY;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox sizeZ;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox water;
		private System.Windows.Forms.PictureBox previewBox;
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.fileName = new System.Windows.Forms.TextBox();
			this.buttonSelectFile = new System.Windows.Forms.Button();
			this.previewBox = new System.Windows.Forms.PictureBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.water = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.sizeZ = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.sizeY = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.sizeX = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
			this.SuspendLayout();
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "bmp";
			this.openFileDialog.Filter = "All images (*.bmp;*.gif;*.png;*.jpg)|*.bmp;*.gif;*.png;*.jpg";
			//! this.openFileDialog.Filter = "全ての画像 (*.bmp;*.gif;*.png;*.jpg)|*.bmp;*.gif;*.png;*.jpg";
			this.openFileDialog.RestoreDirectory = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "&File Name:";
			//! this.label1.Text = "ファイル名(&F)：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// fileName
			// 
			this.fileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.fileName.Location = new System.Drawing.Point(67, 9);
			this.fileName.Name = "fileName";
			this.fileName.Size = new System.Drawing.Size(207, 20);
			this.fileName.TabIndex = 1;
			this.fileName.TextChanged += new System.EventHandler(this.onFileNameChanged);
			// 
			// buttonSelectFile
			// 
			this.buttonSelectFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonSelectFile.Location = new System.Drawing.Point(282, 9);
			this.buttonSelectFile.Name = "buttonSelectFile";
			this.buttonSelectFile.Size = new System.Drawing.Size(80, 20);
			this.buttonSelectFile.TabIndex = 2;
			this.buttonSelectFile.Text = "&Select...";
			//! this.buttonSelectFile.Text = "選択(&S)...";
			this.buttonSelectFile.Click += new System.EventHandler(this.onSelectFile);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(194, 216);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 26);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "&OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(282, 216);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 26);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "&Cancel";
			//! this.buttonCancel.Text = "ｷｬﾝｾﾙ(&C)";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.water);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.sizeZ);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.sizeY);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.sizeX);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(179, 43);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(183, 164);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Size";
			//! this.groupBox1.Text = "大きさ";
			// 
			// water
			// 
			this.water.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.water.Location = new System.Drawing.Point(93, 130);
			this.water.Name = "water";
			this.water.Size = new System.Drawing.Size(82, 20);
			this.water.TabIndex = 7;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(6, 130);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(81, 20);
			this.label6.TabIndex = 6;
			this.label6.Text = "W&ater Level";
			//! this.label6.Text = "水面高(&A)：";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sizeZ
			// 
			this.sizeZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.sizeZ.Location = new System.Drawing.Point(93, 95);
			this.sizeZ.Name = "sizeZ";
			this.sizeZ.Size = new System.Drawing.Size(82, 20);
			this.sizeZ.TabIndex = 5;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(6, 95);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(81, 18);
			this.label5.TabIndex = 4;
			this.label5.Text = "Terrain &Height";
			//! this.label5.Text = "高さ(&H)：";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sizeY
			// 
			this.sizeY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.sizeY.Location = new System.Drawing.Point(93, 61);
			this.sizeY.Name = "sizeY";
			this.sizeY.Size = new System.Drawing.Size(82, 20);
			this.sizeY.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(6, 61);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(81, 20);
			this.label4.TabIndex = 2;
			this.label4.Text = "Map Length";
			//! this.label4.Text = "奥行(&D)：";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sizeX
			// 
			this.sizeX.Location = new System.Drawing.Point(93, 26);
			this.sizeX.Name = "sizeX";
			this.sizeX.Size = new System.Drawing.Size(82, 20);
			this.sizeX.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(6, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(81, 20);
			this.label3.TabIndex = 0;
			this.label3.Text = "Map &Width";
			//! this.label3.Text = "幅(&W)：";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.previewBox);
			this.groupBox2.Location = new System.Drawing.Point(8, 43);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(165, 164);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Preview";
			//! this.groupBox2.Text = "プレビュー：";
			// 
			// previewBox
			// 
			this.previewBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.previewBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.previewBox.Location = new System.Drawing.Point(6, 19);
			this.previewBox.Name = "previewBox";
			this.previewBox.Size = new System.Drawing.Size(153, 131);
			this.previewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.previewBox.TabIndex = 5;
			this.previewBox.TabStop = false;
			// 
			// LoadDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(370, 248);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonSelectFile);
			this.Controls.Add(this.fileName);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoadDialog";
			this.ShowInTaskbar = false;
			this.Text = "Loading terrain...";
			//! this.Text = "地形の読み込み";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.previewBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.GroupBox groupBox2;
		#endregion

		private void onSelectFile(object sender, System.EventArgs e) {
			if( openFileDialog.ShowDialog(this) == DialogResult.OK )
				fileName.Text = openFileDialog.FileName;
		}

		private void onFileNameChanged(object sender, System.EventArgs e) {
			if( previewBox.Image!=null ) {
				Image img = previewBox.Image;
				previewBox.Image = null;
				img.Dispose();
			}

			try {
				previewBox.Image = new Bitmap(fileName.Text);
			} catch( Exception ) {
				previewBox.Image = null;
			}
		}

		private void updateOKButton( Command cmd ) {
			try {
				cmd.Enabled = (previewBox.Image!=null)
					&& int.Parse(sizeX.Text)>0
					&& int.Parse(sizeY.Text)>0
					&& int.Parse(sizeZ.Text)>0
					&& int.Parse(water.Text)>=0
					&& int.Parse(sizeZ.Text) > int.Parse(water.Text);
			} catch( Exception ) {
				cmd.Enabled = false;
			}
		}

		public World createWorld() {
			return TerrainLoader.loadWorld(
				(Bitmap)previewBox.Image, 
				new Size( int.Parse(sizeX.Text), int.Parse(sizeY.Text) ),
				int.Parse(sizeZ.Text), int.Parse(water.Text) );
		}

		private void buttonOK_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

	}
}
