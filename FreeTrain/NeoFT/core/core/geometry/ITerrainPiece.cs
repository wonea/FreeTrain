using System;

namespace nft.core.geometry
{
	/// <summary>
	/// TerrainPiece の概要の説明です。
	/// </summary>
	public interface ITerrainPiece 
	{

		/// <summary>
		/// minimum height between the each vertices.
		/// </summary>
		int BaseHeight { get; set; }

		int AverageHeight { get; }

		int MaxHeight { get; }

		bool Convex{ get; set; }

		/// <summary>
		/// returns vertex height at specified direction.
		/// </summary>
		int this[ InterCardinalDirection dir ]{ get; }

		/// <summary>
		/// returns vertex height at specified direction.
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		int getHeight( InterCardinalDirection dir );

		/// <summary>
		/// returns vertex offset height at specified direction.
		/// the value is offset from BaseHeight
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		int getSlopeHeight( InterCardinalDirection dir );

		/// <summary>
		/// raise the vertex.
		/// </summary>
		/// <param name="dir"></param>
		void vertexUp( InterCardinalDirection dir );

		/// <summary>
		/// lower the vertex.
		/// </summary>
		/// <param name="ne"></param>
		/// <param name="se"></param>
		/// <param name="sw"></param>
		/// <param name="nw"></param>
		void vertexDown( InterCardinalDirection dir );

		/// <summary>
		/// set all the vertex at once.
		/// given value must be offset from BaseHeight.
		/// given values might be corrected to form regular pattern of slope.
		/// </summary>
		/// <param name="ne"></param>
		/// <param name="se"></param>
		/// <param name="sw"></param>
		/// <param name="nw"></param>
		void setSlope( int ne, int se, int sw, int nw );
	}
}
