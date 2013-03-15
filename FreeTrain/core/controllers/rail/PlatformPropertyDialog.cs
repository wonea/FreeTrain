using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.contributions.rail;
using freetrain.framework;

namespace freetrain.world.rail
{
	/// <summary>
	/// PlatformPropertyDialog の概要の説明です。
	/// </summary>
	public class PlatformPropertyDialog : Form
	{

		private readonly Platform platform;
		/// <summary>
		/// Indecies of left/right lanes.
		/// </summary>
		private readonly int lIdx,rIdx;

		public PlatformPropertyDialog(Platform platform) {
			this.platform = platform;

			InitializeComponent();

			if(platform.direction.index>=4) {
				lIdx=0; rIdx=1;
			} else {
				lIdx=1; rIdx=0;
			}
			remove.Enabled = platform.canRemove;
			updateDialog();
			nameBox.Text = platform.name;

			groupFat.Visible = ( platform is FatPlatform );

			// fill host list
			foreach( PlatformHost host in platform.listHosts())
				hostList.Items.Add(host);
			hostList.SelectedItem = platform.host;

			// bell sound list
			bell.DataSource = DepartureBellContribution.all;
			bell.SelectedItem = platform.bellSound;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.ComboBox bell;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupFat;
		private System.Windows.Forms.Button left;
		private System.Windows.Forms.Button right;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button OKbutton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button remove;
		private System.Windows.Forms.PictureBox warning;
		private System.Windows.Forms.ComboBox hostList;
		private System.Windows.Forms.TextBox nameBox;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlatformPropertyDialog));
			this.left = new System.Windows.Forms.Button();
			this.right = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.nameBox = new System.Windows.Forms.TextBox();
			this.groupFat = new System.Windows.Forms.GroupBox();
			this.OKbutton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.hostList = new System.Windows.Forms.ComboBox();
			this.remove = new System.Windows.Forms.Button();
			this.warning = new System.Windows.Forms.PictureBox();
			this.bell = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupFat.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.warning)).BeginInit();
			this.SuspendLayout();
			// 
			// left
			// 
			this.left.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.left.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.left.Location = new System.Drawing.Point(96, 17);
			this.left.Name = "left";
			this.left.Size = new System.Drawing.Size(88, 26);
			this.left.TabIndex = 7;
			this.left.Text = "&Left connect";
			//! this.left.Text = "左に接続(&L)";
			this.left.Click += new System.EventHandler(this.onLeft);
			// 
			// right
			// 
			this.right.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.right.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.right.Location = new System.Drawing.Point(192, 17);
			this.right.Name = "right";
			this.right.Size = new System.Drawing.Size(80, 26);
			this.right.TabIndex = 8;
			this.right.Text = "&Right connect";
			//! this.right.Text = "右に接続(&R)";
			this.right.Click += new System.EventHandler(this.onRight);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "&Name:";
			//! this.label1.Text = "名前(&N):";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// nameBox
			// 
			this.nameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nameBox.Location = new System.Drawing.Point(104, 9);
			this.nameBox.Name = "nameBox";
			this.nameBox.Size = new System.Drawing.Size(176, 20);
			this.nameBox.TabIndex = 2;
			this.nameBox.Text = "";
			// 
			// groupFat
			// 
			this.groupFat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupFat.Controls.Add(this.left);
			this.groupFat.Controls.Add(this.right);
			this.groupFat.Location = new System.Drawing.Point(8, 113);
			this.groupFat.Name = "groupFat";
			this.groupFat.Size = new System.Drawing.Size(280, 52);
			this.groupFat.TabIndex = 999;
			this.groupFat.TabStop = false;
			this.groupFat.Text = "Connect tracks";
			//! this.groupFat.Text = "線路との接続工事";
			// 
			// OKbutton
			// 
			this.OKbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OKbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKbutton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.OKbutton.Location = new System.Drawing.Point(104, 173);
			this.OKbutton.Name = "OKbutton";
			this.OKbutton.Size = new System.Drawing.Size(88, 26);
			this.OKbutton.TabIndex = 10;
			this.OKbutton.Text = "&OK";
			this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cancelButton.Location = new System.Drawing.Point(200, 173);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(80, 26);
			this.cancelButton.TabIndex = 11;
			this.cancelButton.Text = "&Cancel";
			//! this.cancelButton.Text = "キャンセル(&C)";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 18);
			this.label2.TabIndex = 3;
			this.label2.Text = "&Station:";
			//! this.label2.Text = "駅(&S):";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// hostList
			// 
			this.hostList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.hostList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.hostList.Location = new System.Drawing.Point(104, 43);
			this.hostList.Name = "hostList";
			this.hostList.Size = new System.Drawing.Size(176, 21);
			this.hostList.TabIndex = 4;
			// 
			// remove
			// 
			this.remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.remove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.remove.Location = new System.Drawing.Point(8, 173);
			this.remove.Name = "remove";
			this.remove.Size = new System.Drawing.Size(88, 26);
			this.remove.TabIndex = 9;
			this.remove.Text = "R&emove";
			//! this.remove.Text = "撤去(&E)";
			this.remove.Click += new System.EventHandler(this.onRemove);
			// 
			// warning
			// 
			this.warning.Image = ((System.Drawing.Image)(resources.GetObject("warning.Image")));
			this.warning.Location = new System.Drawing.Point(8, 46);
			this.warning.Name = "warning";
			this.warning.Size = new System.Drawing.Size(15, 18);
			this.warning.TabIndex = 1000;
			this.warning.TabStop = false;
			// 
			// bell
			// 
			this.bell.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.bell.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.bell.Location = new System.Drawing.Point(104, 78);
			this.bell.Name = "bell";
			this.bell.Size = new System.Drawing.Size(176, 21);
			this.bell.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 74);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 21);
			this.label3.TabIndex = 5;
			this.label3.Text = "&Bell:";
			//! this.label3.Text = "ベル(&B):";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// PlatformPropertyDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(298, 204);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label3,
																		  this.bell,
																		  this.warning,
																		  this.remove,
																		  this.hostList,
																		  this.label2,
																		  this.cancelButton,
																		  this.OKbutton,
																		  this.groupFat,
																		  this.nameBox,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PlatformPropertyDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Platform properties";
			//! this.Text = "ホームのプロパティ";
			this.groupFat.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.warning)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion


		private void updateDialog() {
			if( platform is FatPlatform ) {
				FatPlatform fp = (FatPlatform)platform;
				if(!fp.hasLane(lIdx))	left.Text = "Left connect";
				//! if(!fp.hasLane(lIdx))	left.Text = "左に接続";
				else					left.Text = "Left release";
				//! else					left.Text = "左を解放";
				if(!fp.hasLane(rIdx))	right.Text = "Right connect";
				//! if(!fp.hasLane(rIdx))	right.Text = "右に接続";
				else					right.Text = "Right release";
				//! else					right.Text = "右を解放";
			}

			if( platform.host!=null )	warning.Hide();
			else						warning.Show();
		}

		private void onLeft(object sender, EventArgs e) { build(lIdx); }
		private void onRight(object sender, EventArgs e) { build(rIdx); }

		private void build( int index ) {
			Debug.Assert( platform is FatPlatform );
			FatPlatform fp = (FatPlatform)platform;

			if(fp.hasLane(index)) {
				if(fp.canRemoveLane(index))
					fp.removeLane(index);
				else
					MainWindow.showError("Could not be released due to an obstacle");
					//! MainWindow.showError("障害物があって解放できません");
			} else {
				if(fp.canAddLane(index))
					fp.addLane(index);
				else
					MainWindow.showError("Could not be connected due to an obstacle");
					//! MainWindow.showError("障害物があって接続できません");
			}
			updateDialog();
		}

		private void OKbutton_Click(object sender, System.EventArgs e) {
			platform.name = nameBox.Text;
			platform.host = (PlatformHost)hostList.SelectedItem;
			platform.bellSound = (DepartureBellContribution)bell.SelectedItem;
		}

		private void onRemove(object sender, System.EventArgs e) {
			if(MessageBox.Show(this,"Do you want to remove this station?","Remove station",MessageBoxButtons.YesNo,MessageBoxIcon.Question)
			//! if(MessageBox.Show(this,"このホームを撤去しますか？","ホームの撤去",MessageBoxButtons.YesNo,MessageBoxIcon.Question)
			== DialogResult.Yes) {
				platform.remove();
				Close();	// close the dialog
			}
		}
	}
}
