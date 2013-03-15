using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using freetrain.contributions.rail;
using freetrain.framework.plugin;
using freetrain.views.map;
using freetrain.world;
using freetrain.world.rail;

namespace freetrain.controllers.rail
{
	/// <summary>
	/// Controller to place/remove BridgeRails
	/// </summary>
	public class SpecialPurposeRailController : AbstractLineController
	{
		public SpecialPurposeRailController( SpecialRailContribution type ) : base(type) {}

		protected override void draw( Direction d, DrawContextEx canvas, Point pt ) {
			RailPattern.get( d, d.opposite ).drawAlpha( canvas.surface, pt );
		}
	}
}
