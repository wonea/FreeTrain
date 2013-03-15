using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.views;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.rail;
using freetrain.framework;
using freetrain.framework.sound;
using freetrain.util.controls;
using freetrain.util.command;
using org.kohsuke.directdraw;

namespace freetrain.controllers.rail
{
	/// <summary>
	/// Controller that allows the user to
	/// place/remove trains.
	/// </summary>
	public class TrainPlacementController : AbstractControllerImpl, MapOverlay, LocationDisambiguator
	{
		#region singleton instance management
		/// <summary>
		/// Creates a new controller window, or active the existing one.
		/// </summary>
		public static void create() {
			if(theInstance==null)
				theInstance = new TrainPlacementController();
			theInstance.Show();
			theInstance.Activate();
		}

		private System.Windows.Forms.ComboBox controllerCombo;
		private System.Windows.Forms.MenuItem miSell;

		private static TrainPlacementController theInstance;

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			base.OnClosing(e);
			theInstance = null;
		}
		#endregion





		public TrainPlacementController() {
			// Windows フォーム デザイナ サポートに必要です。
			InitializeComponent();

			controllerCombo.DataSource = World.world.trainControllers;
			tree.ItemMoved = new ItemMovedHandler(onItemDropped);

			this.FormBorderStyle = FormBorderStyle.SizableToolWindow;

			reset();

			// register command manager
			new Command( commands )
				.addUpdateHandler( new CommandHandler(updateAddGroup) )
				.addExecuteHandler( new CommandHandlerNoArg(executeAddGroup) )
				.commandInstances.AddAll( miAddGroup );
			new Command( commands )
				.addUpdateHandler( new CommandHandler(updateSell) )
				.addExecuteHandler( new CommandHandlerNoArg(executeSell) )
				.commandInstances.AddAll( miSell );
		}

		private readonly CommandManager commands = new CommandManager();

		/// <summary>
		/// Location of the arrow.
		/// </summary>
		private readonly LocationStore arrowLoc = new LocationStore();


		/// <summary>
		/// Resets the contents of the list.
		/// </summary>
		private void reset() {
			tree.BeginUpdate();
			tree.Nodes.Clear();

			TreeNode root = createNode( World.world.rootTrainGroup );
			tree.Nodes.Add(root);
			populate( World.world.rootTrainGroup, root.Nodes );
			
			tree.ExpandAll();
			tree.EndUpdate();
		}

		private TreeNode createNode( TrainItem item ) {
			DDTreeNode n = new DDTreeNode( item.name );
			n.Tag = item;
			n.canAcceptDrop = (item is TrainGroup);
			n.ImageIndex = n.SelectedImageIndex = (item is Train)?2:0;
			return n;
		}

