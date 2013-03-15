using System;
using System.Xml.Serialization;
using DirectShowLib;
using freetrain.framework;

namespace freetrain.tools.vcr
{
	/// <summary>
	/// Persistent settins of VCR
	/// </summary>
	public class VCROptions : PersistentOptions
	{
		/// <summary>
		/// Video frame ratio per second.
		/// </summary>
		public int fps = 5;

		/// <summary>
		/// Taking a snapshot every N minutes in the game time.
		/// </summary>
		public int period = 60;

		/// <summary>
		/// Moniker to the video compressor object.
		/// </summary>
		public CompressorMoniker compressor = null;

		public new VCROptions load() {
			return (VCROptions)base.load();
		}
	}
}
