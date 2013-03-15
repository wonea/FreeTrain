using System;
using System.Collections;

namespace freetrain.world.rail
{
	/// <summary>
	/// Group of trains and child train groups.
	/// </summary>
	[Serializable]
	public class TrainGroup : TrainItem
	{
		public TrainGroup(TrainGroup group,string name) : base(group,name) {}
		public TrainGroup(TrainGroup group) : this(group,string.Format("Group {0}",iota++)) {
		//! public TrainGroup(TrainGroup group) : this(group,string.Format("グループ{0}",iota++)) {
			controller = DelegationTrainControllerImpl.theInstance;
		}
		private static int iota=1;


		public readonly TrainCollection items = new TrainCollection();

		[Serializable]
		public class TrainCollection : CollectionBase {
			public void add( TrainItem t ) {
				base.List.Add(t);
			}
			public TrainItem get( int idx ) {
				return (TrainItem)base.List[idx];
			}
			public void remove( TrainItem t ) {
				base.List.Remove(t);
			}
		}
	}
}
