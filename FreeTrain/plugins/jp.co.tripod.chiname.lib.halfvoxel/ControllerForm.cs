using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.controllers;
using freetrain.contributions.common;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.terrain;
using freetrain.framework;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.util;
using org.kohsuke.directdraw;

namespace freetrain.world.structs.hv
{
	/// <summary>
	/// Controller that allows the user to
	/// place/remove cars.
	/// </summary>
	public class ControllerForm :ControllerHostForm
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton btnRemove;
		private System.Windows.Forms.RadioButton btnPlace;
		private freetrain.controls.CostBox price;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.GroupBox group;
		private System.Windows.Forms.Label namelabel;
		private freetrain.controls.IndexSelector idxDesign;
		private freetrain.controls.IndexSelector idxColor;
		private System.Windows.Forms.ComboBox typeBox;
		private System.Windows.Forms.CheckBox cbRndColor;
		private System.Windows.Forms.CheckBox cbRndDesign;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox cbRndColor2;
		private freetrain.controls.IndexSelector idxColor2;

		private Bitmap previewBitmap;
		private Random rnd;

		public ControllerForm() {
			InitializeComponent();
			World.world.viewOptions.OnViewOptionChanged+=new OptionChangedHandler(updatePreview);
			rnd = new Random();
			
			callback = new createCallback(randomize);
			typeBox.DataSource = loadContributions();
			onTypeChanged(this,null);
			onButtonClicked(this,null);
		}

		/// <summary>
		/// Called to prepare array of contribution used for typeBox.DataSource
		/// </summary>
		/// <returns></returns>
		private ArrayList loadContributions()
		{
			Array src = PluginManager.theInstance.listContributions(typeof(HalfVoxelContribution));
			Hashtable h = new Hashtable();
			foreach( HalfVoxelContribution c in src)
			{
				string key = c.subgroup;
				if(!h.ContainsKey(key))
					h.Add(key,new SubGroup(key));					
				((SubGroup)h[key]).Add(c);
			}
			ArrayList dest = new ArrayList();
			foreach(object o in h.Values)
				dest.Add(o);
			return dest;
		}

		protected override void Dispose( bool disposing ) {
			World.world.viewOptions.OnViewOptionChanged-=new OptionChangedHandler(updatePreview);
			if( disposing && components != null)
				components.Dispose();
			base.Dispose( disposing );
			
			if( previewBitmap!=null )
				previewBitmap.Dispose();
		}

