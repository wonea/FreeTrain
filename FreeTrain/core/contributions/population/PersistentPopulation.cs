using System;
using System.Runtime.Serialization;
using freetrain.world;

namespace freetrain.contributions.population
{
	/// <summary>
	/// Population implementation that wraps another Population and
	/// provides persistence support.
	/// 
	/// During deserialization, reference to this object is re-connected
	/// to the existing PersistentPopulation object.
	/// </summary>
	[Serializable]
	public class PersistentPopulation : Population, ISerializable
	{
		private readonly Population core;

		/// <summary>
		/// Object used to restore the reference to this Population object.
		/// </summary>
		private IObjectReference resolver;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="_core"></param>
		/// <param name="_ref">
		/// Object that returns a reference to the existing same object.
		/// This object needs to be serializable, and will be used to
		/// restore reference correctly.
		/// </param>
		public PersistentPopulation( Population _core, IObjectReference _ref ) {
			this.core = _core;
			this.resolver = _ref;
		}

		public override int residents { get { return core.residents; } }

		public override int calcPopulation( Time currentTime ) {
			return core.calcPopulation(currentTime);
		}

		//
		// serialization
		//
		private void setResolver( IObjectReference resolver ) {
			this.resolver = resolver;
		}


		public virtual void GetObjectData( SerializationInfo info, StreamingContext context ) {
			info.SetType(typeof(SerializationHelper));
			// it seems to me that the resolver is fully resolved to the target object
			// before it's assigned to the reference field.
			// so the type of the reference field is Population, not IObjectReference to
			// a Population.
			info.AddValue("reference",resolver);
		}

		[Serializable]
		internal class SerializationHelper : IObjectReference {
			private Population reference=null;
			public object GetRealObject(StreamingContext context) {
				return reference;
			}
		}
	}
}
