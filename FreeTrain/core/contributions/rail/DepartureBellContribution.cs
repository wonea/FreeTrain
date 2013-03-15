using System;
using System.Xml;
using freetrain.contributions.common;
using freetrain.framework;
using freetrain.framework.plugin;
using freetrain.framework.sound;
using freetrain.world;

namespace freetrain.contributions.rail
{
	/// <summary>
	/// Departure bell for trains.
	/// </summary>
	[Serializable]
	public class DepartureBellContribution : Contribution
	{
		public DepartureBellContribution( XmlElement e ) : base(e) {

			name = XmlUtil.selectSingleNode(e,"name").InnerText;

			string href = XmlUtil.selectSingleNode(e,"sound/@href").InnerText;
			sound = new RepeatableSoundEffectImpl(
				ResourceUtil.loadSound( XmlUtil.resolve( e, href )));
		}

		/// <summary> name of this sound </summary>
		public readonly string name;

		/// <summary> Bell sound </summary>
		public readonly SoundEffect sound;

		public override string ToString() {
			return name;
		}
		



		/// <summary> Gets all the departure bell contributions in the system. </summary>
		public static DepartureBellContribution[] all {
			get {
				return (DepartureBellContribution[])
					PluginManager.theInstance.listContributions(typeof(DepartureBellContribution));
			}
		}

		/// <summary> Default bell sound. </summary>
		public static DepartureBellContribution DEFAULT {
			get {
				return (DepartureBellContribution)PluginManager.theInstance
					.getContribution("{9B087AEA-6E9C-48cd-A1F3-1B774500752E}");
			}
		}
	}
}
