using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.contributions.train;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.views;
using freetrain.world.accounting;

namespace freetrain.world.rail
{
	/// <summary>
	/// Dialog box to buy trains
	/// </summary>
	public class TrainTradingDialog : Form
	{
		public TrainTradingDialog() {
			InitializeComponent();
			//handler = new OptionChangedHandler(updatePreview);
			World.world.viewOptions.OnViewOptionChanged+=new OptionChangedHandler(updatePreview);
			Bitmap bmp = ResourceUtil.loadSystemBitmap("DayNight.bmp");
			buttonImages.TransparentColor = bmp.GetPixel(0,0);
			buttonImages.Images.AddStrip(bmp);
			
			tbDay.Pushed=(World.world.viewOptions.nightSpriteMode==NightSpriteMode.AlwaysDay);
			tbNight.Pushed=(World.world.viewOptions.nightSpriteMode==NightSpriteMode.AlwaysNight);

			// organize trains into a tree
			IDictionary types = new SortedList();
			foreach( TrainContribution tc in Core.plugins.trains ) {
				IDictionary company = (IDictionary)types[tc.companyName];
				if( company==null )
					types[tc.companyName] = company = new SortedList();

				IDictionary type = (IDictionary)company[tc.typeName];
				if( type==null )
					company[tc.typeName] = type = new SortedList();

				type.Add( tc.nickName, tc );
			}

			// build a tree
			foreach( DictionaryEntry company in types ) {
				TreeNode cn = new TreeNode((string)company.Key);
				typeTree.Nodes.Add(cn);

				foreach( DictionaryEntry type in (IDictionary)company.Value ) {
					IDictionary t = (IDictionary)type.Value;
					if(t.Count==1) {
						addTrains( cn, t );
					} else {
						TreeNode tn = new TreeNode((string)type.Key);
						cn.Nodes.Add(tn);

						addTrains( tn, t );
					}
				}
			}

			onTypeChanged(null,null);
		}

