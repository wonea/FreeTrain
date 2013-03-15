using System;
using System.Collections;
using System.ComponentModel;
using freetrain.contributions.common;

namespace freetrain.contributions.land
{
	/// <summary>
	/// Group of LandContribution.
	/// </summary>
	public class LandBuilderGroup : StructureGroup
	{
		public LandBuilderGroup( string _name ) : base(_name) {
		}

		public new LandBuilderContribution get( int idx ) {
			return (LandBuilderContribution)base.List[idx];
		}
		public void remove( LandBuilderContribution sc ) {
			base.List.Remove(sc);
		}

		public override string ToString() { return name; }
	}

	/// <summary>
	/// Group of LandGroup.
	/// 
	/// This object implements IListSource to support data-binding.
	/// </summary>
	public class LandBuilderGroupGroup : StructureGroupGroup
	{
		public LandBuilderGroupGroup() {}

		public new LandBuilderGroup this[ string name ] {
			get {
				LandBuilderGroup g = (LandBuilderGroup)core[name];
				if(g==null) {
					core[name] = g = new LandBuilderGroup(name);
					list.Add(g);
				}

				return g;
			}
		}
	}
}
