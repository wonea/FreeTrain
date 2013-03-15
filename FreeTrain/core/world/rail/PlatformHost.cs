using System;

namespace freetrain.world.rail
{
	/// <summary>
	/// A structure that can host platforms.
	/// 
	/// Nodes added to this host will be notified when
	/// the host is destroied.
	/// </summary>
	public interface PlatformHost : Entity
	{
		void addNode( Platform platform );
		void removeNode( Platform platform );

		Station hostStation { get; }
	}
}
