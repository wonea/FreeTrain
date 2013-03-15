using System;
using nft.core;

namespace nft.core.game
{
	/// <summary>
	/// the interface from World to off game (not on playing) district map.
	/// </summary>
	public interface IOffGameProxy
	{
		object GetSateliteMap(string modename);
		
		/// <summary>
		/// Notify neighboring district was replaced.
		/// </summary>
		/// <param name="old"></param>
		/// <param name="newmap"></param>
		void NotifyNeighborReplaced(IDistrict old, IDistrict newmap);

		/// <summary>
		/// Offer an extend line into <code>offerer</code> to this district.
		/// </summary>
		void OfferExtendLine(IDistrict offerer);
	}
}
