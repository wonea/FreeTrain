using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using nft.framework;
using nft.framework.plugin;
using nft.ui.mainframe;
using nft.core.schedule;
using nft.util;

namespace nft.core.game
{
	public interface IGame
	{
		void Start();
		void Close();
		// if false, no need to save the game.
		bool Modified {get;}
		Clock Clock {get;}
		IClimateController ClimateController {get;}
		string Name {get;set;}
		//World world {get;}
	}
}
