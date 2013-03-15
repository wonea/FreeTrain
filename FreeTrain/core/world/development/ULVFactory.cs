using System;

namespace freetrain.world.development
{
	/// <summary>
	/// Factory of ULV.
	/// 
	/// ULV is a proper function of Cube and time. Often, we compute
	/// a lot of ULVs by fixing a time, so it can be easily cached
	/// for improved performance.
	/// 
	/// ULVFactory hides the caching detail.
	/// </summary>
	public interface ULVFactory
	{
		ULV create( Cube cube );
	}
}
