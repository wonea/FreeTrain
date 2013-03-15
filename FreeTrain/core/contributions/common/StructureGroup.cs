using System;
using System.Collections;
using System.ComponentModel;

namespace freetrain.contributions.common
{
	/// <summary>
	/// Group of StructureContributions.
	/// </summary>
	public class StructureGroup : CollectionBase
	{
		public StructureGroup( string _name ) {
			this.name = _name;
		}

		/// <summary> Name of this group. </summary>
		public string name;


		public void add( StructureContribution sc ) {
			base.List.Add(sc);
		}
		public StructureContribution get( int idx ) {
			return (StructureContribution)base.List[idx];
		}
		public void remove( StructureContribution sc ) {
			base.List.Remove(sc);
		}

		public override string ToString() { return name; }
	}

	/// <summary>
	/// Group of StructureGroups.
	/// 
	/// This object implements IListSource to support data-binding.
	/// </summary>
	public class StructureGroupGroup : IListSource
	{
		public StructureGroupGroup() {}

		protected readonly Hashtable core = new Hashtable();
		// used for data-binding
		protected readonly ArrayList list = new ArrayList();

		public StructureGroup this[ string name ] {
			get {
				StructureGroup g = (StructureGroup)core[name];
				if(g==null) {
					core[name] = g = new StructureGroup(name);
					list.Add(g);
				}

				return g;
			}
		}

		public IEnumerator getEnumerator() {
			return core.Values.GetEnumerator();
		}

		public IList GetList() { return list; }
		public bool ContainsListCollection { get { return true; } }
	}
}
