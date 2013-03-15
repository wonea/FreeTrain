using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace freetrain.world.soccerstadium
{
	/// <summary>
	/// PropertyDialog for a stadium.
	/// </summary>
	internal class StadiumPropertyDialog : System.Windows.Forms.Form
	{
		private readonly StadiumStructure structure;

		#region Windows Form Designer generated code
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.stadiumName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.teamName = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.pastGameView = new System.Windows.Forms.ListView();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.label4 = new System.Windows.Forms.Label();
			this.upcomingGameView = new System.Windows.Forms.ListView();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.button1 = new System.Windows.Forms.Button();
			this.advertise = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Stadium &Name:";
			//! this.label1.Text = "スタジアム名(&N):";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// stadiumName
			// 
			this.stadiumName.Location = new System.Drawing.Point(104, 8);
			this.stadiumName.Name = "stadiumName";
			this.stadiumName.Size = new System.Drawing.Size(240, 19);
			this.stadiumName.TabIndex = 1;
			this.stadiumName.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "&Team Name:";
			//! this.label2.Text = "チーム名(&T):";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// teamName
			// 
			this.teamName.Location = new System.Drawing.Point(104, 32);
			this.teamName.Name = "teamName";
			this.teamName.Size = new System.Drawing.Size(240, 19);
			this.teamName.TabIndex = 3;
			this.teamName.Text = "";
			// 
			// okButton
			// 
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.okButton.Location = new System.Drawing.Point(176, 376);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(80, 24);
			this.okButton.TabIndex = 4;
			this.okButton.Text = "&OK";
			this.okButton.Click += new System.EventHandler(this.onOK);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cancelButton.Location = new System.Drawing.Point(264, 376);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(80, 24);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "&Cancel";
			//! this.cancelButton.Text = "ｷｬﾝｾﾙ(&C)";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Latest match results:";
			//! this.label3.Text = "最近の対戦成績:";
			// 
			// pastGameView
			// 
			this.pastGameView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						   this.columnHeader4,
																						   this.columnHeader1,
																						   this.columnHeader2,
																						   this.columnHeader3});
			this.pastGameView.FullRowSelect = true;
			this.pastGameView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.pastGameView.Location = new System.Drawing.Point(8, 80);
			this.pastGameView.Name = "pastGameView";
			this.pastGameView.Size = new System.Drawing.Size(336, 80);
			this.pastGameView.TabIndex = 7;
			this.pastGameView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Date";
			//! this.columnHeader4.Text = "日付";
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Match opponent";
			//! this.columnHeader1.Text = "対戦相手";
			this.columnHeader1.Width = 100;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Score";
			//! this.columnHeader2.Text = "スコア";
			this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Number of spectators";
			//! this.columnHeader3.Text = "観客動員数";
			this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.columnHeader3.Width = 80;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 176);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "Upcoming matches:";
			//! this.label4.Text = "今後の対戦予定:";
			// 
			// upcomingGameView
			// 
			this.upcomingGameView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							   this.columnHeader5,
																							   this.columnHeader6});
			this.upcomingGameView.FullRowSelect = true;
			this.upcomingGameView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.upcomingGameView.Location = new System.Drawing.Point(8, 192);
			this.upcomingGameView.Name = "upcomingGameView";
			this.upcomingGameView.Size = new System.Drawing.Size(336, 80);
			this.upcomingGameView.TabIndex = 9;
			this.upcomingGameView.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Date";
			//! this.columnHeader5.Text = "日付";
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Match opponent";
			//! this.columnHeader6.Text = "対戦相手";
			this.columnHeader6.Width = 131;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.button1,
																					this.advertise});
			this.groupBox1.Location = new System.Drawing.Point(8, 280);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(336, 80);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Team Management";
			//! this.groupBox1.Text = "チーム運営";
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button1.Location = new System.Drawing.Point(8, 48);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(320, 24);
			this.button1.TabIndex = 1;
			this.button1.Text = "Increase PR spending (100,000)";
			//! this.button1.Text = "チームPR強化 (100,000)";
			this.button1.Click += new System.EventHandler(this.doPR);
			// 
			// advertise
			// 
			this.advertise.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.advertise.Location = new System.Drawing.Point(8, 16);
			this.advertise.Name = "advertise";
			this.advertise.Size = new System.Drawing.Size(320, 24);
			this.advertise.TabIndex = 0;
			this.advertise.Text = "Increase team strength (100,000)";
			//! this.advertise.Text = "チーム戦力補強 (100,000)";
			this.advertise.Click += new System.EventHandler(this.reinforce);
			// 
			// StadiumPropertyDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(354, 405);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1,
																		  this.upcomingGameView,
																		  this.label4,
																		  this.pastGameView,
																		  this.label3,
																		  this.cancelButton,
																		  this.okButton,
																		  this.teamName,
																		  this.label2,
																		  this.stadiumName,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StadiumPropertyDialog";
			this.ShowInTaskbar = false;
			this.Text = "Soccer stadium";
			//! this.Text = "サッカースタジアム";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#region controls
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox stadiumName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox teamName;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button advertise;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ListView pastGameView;
		private System.Windows.Forms.ListView upcomingGameView;
		private System.ComponentModel.Container components = null;
		#endregion

		internal StadiumPropertyDialog( StadiumStructure _structure ) {
			this.structure = _structure;

			InitializeComponent();

			stadiumName.Text = structure.stadiumName;
			teamName.Text = structure.teamName;

			foreach( Game g in structure.pastGames ) {
				ListViewItem lvi = g.createListItem();
				pastGameView.Items.Add( lvi );
				lvi.EnsureVisible();
			}
			
			foreach( Game g in structure.futureGames )
				upcomingGameView.Items.Add( g.createListItem() );
		}

		protected override void Dispose( bool disposing ) {
			if( disposing && components!=null )
				components.Dispose();
			base.Dispose( disposing );
		}


		private void onOK(object sender, EventArgs e) {
			structure.stadiumName = stadiumName.Text;
			structure.teamName = teamName.Text;

			Close();
		}

		private void reinforce(object sender, EventArgs e) {
			structure.reinforce();
		}

		private void doPR(object sender, EventArgs e) {
			structure.doPR();
		}
	}
}
