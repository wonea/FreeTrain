using System;
using System.Xml;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.world;

namespace freetrain.contributions.others
{
	/// <summary>
	/// Creates a new empty game by allowing the user to specify the size of the world.
	/// </summary>
	public class EmptyNewGameContributionImpl : NewGameContribution
	{
		public EmptyNewGameContributionImpl( XmlElement e ) : base(e) {}

		public override string author { get { return "-"; } }
		public override string name { get { return "Empty map"; } }
		//! public override string name { get { return "空マップ"; } }
		public override string description { get { return "Create a completely empty map"; } }
		//! public override string description { get { return "何もない空のマップを作成します"; } }
		
		public override World createNewGame() {
			using( NewWorldDialog dialog = new NewWorldDialog() ) {
				if(dialog.ShowDialog(MainWindow.mainWindow)==DialogResult.OK)
					return dialog.createWorld();
				else
					return null;
			}
		}
	}
}
