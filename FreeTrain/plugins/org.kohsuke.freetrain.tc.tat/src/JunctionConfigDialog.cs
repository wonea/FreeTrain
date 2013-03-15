using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace freetrain.world.rail.tattc
{
	/// <summary>
	/// Configuration Dialog of JunctionController
	/// </summary>
	internal class JunctionConfigDialog : Form
	{
		internal JunctionConfigDialog( Junction jc ) {
			this.junction = jc;
			InitializeComponent();

			updateDirectionButton();
		}

		/// <summary>
		/// The junction controller which we are configuring.
		/// </summary>
		private readonly Junction junction;


		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.Button buttonDirection;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonAdvanced;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JunctionConfigDialog));
			this.buttonDirection = new System.Windows.Forms.Button();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonAdvanced = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonDirection
			// 
			this.buttonDirection.ImageIndex = 0;
			this.buttonDirection.ImageList = this.imageList;
			this.buttonDirection.Location = new System.Drawing.Point(24, 43);
			this.buttonDirection.Name = "buttonDirection";
			this.buttonDirection.Size = new System.Drawing.Size(72, 78);
			this.buttonDirection.TabIndex = 1;
			this.buttonDirection.Click += new System.EventHandler(this.buttonDirection_Click);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "");
			this.imageList.Images.SetKeyName(1, "");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 31);
			this.label1.TabIndex = 1;
			this.label1.Text = "&Direction of movement:";
			//! this.label1.Text = "進行方向(&D)：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOk.Location = new System.Drawing.Point(128, 43);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(72, 26);
			this.buttonOk.TabIndex = 3;
			this.buttonOk.Text = "&OK";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Location = new System.Drawing.Point(120, -9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(2, 141);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			// 
			// buttonAdvanced
			// 
			this.buttonAdvanced.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonAdvanced.Location = new System.Drawing.Point(128, 9);
			this.buttonAdvanced.Name = "buttonAdvanced";
			this.buttonAdvanced.Size = new System.Drawing.Size(72, 26);
			this.buttonAdvanced.TabIndex = 2;
			this.buttonAdvanced.Text = "&Advanced...";
			//! this.buttonAdvanced.Text = "詳細(&A)...";
			this.buttonAdvanced.Click += new System.EventHandler(this.buttonAdvanced_Click);
			// 
			// JunctionConfigDialog
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(206, 130);
			this.ControlBox = false;
			this.Controls.Add(this.buttonAdvanced);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonDirection);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "JunctionConfigDialog";
			this.Text = "Point settings";
			//! this.Text = "ポイントの設定";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonDirection_Click(object sender, System.EventArgs e) {
			junction.defaultRoute =
				(junction.defaultRoute==JunctionRoute.Curve)
				? JunctionRoute.Straight : JunctionRoute.Curve;
			updateDirectionButton();
			World.world.onVoxelUpdated( junction.location );	// upte the map window
		}

		/// <summary>
		/// Update the image of the direction button.
		/// </summary>
		private void updateDirectionButton() {
			if(junction.defaultRoute==JunctionRoute.Curve)
				buttonDirection.ImageIndex = 1;
			else
				buttonDirection.ImageIndex = 0;
		}

		private void buttonAdvanced_Click(object sender, System.EventArgs e) {
			new JunctionAdvancedDialog(junction).ShowDialog(this);
		}

		protected override void OnClosed(EventArgs e) {
			// redraw the voxel
			World.world.onVoxelUpdated(junction.location);
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			// move the dialog to the cursor
			Point pt = Cursor.Position;
			pt.X -= Width/2;
			pt.Y -= Height/2;
			this.Location = pt;
		}
	}
}
