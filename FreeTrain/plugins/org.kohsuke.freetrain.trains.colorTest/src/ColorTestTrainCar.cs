using System;
using System.Drawing;
using System.Xml;
using freetrain.framework.plugin;
using freetrain.contributions.train;

namespace freetrain.world.rail.cttrain
{
	/// <summary>
	/// TrainCar contribution implementation for ColorTestTrain.
	/// </summary>
	[Serializable]
	public class ColorTestTrainCar : ColoredTrainCarImpl
	{
		public ColorTestTrainCar( XmlElement e ) :
			base(
				ColoredTrainPictureContribution.list()[0],
				Color.Red, Color.Blue, Color.Green, Color.Yellow,
				e.Attributes["id"].Value,0) {

			theInstance = this;
		}

		public static ColorTestTrainCar theInstance;
	}
}
