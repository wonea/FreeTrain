using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace freetrain.framework
{
	/// <summary>
	/// Splash screen that reports the progress of initialization.
	/// </summary>
	public class Splash : Form
	{
		bool exflag = true;
		public Splash() {
			InitializeComponent();
		}

		public void updateMessage( string msg, float progress ) {
			status.Text = msg;
			progressBar.Value = (int)(100*progress);
			Application.DoEvents();
			if(progress>0.4 && exflag){
				Graphics g = pictureBox1.CreateGraphics();
				imageList.Draw(g,232,128,0);
				exflag = false;
			}
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label status;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Splash));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.status = new System.Windows.Forms.Label();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(320, 240);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// status
			// 
			this.status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.status.Location = new System.Drawing.Point(0, 256);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(320, 24);
			this.status.TabIndex = 1;
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(35, 24);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(8, 240);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(304, 16);
			this.progressBar.TabIndex = 2;
			// 
			// Splash
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(320, 280);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.status);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "Splash";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Splash";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
