using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace freetrain.world.rail
{
	/// <summary>
	/// Property dialog of a station
	/// </summary>
	public class StationPropertyDialog : Form
	{
		#region Windows Form Designer generated code
		
		private System.Windows.Forms.Button remove;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button OKbutton;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label_trains;
		private System.Windows.Forms.Label label_waiting;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label_unloaded;
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
			this.remove = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.OKbutton = new System.Windows.Forms.Button();
			this.nameBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label_unloaded = new System.Windows.Forms.Label();
			this.label_trains = new System.Windows.Forms.Label();
			this.label_waiting = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// remove
			// 
			this.remove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.remove.Location = new System.Drawing.Point(12, 122);
			this.remove.Name = "remove";
			this.remove.Size = new System.Drawing.Size(88, 26);
			this.remove.TabIndex = 3;
			this.remove.Text = "R&emove";
			//! this.remove.Text = "撤去(&E)";
			this.remove.Click += new System.EventHandler(this.remove_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cancelButton.Location = new System.Drawing.Point(230, 122);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(88, 26);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "&Cancel";
			//! this.cancelButton.Text = "キャンセル(&C)";
			// 
			// OKbutton
			// 
			this.OKbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKbutton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.OKbutton.Location = new System.Drawing.Point(123, 122);
			this.OKbutton.Name = "OKbutton";
			this.OKbutton.Size = new System.Drawing.Size(88, 26);
			this.OKbutton.TabIndex = 4;
			this.OKbutton.Text = "&OK";
			this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
			// 
			// nameBox
			// 
			this.nameBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nameBox.Location = new System.Drawing.Point(56, 12);
			this.nameBox.Name = "nameBox";
			this.nameBox.Size = new System.Drawing.Size(262, 20);
			this.nameBox.TabIndex = 2;
			this.nameBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 17);
			this.label1.TabIndex = 1;
			this.label1.Text = "&Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//! this.label1.Text = "名前(&N):";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(6, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(218, 26);
			this.label2.TabIndex = 6;
			this.label2.Text = "Passengers getting off (today/yesterday):";

			//! this.label2.Text = "降車客数（今日/昨日)：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(3, 85);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(221, 26);
			this.label3.TabIndex = 6;
			this.label3.Text = "Departures and arrivals (today/yesterday):";
			//! this.label3.Text = "発着数（今日/昨日)：";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label_unloaded
			// 
			this.label_unloaded.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_unloaded.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label_unloaded.Location = new System.Drawing.Point(230, 64);
			this.label_unloaded.Name = "label_unloaded";
			this.label_unloaded.Size = new System.Drawing.Size(88, 17);
			this.label_unloaded.TabIndex = 7;
			this.label_unloaded.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_trains
			// 
			this.label_trains.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_trains.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label_trains.Location = new System.Drawing.Point(230, 90);
			this.label_trains.Name = "label_trains";
			this.label_trains.Size = new System.Drawing.Size(88, 17);
			this.label_trains.TabIndex = 7;
			this.label_trains.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label_waiting
			// 
			this.label_waiting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label_waiting.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label_waiting.Location = new System.Drawing.Point(230, 38);
			this.label_waiting.Name = "label_waiting";
			this.label_waiting.Size = new System.Drawing.Size(88, 17);
			this.label_waiting.TabIndex = 7;
			this.label_waiting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(15, 38);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(209, 17);
			this.label5.TabIndex = 6;
			this.label5.Text = "Passengers waiting:";

			//! this.label5.Text = "乗車待ち客数：";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// StationPropertyDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(330, 160);
			this.ControlBox = false;
			this.Controls.Add(this.label_unloaded);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.remove);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.OKbutton);
			this.Controls.Add(this.nameBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label_trains);
			this.Controls.Add(this.label_waiting);
			this.Controls.Add(this.label5);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StationPropertyDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Station properties";
			//! this.Text = "駅のプロパティ";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		public StationPropertyDialog( Station st ) {
			this.station = st;

			InitializeComponent();

			// initialize the dialog
			nameBox.Text = station.name;
			label_unloaded.Text = string.Format("{0} / {1}",station.UnloadedToday,station.UnloadedYesterday);
			label_trains.Text = string.Format("{0} / {1}",station.TrainsToday,station.TrainsYesterday);
			label_waiting.Text = string.Format("{0}",station.population);
		}

		/// <summary> Station object to which this dialog is opened for. </summary>
		private readonly Station station;

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}


		private void remove_Click(object sender, EventArgs e) {
			if(MessageBox.Show(this,"Do you want to remove this station?","Remove station",
			//! if(MessageBox.Show(this,"この駅舎を撤去しますか？","駅舎の撤去",
					MessageBoxButtons.YesNo,MessageBoxIcon.Question) != DialogResult.Yes)
				return;

			// destroy the station and close the dialog
			station.remove();
			Close();
		}

		private void OKbutton_Click(object sender, EventArgs e) {
			station.setName(nameBox.Text);
			Close();
		}
	}
}
