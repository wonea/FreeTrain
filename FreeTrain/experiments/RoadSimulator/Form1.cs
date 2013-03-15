using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace RoadSimulator
{
	/// <summary>
	/// Form1 の概要の説明です。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnConfig;
		private System.Windows.Forms.RadioButton rb_HStation;
		private System.Windows.Forms.RadioButton rb_VStation;
		private System.Windows.Forms.RadioButton rb_Remover;
		private System.Windows.Forms.RadioButton rd_Barrier;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.RadioButton rb_Road;
		private RoadSimulator.MapViewer mapView;
		private System.Windows.Forms.CheckBox cbBoxMod;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.NumericUpDown numLevel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton btnPlay;
		private System.Windows.Forms.RadioButton btnStop;
		private System.Windows.Forms.Button btnPhase;
		private System.Windows.Forms.Button btnStep;

		private World world;
		private System.Windows.Forms.Button btnReset;
		private Timer timer;

		public Form1()
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();
			mapView.boxMode = cbBoxMod.Checked;
			world = new World();
			mapView.setWorld(world);
			numLevel.Value =Configure.stationLevel;
			btnStop.Checked = true;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
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
			this.btnStep = new System.Windows.Forms.Button();
			this.btnPhase = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.rb_HStation = new System.Windows.Forms.RadioButton();
			this.rb_VStation = new System.Windows.Forms.RadioButton();
			this.rb_Remover = new System.Windows.Forms.RadioButton();
			this.rd_Barrier = new System.Windows.Forms.RadioButton();
			this.rb_Road = new System.Windows.Forms.RadioButton();
			this.btnConfig = new System.Windows.Forms.Button();
			this.cbBoxMod = new System.Windows.Forms.CheckBox();
			this.btnPlay = new System.Windows.Forms.RadioButton();
			this.btnStop = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.numLevel = new System.Windows.Forms.NumericUpDown();
			this.btnReset = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.mapView = new RoadSimulator.MapViewer();
			((System.ComponentModel.ISupportInitialize)(this.numLevel)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnStep
			// 
			this.btnStep.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.btnStep.Location = new System.Drawing.Point(104, 17);
			this.btnStep.Name = "btnStep";
			this.btnStep.Size = new System.Drawing.Size(40, 23);
			this.btnStep.TabIndex = 1;
			this.btnStep.Text = ">|";
			//! this.btnStep.Text = "＞|";
			this.toolTip1.SetToolTip(this.btnStep, "NEXT STEP");
			this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
			// 
			// btnPhase
			// 
			this.btnPhase.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.btnPhase.Location = new System.Drawing.Point(152, 17);
			this.btnPhase.Name = "btnPhase";
			this.btnPhase.Size = new System.Drawing.Size(40, 23);
			this.btnPhase.TabIndex = 2;
			this.btnPhase.Text = ">>|";
			this.toolTip1.SetToolTip(this.btnPhase, "NEXT PHASE");
			this.btnPhase.Click += new System.EventHandler(this.btnPhase_Click);
			// 
			// rb_HStation
			// 
			this.rb_HStation.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.rb_HStation.Appearance = System.Windows.Forms.Appearance.Button;
			this.rb_HStation.Location = new System.Drawing.Point(104, 16);
			this.rb_HStation.Name = "rb_HStation";
			this.rb_HStation.Size = new System.Drawing.Size(40, 24);
			this.rb_HStation.TabIndex = 5;
			this.rb_HStation.Text = "Station -";
			//! this.rb_HStation.Text = "駅 −";
			this.rb_HStation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip1.SetToolTip(this.rb_HStation, "Place station (horizontally)");;
			//! this.toolTip1.SetToolTip(this.rb_HStation, "駅(横)設置");
			this.rb_HStation.CheckedChanged += new System.EventHandler(this.rb_HStation_CheckedChanged);
			// 
			// rb_VStation
			// 
			this.rb_VStation.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.rb_VStation.Appearance = System.Windows.Forms.Appearance.Button;
			this.rb_VStation.Location = new System.Drawing.Point(152, 16);
			this.rb_VStation.Name = "rb_VStation";
			this.rb_VStation.Size = new System.Drawing.Size(40, 24);
			this.rb_VStation.TabIndex = 5;
			this.rb_VStation.Text = "Station |";
			//! this.rb_VStation.Text = "駅｜";
			this.rb_VStation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip1.SetToolTip(this.rb_VStation, "Place station (vertically)");
			//! this.toolTip1.SetToolTip(this.rb_VStation, "駅(縦)設置");
			this.rb_VStation.CheckedChanged += new System.EventHandler(this.rb_VStation_CheckedChanged);
			// 
			// rb_Remover
			// 
			this.rb_Remover.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.rb_Remover.Appearance = System.Windows.Forms.Appearance.Button;
			this.rb_Remover.Location = new System.Drawing.Point(296, 16);
			this.rb_Remover.Name = "rb_Remover";
			this.rb_Remover.Size = new System.Drawing.Size(40, 24);
			this.rb_Remover.TabIndex = 5;
			this.rb_Remover.Text = "Remove";
			//! this.rb_Remover.Text = "撤去";
			this.rb_Remover.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip1.SetToolTip(this.rb_Remover, "Remove mode");
			//! this.toolTip1.SetToolTip(this.rb_Remover, "撤去モード");
			this.rb_Remover.CheckedChanged += new System.EventHandler(this.rb_Remover_CheckedChanged);
			// 
			// rd_Barrier
			// 
			this.rd_Barrier.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.rd_Barrier.Appearance = System.Windows.Forms.Appearance.Button;
			this.rd_Barrier.Location = new System.Drawing.Point(248, 16);
			this.rd_Barrier.Name = "rd_Barrier";
			this.rd_Barrier.Size = new System.Drawing.Size(40, 24);
			this.rd_Barrier.TabIndex = 5;
			this.rd_Barrier.Text = "Obstacle";
			//! this.rd_Barrier.Text = "障壁";
			this.rd_Barrier.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip1.SetToolTip(this.rd_Barrier, "Place obstacle");
			//! this.toolTip1.SetToolTip(this.rd_Barrier, "障害物設置");
			this.rd_Barrier.CheckedChanged += new System.EventHandler(this.rd_Barrier_CheckedChanged);
			// 
			// rb_Road
			// 
			this.rb_Road.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.rb_Road.Appearance = System.Windows.Forms.Appearance.Button;
			this.rb_Road.Location = new System.Drawing.Point(200, 16);
			this.rb_Road.Name = "rb_Road";
			this.rb_Road.Size = new System.Drawing.Size(40, 24);
			this.rb_Road.TabIndex = 5;
			this.rb_Road.Text = "Road";
			//! this.rb_Road.Text = "道路";
			this.rb_Road.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip1.SetToolTip(this.rb_Road, "Place road");
			//! this.toolTip1.SetToolTip(this.rb_Road, "道路設置");
			this.rb_Road.CheckedChanged += new System.EventHandler(this.rb_Road_CheckedChanged);
			// 
			// btnConfig
			// 
			this.btnConfig.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnConfig.Enabled = false;
			this.btnConfig.Location = new System.Drawing.Point(296, 472);
			this.btnConfig.Name = "btnConfig";
			this.btnConfig.Size = new System.Drawing.Size(72, 24);
			this.btnConfig.TabIndex = 4;
			this.btnConfig.Text = "Environment Settings";
			//! this.btnConfig.Text = "環境設定";
			this.toolTip1.SetToolTip(this.btnConfig, "Environment settings");
			//! this.toolTip1.SetToolTip(this.btnConfig, "環境設定");
			// 
			// cbBoxMod
			// 
			this.cbBoxMod.Location = new System.Drawing.Point(352, 16);
			this.cbBoxMod.Name = "cbBoxMod";
			this.cbBoxMod.Size = new System.Drawing.Size(80, 24);
			this.cbBoxMod.TabIndex = 6;
			this.cbBoxMod.Text = "box mode";
			this.toolTip1.SetToolTip(this.cbBoxMod, "Box place mode");
			//! this.toolTip1.SetToolTip(this.cbBoxMod, "矩形設置モード");
			this.cbBoxMod.CheckedChanged += new System.EventHandler(this.cbBoxMod_CheckedChanged);
			// 
			// btnPlay
			// 
			this.btnPlay.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.btnPlay.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnPlay.Location = new System.Drawing.Point(8, 16);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(40, 24);
			this.btnPlay.TabIndex = 5;
			this.btnPlay.Text = ">";
			//! this.btnPlay.Text = "＞";
			this.btnPlay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip1.SetToolTip(this.btnPlay, "PLAY");
			this.btnPlay.CheckedChanged += new System.EventHandler(this.btnPlay_CheckedChanged);
			// 
			// btnStop
			// 
			this.btnStop.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.btnStop.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnStop.Location = new System.Drawing.Point(56, 16);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(40, 24);
			this.btnStop.TabIndex = 5;
			this.btnStop.Text = "■";
			this.btnStop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.toolTip1.SetToolTip(this.btnStop, "STOP");
			this.btnStop.CheckedChanged += new System.EventHandler(this.btnStop_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 8;
			this.label1.Text = "level:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.toolTip1.SetToolTip(this.label1, "Station and road scale (lower value means larger scale)");
			//! this.toolTip1.SetToolTip(this.label1, "駅や道路の規模(小さい値ほど規模大)");
			// 
			// numLevel
			// 
			this.numLevel.Location = new System.Drawing.Point(56, 19);
			this.numLevel.Maximum = new System.Decimal(new int[] {
																	 6,
																	 0,
																	 0,
																	 0});
			this.numLevel.Name = "numLevel";
			this.numLevel.Size = new System.Drawing.Size(32, 19);
			this.numLevel.TabIndex = 7;
			this.toolTip1.SetToolTip(this.numLevel, "Station and road scale (lower value means larger scale)");
			//! this.toolTip1.SetToolTip(this.numLevel, "駅や道路の規模(小さい値ほど規模大)");
			this.numLevel.ValueChanged += new System.EventHandler(this.numLevel_ValueChanged);
			// 
			// btnReset
			// 
			this.btnReset.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnReset.Location = new System.Drawing.Point(216, 472);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(72, 24);
			this.btnReset.TabIndex = 4;
			this.btnReset.Text = "Reset";
			//! this.btnReset.Text = "リセット";
			this.toolTip1.SetToolTip(this.btnReset, "Reset map");
			//! this.toolTip1.SetToolTip(this.btnReset, "マップをリセット");
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.label1,
																					this.numLevel,
																					this.cbBoxMod,
																					this.rd_Barrier,
																					this.rb_VStation,
																					this.rb_HStation,
																					this.rb_Remover,
																					this.rb_Road});
			this.groupBox1.Location = new System.Drawing.Point(8, 400);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(440, 48);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tool";
			//! this.groupBox1.Text = "ツール";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnStep,
																					this.btnPhase,
																					this.btnPlay,
																					this.btnStop});
			this.groupBox2.Location = new System.Drawing.Point(8, 456);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 48);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Control";
			//! this.groupBox2.Text = "コントロール";
			// 
			// btnClose
			// 
			this.btnClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnClose.Location = new System.Drawing.Point(376, 472);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(72, 24);
			this.btnClose.TabIndex = 4;
			this.btnClose.Text = "Quit";
			//! this.btnClose.Text = "終了";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// mapView
			// 
			this.mapView.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.mapView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.mapView.CausesValidation = false;
			this.mapView.Location = new System.Drawing.Point(16, 16);
			this.mapView.Name = "mapView";
			this.mapView.Size = new System.Drawing.Size(424, 368);
			this.mapView.TabIndex = 8;
			this.mapView.TabStop = false;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(456, 509);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.mapView,
																		  this.groupBox2,
																		  this.groupBox1,
																		  this.btnConfig,
																		  this.btnClose,
																		  this.btnReset});
			this.Name = "Form1";
			this.Text = "RoadSimulator";
			((System.ComponentModel.ISupportInitialize)(this.numLevel)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
		
		#region 'tool' radio button click handlers
		private void rb_HStation_CheckedChanged(object sender, System.EventArgs e)
		{
			mapView.mode = EditMode.StationH;
			cbBoxMod.Enabled = false;
			numLevel.Enabled = true;
		}

		private void rb_VStation_CheckedChanged(object sender, System.EventArgs e)
		{
			mapView.mode = EditMode.StationV;		
			cbBoxMod.Enabled = false;
			numLevel.Enabled = true;
		}

		private void rb_Road_CheckedChanged(object sender, System.EventArgs e)
		{
			mapView.mode =  EditMode.Road;
			cbBoxMod.Enabled = true;
			numLevel.Enabled = true;
		}

		private void rd_Barrier_CheckedChanged(object sender, System.EventArgs e)
		{
			mapView.mode =  EditMode.Barrier;
			cbBoxMod.Enabled = true;
			numLevel.Enabled = false;
		}

		private void rb_Remover_CheckedChanged(object sender, System.EventArgs e)
		{
			mapView.mode =  EditMode.Erase;		
			cbBoxMod.Enabled = true;
			numLevel.Enabled = false;
		}
		#endregion

		private void cbBoxMod_CheckedChanged(object sender, System.EventArgs e)
		{
			mapView.boxMode = cbBoxMod.Checked;
		}

		private void numLevel_ValueChanged(object sender, System.EventArgs e)
		{
			Configure.stationLevel = (int)numLevel.Value;
		}
		
		#region 'control' button click handlers
		private void btnPlay_CheckedChanged(object sender, System.EventArgs e)
		{
			btnPhase.Enabled = false;
			btnStep.Enabled = false;
			killTimer();
			startTimer();
		}

		private void btnStop_CheckedChanged(object sender, System.EventArgs e)
		{
			btnPhase.Enabled = true;
			btnStep.Enabled = true;		
			killTimer();
		}

		private void btnPhase_Click(object sender, System.EventArgs e)
		{
			world.nextPhase();
		}

		private void btnStep_Click(object sender, System.EventArgs e)
		{
			world.nextStep();		
		}
		#endregion

		#region timer functions
		private void killTimer()
		{
			if( timer != null && timer.Enabled)
			{
				timer.Stop();
				//timer.Enabled = false;
				timer.Dispose();
			}
		}

		private void startTimer()
		{
			timer = new Timer();
			timer.Interval = Configure.TimerInterval;
			timer.Tick += new EventHandler(btnStep_Click);
			timer.Start();
		}
		#endregion

		private void btnReset_Click(object sender, System.EventArgs e)
		{
			world = new World();
			mapView.setWorld(world);
		}
	}
}