		private void addTrains( TreeNode parent, IDictionary list ) {
			foreach( DictionaryEntry trainEntry in list ) {
				TrainContribution t = (TrainContribution)trainEntry.Value;

				TreeNode trainNode = new TreeNode(t.name);
				trainNode.Tag = t;
				parent.Nodes.Add(trainNode);
			}
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

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown length;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label speed;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label totalPrice;
		private System.Windows.Forms.NumericUpDown count;
		private System.Windows.Forms.Label passenger;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TreeView typeTree;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox description;
		private System.Windows.Forms.Label author;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label name;
		private System.Windows.Forms.PictureBox preview;
		private System.Windows.Forms.ToolBarButton tbDay;
		private System.Windows.Forms.ToolBarButton tbNight;
		private System.Windows.Forms.ImageList buttonImages;
		private System.Windows.Forms.ToolBar toolBarDayNight;
		private System.ComponentModel.IContainer components;
		
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label2 = new System.Windows.Forms.Label();
			this.length = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.count = new System.Windows.Forms.NumericUpDown();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.speed = new System.Windows.Forms.Label();
			this.totalPrice = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.passenger = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.typeTree = new System.Windows.Forms.TreeView();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.description = new System.Windows.Forms.TextBox();
			this.author = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.name = new System.Windows.Forms.Label();
			this.preview = new System.Windows.Forms.PictureBox();
			this.buttonImages = new System.Windows.Forms.ImageList(this.components);
			this.toolBarDayNight = new System.Windows.Forms.ToolBar();
			this.tbDay = new System.Windows.Forms.ToolBarButton();
			this.tbNight = new System.Windows.Forms.ToolBarButton();
			((System.ComponentModel.ISupportInitialize)(this.length)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.count)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(202, 257);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(76, 24);
			this.label2.TabIndex = 2;
			this.label2.Text = "&Length:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// length
			// 
			this.length.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.length.Location = new System.Drawing.Point(283, 258);
			this.length.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.length.Name = "length";
			this.length.Size = new System.Drawing.Size(77, 21);
			this.length.TabIndex = 4;
			this.length.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.length.Value = new decimal(new int[] {
									3,
									0,
									0,
									0});
			this.length.ValueChanged += new System.EventHandler(this.onAmountChanged);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(409, 259);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(30, 20);
			this.label3.TabIndex = 5;
			this.label3.Text = "x";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// count
			// 
			this.count.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.count.Location = new System.Drawing.Point(435, 258);
			this.count.Name = "count";
			this.count.Size = new System.Drawing.Size(76, 21);
			this.count.TabIndex = 6;
			this.count.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.count.Value = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.count.ValueChanged += new System.EventHandler(this.onAmountChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Location = new System.Drawing.Point(180, 291);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(458, 4);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(517, 257);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(39, 22);
			this.label4.TabIndex = 7;
			this.label4.Text = "set(s)";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(356, 346);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(96, 28);
			this.buttonOK.TabIndex = 8;
			this.buttonOK.Text = "&Buy";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(460, 346);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(96, 28);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "&Close";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(202, 229);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(76, 19);
			this.label5.TabIndex = 10;
			this.label5.Text = "Speed:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// speed
			// 
			this.speed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.speed.Location = new System.Drawing.Point(286, 229);
			this.speed.Name = "speed";
			this.speed.Size = new System.Drawing.Size(264, 19);
			this.speed.TabIndex = 11;
			this.speed.Text = "Rapid";
			this.speed.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// totalPrice
			// 
			this.totalPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.totalPrice.Location = new System.Drawing.Point(283, 298);
			this.totalPrice.Name = "totalPrice";
			this.totalPrice.Size = new System.Drawing.Size(273, 20);
			this.totalPrice.TabIndex = 14;
			this.totalPrice.Text = "100,000";
			this.totalPrice.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.Location = new System.Drawing.Point(202, 298);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(76, 20);
			this.label8.TabIndex = 15;
			this.label8.Text = "Total cost:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// passenger
			// 
			this.passenger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.passenger.Location = new System.Drawing.Point(283, 318);
			this.passenger.Name = "passenger";
			this.passenger.Size = new System.Drawing.Size(267, 19);
			this.passenger.TabIndex = 17;
			this.passenger.Text = "100";
			this.passenger.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.Location = new System.Drawing.Point(202, 318);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(76, 19);
			this.label9.TabIndex = 16;
			this.label9.Text = "Capacity:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// typeTree
			// 
			this.typeTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.typeTree.Location = new System.Drawing.Point(0, 0);
			this.typeTree.Name = "typeTree";
			this.typeTree.Size = new System.Drawing.Size(194, 380);
			this.typeTree.TabIndex = 18;
			this.typeTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.onTypeChanged);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(202, 130);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 20);
			this.label1.TabIndex = 19;
			this.label1.Text = "Author:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(202, 154);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(76, 19);
			this.label6.TabIndex = 20;
			this.label6.Text = "Description:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// description
			// 
			this.description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.description.BackColor = System.Drawing.SystemColors.Control;
			this.description.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.description.Location = new System.Drawing.Point(286, 158);
			this.description.Multiline = true;
			this.description.Name = "description";
			this.description.ReadOnly = true;
			this.description.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.description.Size = new System.Drawing.Size(264, 68);
			this.description.TabIndex = 21;
			this.description.Text = "tadasdffas";
			// 
			// author
			// 
			this.author.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.author.Location = new System.Drawing.Point(283, 130);
			this.author.Name = "author";
			this.author.Size = new System.Drawing.Size(267, 20);
			this.author.TabIndex = 22;
			this.author.Text = "477";
			this.author.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(370, 257);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(40, 22);
			this.label7.TabIndex = 23;
			this.label7.Text = "car(s)";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.Location = new System.Drawing.Point(202, 10);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(76, 18);
			this.label10.TabIndex = 24;
			this.label10.Text = "Name:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// name
			// 
			this.name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.name.Location = new System.Drawing.Point(286, 10);
			this.name.Name = "name";
			this.name.Size = new System.Drawing.Size(264, 18);
			this.name.TabIndex = 25;
			this.name.Text = "123 Series ABCDEF";
			this.name.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// preview
			// 
			this.preview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.preview.Location = new System.Drawing.Point(283, 38);
			this.preview.Name = "preview";
			this.preview.Size = new System.Drawing.Size(267, 84);
			this.preview.TabIndex = 26;
			this.preview.TabStop = false;
			// 
			// buttonImages
			// 
			this.buttonImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.buttonImages.ImageSize = new System.Drawing.Size(16, 15);
			this.buttonImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// toolBarDayNight
			// 
			this.toolBarDayNight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.toolBarDayNight.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
									this.tbDay,
									this.tbNight});
			this.toolBarDayNight.Dock = System.Windows.Forms.DockStyle.None;
			this.toolBarDayNight.DropDownArrows = true;
			this.toolBarDayNight.ImageList = this.buttonImages;
			this.toolBarDayNight.Location = new System.Drawing.Point(230, 38);
			this.toolBarDayNight.Name = "toolBarDayNight";
			this.toolBarDayNight.ShowToolTips = true;
			this.toolBarDayNight.Size = new System.Drawing.Size(46, 48);
			this.toolBarDayNight.TabIndex = 27;
			this.toolBarDayNight.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// tbDay
			// 
			this.tbDay.ImageIndex = 1;
			this.tbDay.Name = "tbDay";
			this.tbDay.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbDay.Tag = freetrain.views.NightSpriteMode.AlwaysDay;
			// 
			// tbNight
			// 
			this.tbNight.ImageIndex = 2;
			this.tbNight.Name = "tbNight";
			this.tbNight.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbNight.Tag = freetrain.views.NightSpriteMode.AlwaysNight;
			// 
			// TrainTradingDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(570, 380);
			this.Controls.Add(this.toolBarDayNight);
			this.Controls.Add(this.preview);
			this.Controls.Add(this.name);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.author);
			this.Controls.Add(this.description);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.typeTree);
			this.Controls.Add(this.passenger);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.totalPrice);
			this.Controls.Add(this.speed);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.count);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.length);
			this.Controls.Add(this.label2);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TrainTradingDialog";
			this.Text = "Train Trading";
			this.Load += new System.EventHandler(this.TrainTradingDialogLoad);
			((System.ComponentModel.ISupportInitialize)(this.length)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.count)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion

		private TrainContribution selectedTrain {
			get {
				TreeNode n = typeTree.SelectedNode;
				if(n==null)	return null;
				return (TrainContribution)n.Tag;
			}
		}

		private long getTotalPrice() {
			return (long)(selectedTrain.price(1) * length.Value * count.Value);
		}

		private void onTypeChanged(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			updatePreview();
		}

		public void updatePreview() 
		{
			length.Enabled = count.Enabled = buttonOK.Enabled = (selectedTrain!=null);

			Image im = preview.Image;
			if(im!=null) {
				preview.Image = null;
				im.Dispose();
			}

			if( selectedTrain!=null ) {
				name.Text = selectedTrain.name;
				author.Text = selectedTrain.author;
				description.Text = selectedTrain.description;
				speed.Text = selectedTrain.speedDisplayName;
				using( PreviewDrawer pd = selectedTrain.createPreview( preview.ClientSize ) ) {
					preview.Image = pd.createBitmap();
				}

				if( count.Value==0 )
					// if the user changes the type, s/he is going to buy another train.
					// thus change the value to 1.
					count.Value=1;

				onAmountChanged(null,null);
			} else {
				name.Text = author.Text = description.Text = speed.Text = "";
			}
		}

		private void onAmountChanged(object sender, EventArgs e) {
			if( count.Value!=0 && selectedTrain!=null ) {
				TrainCarContribution[] cars = selectedTrain.create((int)length.Value);
				if( cars!=null ) {
					buttonOK.Enabled = true;

					// TODO: non-linear price support
					totalPrice.Text = getTotalPrice().ToString();

					int p=0;
					foreach( TrainCarContribution car in cars )
						p += car.capacity;

					passenger.Text = p.ToString()+" passengers/set";
					//! passenger.Text = p.ToString()+" 人/編成";
					return;
				}
			}

			buttonOK.Enabled = false;
			totalPrice.Text = "---";
			passenger.Text = "---";

		}

		private void buttonOK_Click(object sender, EventArgs e) {
			// buy trains
			for( int i=0; i<(int)count.Value; i++ )
				new Train( World.world.rootTrainGroup,
					(int)length.Value, selectedTrain );

			freetrain.framework.sound.SoundEffectManager
				.PlaySynchronousSound( ResourceUtil.findSystemResource("vehiclePurchase.wav") );
			
			AccountManager.theInstance.spend( getTotalPrice(), AccountGenre.RAIL_SERVICE );

			// set count to 0 to avoid accidental purchase
			count.Value=0;
		}

		private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			foreach( ToolBarButton tb in toolBarDayNight.Buttons)
			{
				if(e.Button == tb)
				{
					if(tb.Pushed)
						World.world.viewOptions.nightSpriteMode = (NightSpriteMode)tb.Tag;
					else
						World.world.viewOptions.nightSpriteMode = NightSpriteMode.AlignClock;
				}
				else
				{
					tb.Pushed = false;
				}
			}
		}

		private void TrainTradingDialog_Closed(object sender, System.EventArgs e)
		{
			World.world.viewOptions.OnViewOptionChanged-=new OptionChangedHandler(updatePreview);
		}

		
		void TrainTradingDialogLoad(object sender, EventArgs e)
		{
			
		}
	}
}
