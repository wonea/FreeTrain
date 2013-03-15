using System;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace freetrain.world.rail.tattc
{
	/// <summary>
	/// JunctionAdvancedDialog の概要の説明です。
	/// </summary>
	internal class StationAdvancedDialog : Form
	{
		internal StationAdvancedDialog( AdvancedStationHandler handler ) {
			this.station = handler;
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			resetEntryBoxes();
			onSelectionChanged(null,null);

			// populate the list view
			foreach( AdvStationRule rule in station.rules ) {
				ListViewItem lvi = new ListViewItem();
				updateListViewItem(rule,lvi);
				triggerList.Items.Add(lvi);
			}
		}

		private System.Windows.Forms.ComboBox minBox;
		private System.Windows.Forms.RadioButton radioReverse;

		private readonly AdvancedStationHandler station;

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private System.Windows.Forms.RadioButton radioStop;
		private System.Windows.Forms.RadioButton radioPass;
		private System.Windows.Forms.RadioButton radioGo;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox monthBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox dayBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox dayOfWeekBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox hourBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonUp;
		private System.Windows.Forms.Button buttonDown;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Button buttonReplace;
		private System.Windows.Forms.ListView triggerList;
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioReverse = new System.Windows.Forms.RadioButton();
			this.minBox = new System.Windows.Forms.ComboBox();
			this.radioGo = new System.Windows.Forms.RadioButton();
			this.radioStop = new System.Windows.Forms.RadioButton();
			this.radioPass = new System.Windows.Forms.RadioButton();
			this.buttonReplace = new System.Windows.Forms.Button();
			this.hourBox = new System.Windows.Forms.ComboBox();
			this.dayOfWeekBox = new System.Windows.Forms.ComboBox();
			this.dayBox = new System.Windows.Forms.ComboBox();
			this.monthBox = new System.Windows.Forms.ComboBox();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonUp = new System.Windows.Forms.Button();
			this.buttonDown = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.triggerList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.radioReverse);
			this.groupBox1.Controls.Add(this.minBox);
			this.groupBox1.Controls.Add(this.radioGo);
			this.groupBox1.Controls.Add(this.radioStop);
			this.groupBox1.Controls.Add(this.radioPass);
			this.groupBox1.Controls.Add(this.buttonReplace);
			this.groupBox1.Controls.Add(this.hourBox);
			this.groupBox1.Controls.Add(this.dayOfWeekBox);
			this.groupBox1.Controls.Add(this.dayBox);
			this.groupBox1.Controls.Add(this.monthBox);
			this.groupBox1.Controls.Add(this.buttonAdd);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(0, 9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(499, 86);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Requirements";
			//! this.groupBox1.Text = "条件";
			// 
			// radioReverse
			// 
			this.radioReverse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioReverse.Location = new System.Drawing.Point(160, 52);
			this.radioReverse.Name = "radioReverse";
			this.radioReverse.Size = new System.Drawing.Size(72, 26);
			this.radioReverse.TabIndex = 15;
			this.radioReverse.Text = "Reverse";
			//! this.radioReverse.Text = "折返発車";
			this.radioReverse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// minBox
			// 
			this.minBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.minBox.Items.AddRange(new object[] {
														"*",
														"00",
														"10",
														"20",
														"30",
														"40",
														"50"});
			this.minBox.Location = new System.Drawing.Point(389, 17);
			this.minBox.MaxDropDownItems = 13;
			this.minBox.Name = "minBox";
			this.minBox.Size = new System.Drawing.Size(40, 21);
			this.minBox.TabIndex = 13;
			// 
			// radioGo
			// 
			this.radioGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioGo.Location = new System.Drawing.Point(112, 52);
			this.radioGo.Name = "radioGo";
			this.radioGo.Size = new System.Drawing.Size(48, 26);
			this.radioGo.TabIndex = 12;
			this.radioGo.Text = "Go";
			//! this.radioGo.Text = "発車";
			this.radioGo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// radioStop
			// 
			this.radioStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioStop.Location = new System.Drawing.Point(64, 52);
			this.radioStop.Name = "radioStop";
			this.radioStop.Size = new System.Drawing.Size(48, 26);
			this.radioStop.TabIndex = 9;
			this.radioStop.Text = "Stop";
			//! this.radioStop.Text = "停車";
			this.radioStop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// radioPass
			// 
			this.radioPass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioPass.Checked = true;
			this.radioPass.Location = new System.Drawing.Point(16, 52);
			this.radioPass.Name = "radioPass";
			this.radioPass.Size = new System.Drawing.Size(48, 26);
			this.radioPass.TabIndex = 8;
			this.radioPass.TabStop = true;
			this.radioPass.Text = "Pass";
			//! this.radioPass.Text = "通過";
			// 
			// buttonReplace
			// 
			this.buttonReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonReplace.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonReplace.Location = new System.Drawing.Point(427, 52);
			this.buttonReplace.Name = "buttonReplace";
			this.buttonReplace.Size = new System.Drawing.Size(64, 26);
			this.buttonReplace.TabIndex = 11;
			this.buttonReplace.Text = "&Replace";
			//! this.buttonReplace.Text = "置換(&R)";
			this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
			// 
			// hourBox
			// 
			this.hourBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.hourBox.Items.AddRange(new object[] {
														 "*",
														 "00",
														 "01",
														 "02",
														 "03",
														 "04",
														 "05",
														 "06",
														 "07",
														 "08",
														 "09",
														 "10",
														 "11",
														 "12",
														 "13",
														 "14",
														 "15",
														 "16",
														 "17",
														 "18",
														 "19",
														 "20",
														 "21",
														 "22",
														 "23"});
			this.hourBox.Location = new System.Drawing.Point(309, 17);
			this.hourBox.MaxDropDownItems = 13;
			this.hourBox.Name = "hourBox";
			this.hourBox.Size = new System.Drawing.Size(40, 21);
			this.hourBox.TabIndex = 6;
			// 
			// dayOfWeekBox
			// 
			this.dayOfWeekBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			/*this.dayOfWeekBox.Items.AddRange(new object[] {
															  "*",
 															  "日",
 															  "月",
 															  "火",
 															  "水",
 															  "木",
 															  "金",
 															  "土"*/
			this.dayOfWeekBox.Items.AddRange(new object[] {
															  "*",
															  "7",
															  "1",
															  "2",
															  "3",
															  "4",
															  "5",
															  "6"});
			this.dayOfWeekBox.Location = new System.Drawing.Point(205, 17);
			this.dayOfWeekBox.MaxDropDownItems = 13;
			this.dayOfWeekBox.Name = "dayOfWeekBox";
			this.dayOfWeekBox.Size = new System.Drawing.Size(43, 21);
			this.dayOfWeekBox.TabIndex = 4;
			// 
			// dayBox
			// 
			this.dayBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.dayBox.Items.AddRange(new object[] {
														"*",
														"1",
														"2",
														"3",
														"4",
														"5",
														"6",
														"7",
														"8",
														"9",
														"10",
														"11",
														"12",
														"13",
														"14",
														"15",
														"16",
														"17",
														"18",
														"19",
														"20",
														"21",
														"22",
														"23",
														"24",
														"25",
														"26",
														"27",
														"28",
														"29",
														"30",
														"31"});
			this.dayBox.Location = new System.Drawing.Point(112, 17);
			this.dayBox.MaxDropDownItems = 13;
			this.dayBox.Name = "dayBox";
			this.dayBox.Size = new System.Drawing.Size(48, 21);
			this.dayBox.TabIndex = 2;
			// 
			// monthBox
			// 
			this.monthBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.monthBox.Items.AddRange(new object[] {
														  "*",
														  "1",
														  "2",
														  "3",
														  "4",
														  "5",
														  "6",
														  "7",
														  "8",
														  "9",
														  "10",
														  "11",
														  "12"});
			this.monthBox.Location = new System.Drawing.Point(12, 18);
			this.monthBox.MaxDropDownItems = 13;
			this.monthBox.Name = "monthBox";
			this.monthBox.Size = new System.Drawing.Size(56, 21);
			this.monthBox.TabIndex = 0;
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonAdd.Location = new System.Drawing.Point(355, 52);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(64, 26);
			this.buttonAdd.TabIndex = 10;
			this.buttonAdd.Text = "&Add";
			//! this.buttonAdd.Text = "追加(&A)";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.Transparent;
			this.label5.Location = new System.Drawing.Point(435, 17);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(58, 21);
			this.label5.TabIndex = 14;
			this.label5.Text = "Minute";
			//! this.label5.Text = "分";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.Location = new System.Drawing.Point(355, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(50, 22);
			this.label4.TabIndex = 7;
			this.label4.Text = "Hour";
			//! this.label4.Text = "時";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Location = new System.Drawing.Point(254, 18);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(62, 22);
			this.label3.TabIndex = 5;
			this.label3.Text = "Weekday";
			//! this.label3.Text = "曜日";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(166, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(33, 22);
			this.label2.TabIndex = 3;
			this.label2.Text = "Day";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(74, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 22);
			this.label1.TabIndex = 1;
			this.label1.Text = "Month";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonUp
			// 
			this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonUp.Location = new System.Drawing.Point(427, 104);
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.Size = new System.Drawing.Size(64, 26);
			this.buttonUp.TabIndex = 13;
			//this.buttonUp.Text = "↑";
			this.buttonUp.Text = "Up";
			this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
			// 
			// buttonDown
			// 
			this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonDown.Location = new System.Drawing.Point(427, 139);
			this.buttonDown.Name = "buttonDown";
			this.buttonDown.Size = new System.Drawing.Size(64, 26);
			this.buttonDown.TabIndex = 14;
			//this.buttonDown.Text = "↓";
			this.buttonDown.Text = "Down";
			this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonRemove.Location = new System.Drawing.Point(427, 173);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(64, 26);
			this.buttonRemove.TabIndex = 15;
			this.buttonRemove.Text = "Delete";
			//! this.buttonRemove.Text = "削除";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOk.Location = new System.Drawing.Point(427, 266);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(64, 26);
			this.buttonOk.TabIndex = 16;
			this.buttonOk.Text = "&OK";
			// 
			// triggerList
			// 
			this.triggerList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.triggerList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						  this.columnHeader1,
																						  this.columnHeader2,
																						  this.columnHeader3,
																						  this.columnHeader4,
																						  this.columnHeader5,
																						  this.columnHeader6});
			this.triggerList.FullRowSelect = true;
			this.triggerList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.triggerList.HideSelection = false;
			this.triggerList.Location = new System.Drawing.Point(8, 104);
			this.triggerList.MultiSelect = false;
			this.triggerList.Name = "triggerList";
			this.triggerList.Scrollable = false;
			this.triggerList.Size = new System.Drawing.Size(403, 188);
			this.triggerList.TabIndex = 12;
			this.triggerList.UseCompatibleStateImageBehavior = false;
			this.triggerList.View = System.Windows.Forms.View.Details;
			this.triggerList.SelectedIndexChanged += new System.EventHandler(this.onSelectionChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Month";
			//! this.columnHeader1.Text = "月";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Date";
			//! this.columnHeader2.Text = "日";
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Day";
			//! this.columnHeader3.Text = "曜日";
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Hour";
			//! this.columnHeader4.Text = "時";
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Minute";
			//! this.columnHeader5.Text = "分";
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Action";
			//! this.columnHeader6.Text = "動作";
			// 
			// StationAdvancedDialog
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(499, 304);
			this.Controls.Add(this.triggerList);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonDown);
			this.Controls.Add(this.buttonUp);
			this.Controls.Add(this.groupBox1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(456, 338);
			this.Name = "StationAdvancedDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Advanced departure time settings";
			//! this.Text = "発車時刻の詳細設定";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		/// <summary> clear all the boxes back to the default </summary>
		private void resetEntryBoxes() {
			monthBox.SelectedIndex = 0;
			dayBox.SelectedIndex = 0;
			dayOfWeekBox.SelectedIndex = 0;
			hourBox.SelectedIndex = 0;
			minBox.SelectedIndex = 0;
		}

		private void buttonAdd_Click(object sender, System.EventArgs e) {
			// update the data structure
			AdvStationRule tm = createRule();
			station.rules.add(tm);

			// update the UI
			ListViewItem lvi = new ListViewItem();
			updateListViewItem(tm,lvi);
			triggerList.Items.Add(lvi);
			
			resetEntryBoxes();
		}

		private int selectedIndex { get { return triggerList.SelectedIndices[0]; } }
		private AdvStationRule selectedRule {
			get {
				return (AdvStationRule)triggerList.Items[selectedIndex].Tag;
			}
		}

		private void buttonReplace_Click(object sender, System.EventArgs e) {
			int idx = selectedIndex;

			// update the data structure
			AdvStationRule tm = createRule();
			station.rules.set( idx, tm );

			// update the UI
			ListViewItem lvi = triggerList.Items[idx];
			updateListViewItem(tm,lvi);
		}

		private void buttonUp_Click(object sender, System.EventArgs e) {
			moveData(-1);
		}

		private void buttonDown_Click(object sender, System.EventArgs e) {
			moveData(+1);
		}

		private void moveData( int offset ) {
			int idx = selectedIndex;
			
			// update the data structure
			AdvStationRule rule = selectedRule;
			station.rules.remove(rule);
			station.rules.insert(idx+offset,rule);

			// update the UI
			ListViewItem lvi = triggerList.Items[idx];
			triggerList.Items.Remove(lvi);
			triggerList.Items.Insert(idx+offset,lvi);
		}

		private void buttonRemove_Click(object sender, System.EventArgs e) {
			int idx = selectedIndex;

			// update the data structure
			station.rules.remove( selectedRule );

			// update the UI
			triggerList.Items.RemoveAt(idx);
		}

		private AdvStationRule createRule() {
			AdvStationRule tm = new AdvStationRule();
			
			tm.month = (sbyte)monthBox.SelectedIndex;
			if(tm.month==0)	tm.month = -1;

			tm.day = (sbyte)dayBox.SelectedIndex;
			if(tm.day==0)	tm.day = -1;

			tm.dayOfWeek = (sbyte)(dayOfWeekBox.SelectedIndex-1);

			tm.hour = (sbyte)(hourBox.SelectedIndex-1);

			tm.minutes = (sbyte)(minBox.SelectedIndex*10);
			if(tm.minutes==0)	tm.minutes = -1;
			else				tm.minutes -= 10;

			if( radioPass.Checked )		tm.action = StationAction.pass;
			if( radioStop.Checked )		tm.action = StationAction.stop;
			if( radioGo.Checked )		tm.action = StationAction.go;
			if( radioReverse.Checked )	tm.action = StationAction.reverse;

			return tm;
		}

		private void updateListViewItem( AdvStationRule rule, ListViewItem lvi ) {
			lvi.SubItems.Clear();

			lvi.Tag = rule;
			lvi.Text = numberToString(rule.month);

			lvi.SubItems.Add(numberToString(rule.day));
			if( rule.dayOfWeek==-1 )
				lvi.SubItems.Add("*");
			else
				lvi.SubItems.Add(""+Clock.dayOfWeekChar(rule.dayOfWeek));
			lvi.SubItems.Add(numberToString(rule.hour));
			lvi.SubItems.Add(numberToString(rule.minutes));

			lvi.SubItems.Add( displayName(rule.action) );
		}

		private string displayName( StationAction a ) {
			switch(a) {
			case StationAction.go:		return "Go";
			//! case StationAction.go:		return "発車";
			case StationAction.pass:	return "Pass";
			//! case StationAction.pass:	return "通過";
			case StationAction.stop:	return "Stop";
			//! case StationAction.stop:	return "停車";
			case StationAction.reverse:	return "Reverse";
			//! case StationAction.reverse:	return "折返発車";
			default:
				Debug.Assert(false);
				return null;
			}
		}
		private string numberToString( sbyte i ) {
			if(i==-1)	return "*";
			else		return i.ToString();
		}

		private void onSelectionChanged(object sender, System.EventArgs e) {
			bool b = ( triggerList.SelectedIndices.Count!=0 );
			int idx=-1;
			if( b )	idx = selectedIndex;

			buttonUp.Enabled = b && idx!=0;
			buttonDown.Enabled = b && idx!=triggerList.Items.Count-1;
			buttonRemove.Enabled = b;
			buttonReplace.Enabled = b;

			if(idx!=-1) {
				// update the edit box
				AdvStationRule rule = selectedRule;

				if( rule.month== -1 )	monthBox.SelectedIndex = 0;
				else					monthBox.SelectedIndex = rule.month;

				if( rule.day== -1 )		dayBox.SelectedIndex = 0;
				else					dayBox.SelectedIndex = rule.month;

				dayOfWeekBox.SelectedIndex = rule.dayOfWeek+1;

				hourBox.SelectedIndex = rule.hour+1;

				if( rule.minutes== -1 )	minBox.SelectedIndex = 0;
				else					minBox.SelectedIndex = (rule.minutes/10)+1;

				radioGo.Checked		= (rule.action==StationAction.go);
				radioPass.Checked	= (rule.action==StationAction.pass);
				radioStop.Checked	= (rule.action==StationAction.stop);
				radioReverse.Checked= (rule.action==StationAction.reverse);
			}
		}
	}
}
