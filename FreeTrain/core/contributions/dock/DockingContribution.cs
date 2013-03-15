using System;
using System.Windows.Forms;
using System.Xml;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.util.docking;
using freetrain.util.command;

namespace freetrain.contributions.dock
{
	/// <summary>
	/// Contributes a docking pane to the system.
	/// 
	/// This class has two modes. In the singleton mode, only one
	/// instance of the panel is allowed. We keep one Content instance
	/// alive all the time and show/hide it appropriately.
	/// 
	/// In the non-singleton mode, many instances of the panel are
	/// allowed. We keep a fixed number of persistent instances
	/// (so that window location/state is restored correctly.)
	/// If the user asks to open more instance of the panel,
	/// <c>SuicidalContent</c> is used to launch one time only panel.
	/// </summary>
	public class DockingContribution : Contribution
	{
		/// <summary>
		/// True if only one instance of the panel is allowed.
		/// </summary>
		private readonly bool singleton;

		/// <summary> Menu item that invokes this view. </summary>
		private readonly MenuItem menuItem;

		/// <summary>
		/// The sole instance of the content. This field is used
		/// when in the singleton mode.
		/// </summary>
		private readonly ContentEx content;

		/// <summary>
		/// Persistent content instances. This field is used
		/// when in the non-singleton mode.
		/// </summary>
		private readonly ContentEx[] contents;

		/// <summary>
		/// Type of the control associated with this the content.
		/// </summary>
		private readonly Type windowType;

		/// <summary>
		/// Sequence number generator.
		/// </summary>
		private int ticketMaster = 1;

		/// <summary>
		/// Name of the panel.
		/// </summary>
		private string name;


		public DockingContribution( XmlElement e ) : base("dockingContent",e.Attributes["id"].Value) {

			singleton = (e.SelectSingleNode("multiple")==null);

			name = XmlUtil.selectSingleNode(e,"name").InnerText;
			windowType = PluginUtil.loadTypeFromManifest((XmlElement)XmlUtil.selectSingleNode(e,"class"));

			XmlElement menuElement = (XmlElement)XmlUtil.selectSingleNode(e,"menu");

			// create a content object
			if( singleton ) {
				content = createEconomicalContent(name);
				contents = null;
			} else {
				content = null;
				contents = new ContentEx[5];
				for( int i=0; i<contents.Length; i++ )
					contents[i] = createEconomicalContent(name+(ticketMaster++).ToString());
			}

			// set up a menu item
			menuItem = new MenuItem( XmlUtil.selectSingleNode(menuElement,"@name").InnerText );
			new Command( MainWindow.mainWindow.commands )
				.addExecuteHandler( new CommandHandlerNoArg(toggle) )
				.addUpdateHandler(  new CommandHandler(onUpdate) )
				.commandInstances.Add( menuItem );

			MenuItem parent = MenuItemConstants.parse(
				XmlUtil.selectSingleNode(menuElement,"@location").InnerText ).menuItem;
			if( menuElement.Attributes["position"]==null )
				parent.MenuItems.Add(menuItem);
			else
				parent.MenuItems.Add( int.Parse(menuElement.Attributes["position"].Value), menuItem );
		}

		private ContentEx createEconomicalContent( string name ) {
			ContentEx c = new EconomicalContentImpl2(
				MainWindow.mainWindow.dockingManager,
				name,
				-1,
				new EconomicalContentImpl2.WindowCreator(createControl) );
			MainWindow.mainWindow.dockingManager.Contents.Add(c);

			return c;
		}

		private ContentEx createTemporaryContent() {
			ContentEx c = new SuicidalContent(
				MainWindow.mainWindow.dockingManager,
				name+(ticketMaster++).ToString(),
				-1 );

			Control ctrl = createControl();
			c.Control = ctrl;
			if( ctrl is IDockingWindow )
				((IDockingWindow)ctrl).setSite(c);
			c.FullTitle = c.Control.Text;

			MainWindow.mainWindow.dockingManager.Contents.Add(c);

			return c;
		}
		
		/// <summary>
		/// Show or hide the docking window, depending on the current visibility.
		/// </summary>
		public void toggle() {
			if(!singleton)
				show();	// always add a new window
			else {
				if(content.Visible)
					content.hide();
				else
					content.show();
			}
		}

		/// <summary>
		/// Show the docking window.
		/// </summary>
		public void show() {
			if(singleton)
				content.show();
			else {
				for( int i=0; i<contents.Length; i++ )
					if( !contents[i].Visible ) {
						contents[i].show();
						return;
					}

				// all the persistent contents are shown. launch a temporary one
				createTemporaryContent().show();
			}
		}

		private void onUpdate( Command cmd ) {
			if( singleton )
				// if not singleton, don't show a check mark.
				// apply a check if it's already displayed.
				cmd.Checked = (content.ParentWindowContent!=null);
		}

		private Control createControl() {
			return (Control)Activator.CreateInstance(windowType);
		}

		public class EconomicalContentImpl2 : EconomicalContent
		{
			public delegate Control WindowCreator();

			private readonly WindowCreator creator;

			public EconomicalContentImpl2( DockingManagerEx owner, string title, int imageIndex, WindowCreator _creator )
				: base(owner,title,imageIndex) {

				this.creator = _creator;
			}

			protected override Control createControl() {
				Control c = creator();
				if( c is IDockingWindow )
					((IDockingWindow)c).setSite(this);
				this.FullTitle = c.Text;
				return c;
			}
		}
	}
}