		// populate a tree control with trains
		private void populate( TrainGroup group, TreeNodeCollection col ) {
			foreach( TrainItem ti in group.items ) {
				TreeNode node = createNode(ti);
				col.Add(node);
				if( ti is TrainGroup )
					populate( (TrainGroup)ti, node.Nodes );
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
		private DDTreeView tree;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label typeBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ContextMenu treeMenu;
		private System.Windows.Forms.MenuItem miAddGroup;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.RadioButton buttonRemove;
		private System.Windows.Forms.RadioButton buttonPlace;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainPlacementController));
			this.buttonRemove = new System.Windows.Forms.RadioButton();
			this.buttonPlace = new System.Windows.Forms.RadioButton();
			this.tree = new freetrain.util.controls.DDTreeView();
			this.treeMenu = new System.Windows.Forms.ContextMenu();
			this.miAddGroup = new System.Windows.Forms.MenuItem();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.panel2 = new System.Windows.Forms.Panel();
			this.typeBox = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.controllerCombo = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.nameBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.miSell = new System.Windows.Forms.MenuItem();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemove.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonRemove.Location = new System.Drawing.Point(187, 182);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(89, 26);
			this.buttonRemove.TabIndex = 5;
			this.buttonRemove.Text = "Remove";
			//! this.buttonRemove.Text = "撤去";
			this.buttonRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.buttonRemove.CheckedChanged += new System.EventHandler(this.onModeChange);
			// 
			// buttonPlace
			// 
			this.buttonPlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonPlace.Appearance = System.Windows.Forms.Appearance.Button;
			this.buttonPlace.Checked = true;
			this.buttonPlace.Location = new System.Drawing.Point(110, 182);
			this.buttonPlace.Name = "buttonPlace";
			this.buttonPlace.Size = new System.Drawing.Size(71, 26);
			this.buttonPlace.TabIndex = 4;
			this.buttonPlace.TabStop = true;
			this.buttonPlace.Text = "Place";
			//! this.buttonPlace.Text = "配置";
			this.buttonPlace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.buttonPlace.CheckedChanged += new System.EventHandler(this.onModeChange);
			// 
			// tree
			// 
			this.tree.AllowDrop = true;
			this.tree.ContextMenu = this.treeMenu;
			this.tree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tree.FullRowSelect = true;
			this.tree.ImageIndex = 0;
			this.tree.ImageList = this.imageList;
			this.tree.Location = new System.Drawing.Point(0, 0);
			this.tree.Name = "tree";
			this.tree.SelectedImageIndex = 0;
			this.tree.Size = new System.Drawing.Size(176, 220);
			this.tree.TabIndex = 1;
			this.tree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.onNodeExpanded);
			this.tree.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.onNodeCollapsed);
			this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.onTrainChange);
			this.tree.DoubleClick += new System.EventHandler(this.onDoubleClick);
			// 
			// treeMenu
			// 
			this.treeMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.miAddGroup,
																					 this.miSell});
			this.treeMenu.Popup += new System.EventHandler(this.treeMenu_Popup);
			// 
			// miAddGroup
			// 
			this.miAddGroup.Index = 0;
			this.miAddGroup.Text = "&Add new group";
			//! this.miAddGroup.Text = "新しいグループの追加(&A)";
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "");
			this.imageList.Images.SetKeyName(1, "");
			this.imageList.Images.SetKeyName(2, "");
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.typeBox);
			this.panel2.Controls.Add(this.label3);
			this.panel2.Controls.Add(this.controllerCombo);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.nameBox);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.buttonPlace);
			this.panel2.Controls.Add(this.buttonRemove);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel2.Location = new System.Drawing.Point(176, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(284, 220);
			this.panel2.TabIndex = 3;
			// 
			// typeBox
			// 
			this.typeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.typeBox.Enabled = false;
			this.typeBox.Location = new System.Drawing.Point(90, 78);
			this.typeBox.Name = "typeBox";
			this.typeBox.Size = new System.Drawing.Size(186, 101);
			this.typeBox.TabIndex = 13;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 73);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(78, 22);
			this.label3.TabIndex = 12;
			this.label3.Text = "&Type:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			//! this.label3.Text = "種類(&T)：";
			// 
			// controllerCombo
			// 
			this.controllerCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.controllerCombo.Enabled = false;
			this.controllerCombo.Location = new System.Drawing.Point(90, 43);
			this.controllerCombo.Name = "controllerCombo";
			this.controllerCombo.Size = new System.Drawing.Size(166, 21);
			this.controllerCombo.TabIndex = 3;
			this.controllerCombo.SelectedIndexChanged += new System.EventHandler(this.controllerCombo_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 43);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(81, 21);
			this.label2.TabIndex = 10;
			this.label2.Text = "&Diagram:";
			//! this.label2.Text = "ダイヤ(&T)：";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// nameBox
			// 
			this.nameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.nameBox.Enabled = false;
			this.nameBox.Location = new System.Drawing.Point(90, 9);
			this.nameBox.Name = "nameBox";
			this.nameBox.Size = new System.Drawing.Size(186, 20);
			this.nameBox.TabIndex = 2;
			this.nameBox.TextChanged += new System.EventHandler(this.onNameChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 21);
			this.label1.TabIndex = 8;
			this.label1.Text = "&Name:";
			//! this.label1.Text = "名前(&N)：";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tree);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(176, 220);
			this.panel1.TabIndex = 7;
			// 
			// miSell
			// 
			this.miSell.Index = 1;
			this.miSell.Text = "&Sell...";
			//! this.miSell.Text = "売却(&S)...";
			// 
			// TrainPlacementController
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(460, 220);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MinimumSize = new System.Drawing.Size(416, 195);
			this.Name = "TrainPlacementController";
			this.Text = "Place Train";
			//! this.Text = "車両の配置";
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private bool isPlacingMode { get { return buttonPlace.Checked; } }

		private TrainItem selectedItem {
			get {
				TreeNode n = tree.SelectedNode;
				if(n==null)			return null;
				return (TrainItem)n.Tag;
			}
		}
		/// <summary>
		/// Gets the currently selected train, if any
		/// </summary>
		private Train selectedTrain {
			get {
				TrainItem ti = selectedItem;
				if(ti is Train)		return (Train)ti;
				else				return null;
			}
		}
		/// <summary>
		/// Gets the currently selected group, if any.
		/// </summary>
		private TrainGroup selectedGroup {
			get {
				TrainItem ti = selectedItem;
				if(ti is TrainGroup)	return (TrainGroup)ti;
				else					return null;
			}
		}

		/// <summary>
		/// Re-computes the arrow location correctly
		/// </summary>
		private void resetArrowLocation() {
			Train tr = this.selectedTrain;
			if(tr==null || !tr.head.state.isInside ) {
				arrowLoc.location = world.Location.UNPLACED;
			} else {
				arrowLoc.location = tr.head.state.asInside().location;
			}
		}

		public override void onClick( MapViewWindow source, Location loc, Point ab ) {
			if(isPlacingMode) {
				if( this.selectedTrain==null )	return;

				// place
				Train tr = this.selectedTrain;
				if(tr.isPlaced) {
					// see if the user has clicked the same train
					Car c = Car.get(loc);
					if(c is Train.TrainCar && ((Train.TrainCar)c).parent==tr) {
						// clicking the same train will be considered to reverse its direction
						// and change the position of arrow
						tr.reverse();
						resetArrowLocation();
						return;
					} else {
						MainWindow.showError("This train is already placed");
						//; MainWindow.showError("配置済みです");
						return;
					}
				}

				RailRoad rr = RailRoad.get(loc);
				if(rr==null) {
					MainWindow.showError("Can not place without tracks");
					//; MainWindow.showError("線路のないところには配置できません");
					return;
				}

				if(!tr.place(loc)) {
					MainWindow.showError("Can not place");
					//! MainWindow.showError("配置できません");
				} else
					playSound();

			} else {
				// remove
				RailRoad rr = RailRoad.get(loc);
				if(rr==null) {
					MainWindow.showError("There are no tracks");
					//! MainWindow.showError("線路がありません");
					return;
				}
				if(!(rr.voxel.car is Train.TrainCar)) {
					MainWindow.showError("There are no cars");
					//! MainWindow.showError("車両がありません");
					return;
				}
				((Train.TrainCar)rr.voxel.car).parent.remove();
				playSound();
				// successfully removed
			}
		}

		private void playSound() {
			SoundEffectManager.PlayAsynchronousSound(
				ResourceUtil.findSystemResource("vehiclePurchase.wav") );
		}

		public override void onAttached() {
			reset();
		}

		// place or remove button is clicked.
		private void onModeChange(object sender, EventArgs e) {
		}

		// when a selected train is changed
		private void onTrainChange(object sender, TreeViewEventArgs e) {

			// update controls
			if( selectedItem!=null ) {
				nameBox.Enabled = true;
				nameBox.Text = selectedItem.name;
				controllerCombo.Enabled = true;
				controllerCombo.SelectedItem = selectedItem.controller;
			} else {
				nameBox.Enabled = false;
				controllerCombo.Enabled = false;
				controllerCombo.SelectedIndex=-1;	// unset selection
			}

			if( selectedTrain!=null ) {
				typeBox.Enabled = true;
				typeBox.Text = selectedTrain.type.name;
			} else {
				typeBox.Enabled = false;
				typeBox.Text = "";
			}
			
			resetArrowLocation();
		}

		public override LocationDisambiguator disambiguator { get { return this; } }

		public bool isSelectable(Location loc) {
			RailRoad rr = RailRoad.get(loc);
			if(rr==null)	return false;

			if( isPlacingMode ) {
				if(rr.voxel.car==null)	return true;
				
				Train.TrainCar car = rr.voxel.car as Train.TrainCar;
				if( car!=null && car.parent == selectedTrain )
					// allow selecting the same train to reverse the direction
					return true;

				return false;
			} else {
				return rr.voxel.car!=null;
			}
		}



		public void drawBefore( QuarterViewDrawer view, DrawContextEx surface ) {}

		public void drawVoxel( QuarterViewDrawer view, DrawContextEx canvas, Location loc, Point pt ) {}

		public void drawAfter( QuarterViewDrawer view, DrawContextEx dc ) {
			Train tr = this.selectedTrain;
			if(tr==null || !tr.head.state.isInside)	return;

			// draw an arrow that indicates the direction of the train
			CarState.Inside ci = tr.head.state.asInside();

			Point pt = view.fromXYZToClient(ci.location);
			pt.Y -= 12;

			ci.direction.drawArrow( dc.surface, pt );
		}

		private void onDoubleClick(object sender, EventArgs e) {
		
		}

		private void onNameChanged(object sender, EventArgs e) {
			string name = nameBox.Text;
			selectedItem.name = name;
			tree.SelectedNode.Text = name;
		}

		private TreeNode rightSelectedItem;

		// this method is called before the context menu pops up.
		private void treeMenu_Popup(object sender, System.EventArgs e) {
			rightSelectedItem = tree.GetNodeAt(tree.PointToClient(Cursor.Position));
			commands.updateAll();
		}

		private void onNodeExpanded(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			e.Node.SelectedImageIndex = e.Node.ImageIndex = 1;
		}

		private void onNodeCollapsed(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			e.Node.SelectedImageIndex = e.Node.ImageIndex = 0;
		}



		private void onItemDropped(TreeNode node, TreeNode newParent) {
			// move this train under a new group.
			node.Remove();
			newParent.Nodes.Add( node );

			TrainItem item = (TrainItem)node.Tag;
			TrainGroup newGroup = (TrainGroup)newParent.Tag;

			item.moveUnder(newGroup);
		}

		private void controllerCombo_SelectedIndexChanged(object sender, System.EventArgs e) {
			TrainItem item = selectedItem;
			if( item!=null )
				item.controller = (TrainController)controllerCombo.SelectedItem;
		}


		
		

		private void updateAddGroup( Command cmd ) {
			cmd.Enabled = ( rightSelectedItem!=null && rightSelectedItem.Tag is TrainGroup );
		}

		private void executeAddGroup() {
			TrainGroup newGroup = new TrainGroup( (TrainGroup)rightSelectedItem.Tag );
			TreeNode node = createNode(newGroup);
			rightSelectedItem.Nodes.Add(node);
			tree.SelectedNode = node;
		}

		private void updateSell( Command cmd ) {
			bool b = false;
			if( rightSelectedItem!=null ) {
				Train tr = rightSelectedItem.Tag as Train;
				if( tr!=null && !tr.isPlaced )
					b = true;
			}

			cmd.Enabled = b;
		}

		private void executeSell() {
			if( MessageBox.Show(this,"Do you want to sell this train?",Application.ProductName,
			//! if( MessageBox.Show(this,"売却しますか？",Application.ProductName,
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question) == DialogResult.Yes) {

				Train tr = (Train)rightSelectedItem.Tag;
				tr.sell();

				rightSelectedItem.Remove();
			}
		}
	}
}
