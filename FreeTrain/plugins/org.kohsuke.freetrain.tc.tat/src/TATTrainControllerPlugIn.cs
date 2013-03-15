using System;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using freetrain.contributions.rail;
using freetrain.framework.plugin;

namespace freetrain.world.rail.tattc
{
	/// <summary>
	/// TATTrainControllerPlugIn の概要の説明です。
	/// </summary>
	[Serializable]
	public class TATTrainControllerPlugIn : TrainControllerContribution
	{
		public TATTrainControllerPlugIn( XmlElement e ) : base(e) {
			theInstance = this;
		}

		internal static TATTrainControllerPlugIn theInstance;

		public override string name { get { return "A-Train style train controller"; } }
		//! public override string name { get { return "「Ａ列車で行こう」式ダイヤグラム"; } }

		public override string description {
			get {
				return "Create diagrams that control departure times for each station and directions at each point";
				//! return "各駅の発車時刻と各ポイントでの進行方向を設定することによってダイヤを設定します";
			}
		}

		public override TrainController newController( string name ) {
			return new TATTrainController(name);
		}
	}
}
