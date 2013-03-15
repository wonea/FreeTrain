using System;
using nft.core.geometry;

namespace nft.core.structure
{
	[Serializable]
	public class Lot
	{
		public Lot(string category, int level, Rect3D bounds )
		{
			Category = category;
			Level = level;
			Bounds = bounds;
		}

		public Lot(IStructureTemplate structure, Location loc )
		{
			ID = structure.ID;
			Level = structure.Level;
			Category = structure.Category;
			Bounds = new Rect3D(loc,structure.Size);
		}

		public readonly string ID;
		public readonly object Category;
		public readonly int Level;
		public readonly Rect3D Bounds;
	}
}
