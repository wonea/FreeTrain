using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using org.kohsuke.directdraw;
using freetrain.controllers;
using freetrain.contributions.population;
using freetrain.contributions.land;
using freetrain.framework.graphics;
using freetrain.framework.plugin;
using freetrain.views;
using freetrain.world;

namespace freetrain.contributions.fence
{
	[Serializable]
	public class FenceRemover : FenceBuilder 
	{

		public FenceRemover( XmlElement e ) : base(e) 
		{
		}

		protected override void setFence(Location loc, Direction d )
		{
			if( World.world[loc] != null ) {
				World.world[loc].setFence(d,null);
				World.world.onVoxelUpdated(loc);
			}
		}
	}
}
