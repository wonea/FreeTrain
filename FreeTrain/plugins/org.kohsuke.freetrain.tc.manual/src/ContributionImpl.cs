using System;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using freetrain.contributions.rail;
using freetrain.framework.plugin;

namespace freetrain.world.rail.manualtc
{
	/// <summary>
	/// Contribution implementation
	/// </summary>
	[Serializable]
	public class TrainConrollerContributionImpl : TrainControllerContribution
	{
		public TrainConrollerContributionImpl( XmlElement e ) : base(e) {}


		public override string name { get { return "Manually-operated diagram"; } }
		//! public override string name { get { return "手動運転ダイヤグラム"; } }

		public override string description {
			get {
				return "Allows you to control the train manually with the keyboard";
				//! return "キーボードから列車を手動で運転できるようにします";
			}
		}

		public override TrainController newController( string name ) {
			return new ManualTrainController(name,this);
		}
	}
}