		#region Designer generated code
		private System.Windows.Forms.PictureBox preview;
		private System.ComponentModel.IContainer components = null;
		private freetrain.controls.IndexSelector indexSelector;

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.preview = new System.Windows.Forms.PictureBox();
			this.btnRemove = new System.Windows.Forms.RadioButton();
			this.btnPlace = new System.Windows.Forms.RadioButton();
			this.group = new System.Windows.Forms.GroupBox();
			this.namelabel = new System.Windows.Forms.Label();
			this.idxDesign = new freetrain.controls.IndexSelector();
			this.idxColor = new freetrain.controls.IndexSelector();
			this.label1 = new System.Windows.Forms.Label();
			this.typeBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cbRndColor = new System.Windows.Forms.CheckBox();
			this.cbRndDesign = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cbRndColor2 = new System.Windows.Forms.CheckBox();
			this.idxColor2 = new freetrain.controls.IndexSelector();
			this.price = new freetrain.controls.CostBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.group.SuspendLayout();
			this.SuspendLayout();
			// 
			// preview
			// 
			this.preview.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.preview.Location = new System.Drawing.Point(152, 8);
			this.preview.Name = "preview";
			this.preview.Size = new System.Drawing.Size(80, 112);
			this.preview.TabIndex = 1;
			this.preview.TabStop = false;
			this.toolTip1.SetToolTip(this.preview, "Click to create another random pattern");
			//! this.toolTip1.SetToolTip(this.preview, "クリックすると別のランダムパターンを生成します");
			this.preview.Click += new System.EventHandler(this.onPreviewClick);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnRemove.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnRemove.Location = new System.Drawing.Point(188, 164);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(56, 24);
			this.btnRemove.TabIndex = 8;
			this.btnRemove.Text = "Remove";
			//! this.btnRemove.Text = "撤去";
			this.btnRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btnRemove.Click += new System.EventHandler(this.onButtonClicked);
			// 
			// btnPlace
			// 
			this.btnPlace.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.btnPlace.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnPlace.Checked = true;
			this.btnPlace.Location = new System.Drawing.Point(140, 164);
			this.btnPlace.Name = "btnPlace";
			this.btnPlace.Size = new System.Drawing.Size(48, 24);
			this.btnPlace.TabIndex = 7;
			this.btnPlace.TabStop = true;
			this.btnPlace.Text = "Place";
			//! this.btnPlace.Text = "設置";
			this.btnPlace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btnPlace.Click += new System.EventHandler(this.onButtonClicked);
			// 
			// group
			// 
			this.group.Controls.AddRange(new System.Windows.Forms.Control[] {
																				this.namelabel,
																				this.idxDesign,
																				this.idxColor,
																				this.label1,
																				this.typeBox,
																				this.label3,
																				this.cbRndColor,
																				this.cbRndDesign,
																				this.label2,
																				this.cbRndColor2,
																				this.idxColor2});
			this.group.Name = "group";
			this.group.Size = new System.Drawing.Size(136, 188);
			this.group.TabIndex = 1;
			this.group.TabStop = false;
			this.group.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)|(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
			this.toolTip1.SetToolTip(this.group, "Select a building to the left");
			//! this.toolTip1.SetToolTip(this.group, "左側の建物選択");
			// 
			// namelabel
			// 
			this.namelabel.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.namelabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.namelabel.Location = new System.Drawing.Point(8, 76);
			this.namelabel.Name = "namelabel";
			this.namelabel.Size = new System.Drawing.Size(120, 28);
			this.namelabel.TabIndex = 13;
			this.namelabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// idxDesign
			// 
			this.idxDesign.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.idxDesign.count = 10;
			this.idxDesign.current = 0;
			this.idxDesign.dataSource = null;
			this.idxDesign.Location = new System.Drawing.Point(8, 56);
			this.idxDesign.Name = "idxDesign";
			this.idxDesign.Size = new System.Drawing.Size(120, 16);
			this.idxDesign.TabIndex = 5;
			this.idxDesign.indexChanged += new System.EventHandler(this.onDesignChanged);
			// 
			// idxColor
			// 
			this.idxColor.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.idxColor.count = 10;
			this.idxColor.current = 0;
			this.idxColor.dataSource = null;
			this.idxColor.Location = new System.Drawing.Point(8, 126);
			this.idxColor.Name = "idxColor";
			this.idxColor.Size = new System.Drawing.Size(120, 16);
			this.idxColor.TabIndex = 11;
			this.idxColor.indexChanged += new System.EventHandler(this.onColorChanged);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(8, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 12;
			this.label1.Text = "Design:";
			//! this.label1.Text = "デザイン：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// typeBox
			// 
			this.typeBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.typeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.typeBox.Location = new System.Drawing.Point(3, 8);
			this.typeBox.Name = "typeBox";
			this.typeBox.Size = new System.Drawing.Size(130, 20);
			this.typeBox.Sorted = true;
			this.typeBox.TabIndex = 1;
			this.toolTip1.SetToolTip(this.typeBox, "Select a building to the left");
			//! this.toolTip1.SetToolTip(this.typeBox, "左側の建物選択");
			this.typeBox.SelectedIndexChanged += new System.EventHandler(this.onTypeChanged);
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Location = new System.Drawing.Point(8, 108);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 16);
			this.label3.TabIndex = 12;
			this.label3.Text = "Color:";
			//! this.label3.Text = "カラー：";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbRndColor
			// 
			this.cbRndColor.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.cbRndColor.BackColor = System.Drawing.Color.Transparent;
			this.cbRndColor.Location = new System.Drawing.Point(68, 108);
			this.cbRndColor.Name = "cbRndColor";
			this.cbRndColor.Size = new System.Drawing.Size(68, 16);
			this.cbRndColor.TabIndex = 7;
			this.cbRndColor.Text = "Random";
			//! this.cbRndColor.Text = "ランダム";
			this.cbRndColor.CheckedChanged += new System.EventHandler(this.onCheckBoxChanged);
			// 
			// cbRndDesign
			// 
			this.cbRndDesign.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.cbRndDesign.BackColor = System.Drawing.Color.Transparent;
			this.cbRndDesign.Location = new System.Drawing.Point(68, 40);
			this.cbRndDesign.Name = "cbRndDesign";
			this.cbRndDesign.Size = new System.Drawing.Size(68, 16);
			this.cbRndDesign.TabIndex = 3;
			this.cbRndDesign.Text = "Random";
			//! this.cbRndDesign.Text = "ランダム";
			this.cbRndDesign.CheckedChanged += new System.EventHandler(this.onCheckBoxChanged);
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(8, 148);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 16);
			this.label2.TabIndex = 12;
			this.label2.Text = "Color 2:";
			//! this.label2.Text = "カラー2：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbRndColor2
			// 
			this.cbRndColor2.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.cbRndColor2.BackColor = System.Drawing.Color.Transparent;
			this.cbRndColor2.Location = new System.Drawing.Point(68, 148);
			this.cbRndColor2.Name = "cbRndColor2";
			this.cbRndColor2.Size = new System.Drawing.Size(68, 16);
			this.cbRndColor2.TabIndex = 7;
			this.cbRndColor2.Text = "Random";
			//! this.cbRndColor2.Text = "ランダム";
			this.cbRndColor2.CheckedChanged += new System.EventHandler(this.onCheckBoxChanged);
			// 
			// idxColor2
			// 
			this.idxColor2.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.idxColor2.count = 10;
			this.idxColor2.current = 0;
			this.idxColor2.dataSource = null;
			this.idxColor2.Location = new System.Drawing.Point(8, 166);
			this.idxColor2.Name = "idxColor2";
			this.idxColor2.Size = new System.Drawing.Size(120, 16);
			this.idxColor2.TabIndex = 11;
			this.idxColor2.indexChanged += new System.EventHandler(this.onColor2Changed);
			// 
			// price
			// 
			this.price.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.price.cost = 0;
			this.price.label = "Cost:";
			//! this.price.label = "費用：";
			this.price.Location = new System.Drawing.Point(152, 130);
			this.price.Name = "price";
			this.price.Size = new System.Drawing.Size(80, 32);
			this.price.TabIndex = 14;
			this.toolTip1.SetToolTip(this.price, "Building cost (total)");
			//! this.toolTip1.SetToolTip(this.price, "設置費用（左右合計）");
			// 
			// ControllerForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(248, 194);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.group,
																		  this.btnRemove,
																		  this.btnPlace,
																		  this.preview,
																		  this.price});
			this.Name = "ControllerForm";
			this.Text = "Half-tile Construction";
			//! this.Text = "半ボクセル建築";
			this.Resize += new EventHandler(this.updateSize);
			this.group.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		public bool isPlacing {
			get {
				return btnPlace.Checked;
			}
		}

		#region private properties
		private int currentColor2 
		{
			get 
			{
				return idxColor2.current;
			}
		}


		private int currentColor 
		{
			get 
			{
				return idxColor.current;
			}
		}

		private int currentDesign 
		{
			get 
			{
				return idxDesign.current;
			}
		}

		private SubGroup currentGroup 
		{
			get 
			{
				return (SubGroup)typeBox.SelectedItem;
			}
		}

		private HalfVoxelContribution currentContrib 
		{
			get 
			{
				return (HalfVoxelContribution)currentGroup[currentDesign];
			}
		}
		#endregion

		#region Event Handlers
		private void onTypeChanged(object sender, System.EventArgs e) 
		{			
			idxDesign.count = currentGroup.size;
			onDesignChanged(sender,e);
		}

		private void onDesignChanged(object sender, System.EventArgs e)
		{
			idxColor.count = currentContrib.colors.size;
			idxColor2.count = currentContrib.getHighlihtPatternCount();
			namelabel.Text = currentContrib.name;
			price.cost = currentContrib.price;
			onButtonClicked(sender,e);
			onColorChanged(sender,e);
			onColor2Changed(sender,e);
		}

		private void onColorChanged(object sender, System.EventArgs e) 
		{
			currentContrib.currentColor  = currentColor;
			updatePreview();
		}

		private void onColor2Changed(object sender, System.EventArgs e) 
		{
			currentContrib.currentHighlight  = currentColor2;
			updatePreview();
		}

		private void onCheckBoxChanged(object sender, System.EventArgs e) 
		{
			idxDesign.Enabled = !cbRndDesign.Checked;
			idxColor.Enabled = !cbRndColor.Checked;
			idxColor2.Enabled = !cbRndColor2.Checked;
			onPreviewClick(sender,e);
		}

		private void onPreviewClick(object sender, System.EventArgs e)
		{
			if(cbRndDesign.Checked)
			{
				idxDesign.current = rnd.Next(idxDesign.count);
				idxColor.count = currentContrib.colors.size;
			}
			if(cbRndColor.Checked)
				idxColor.current = rnd.Next(idxColor.count);
			if(cbRndColor2.Checked)
				idxColor2.current = rnd.Next(idxColor2.count);

			if(cbRndDesign.Checked)
				onDesignChanged(sender,e);
			else if(cbRndColor.Checked)
				onColorChanged(sender,e);
		}

		private void onButtonClicked(object sender, System.EventArgs e) 
		{
			if(currentController!=null)
				((HVControllerImpl)currentController).onCreated -= callback;
			if(isPlacing) 
			{
				currentController = currentContrib.createBuilder(this.siteImpl);
				((HVControllerImpl)currentController).onCreated += callback;
			}
			else
				currentController = currentContrib.createRemover(this.siteImpl);
		}

		#endregion

		internal void randomize()
		{
			onPreviewClick(this,null);
		}
		
		protected virtual void updateSize(object sender, System.EventArgs e){
			updatePreview();
		}

		private createCallback callback;

		/// <summary>
		/// Called when a selection of the structure has changed.
		/// </summary>
		protected virtual void updatePreview() 
		{
			using( PreviewDrawer drawer = currentContrib.createPreview( preview.Size) ) {				
				if( previewBitmap!=null )	previewBitmap.Dispose();
				preview.Image = previewBitmap = drawer.createBitmap();
			}
		}


		#region SubGroup
		private class SubGroup : object
		{
			public readonly string name;
			private ArrayList arr;
			public SubGroup(string _name)
			{
				name = _name;
				arr = new ArrayList();
			}

			public void Add(HalfVoxelContribution contrib)
			{
				arr.Add(contrib);
			}

			public int size
			{
				get{ return arr.Count;}
			}

			public HalfVoxelContribution this[int index]
			{
				get
				{
					return (HalfVoxelContribution)arr[index];
				}
			}

			public override string ToString()
			{
				return name;
			}
		}
		#endregion
	}
}

