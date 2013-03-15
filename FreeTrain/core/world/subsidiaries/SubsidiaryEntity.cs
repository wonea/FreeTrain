using System;

namespace freetrain.world.subsidiaries
{
	public interface SubsidiaryEntity : Entity
	{
		string name { get; }

		/// <summary>
		/// price of the structure
		/// </summary>
		long structurePrice { get; }

		/// <summary>
		/// Sum of the land prices.
		/// </summary>
		long totalLandPrice { get; }
		
		/// <summary>
		/// Returns the location such that the returned value <code>v</code>
		/// will satisfy <code>World.world[v].entity==this</code>.
		/// 
		/// It is desirable for this method to return the location close to the
		/// center of the entity.
		/// </summary>
		Location locationClue { get; }
	}
}
