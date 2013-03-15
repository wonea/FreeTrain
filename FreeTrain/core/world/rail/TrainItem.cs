using System;

namespace freetrain.world.rail
{
	/// <summary>
	/// Common aspect of Train and TrainGroup.
	/// </summary>
	[Serializable]
	public abstract class TrainItem
	{
		protected TrainItem( TrainGroup group, string _name ) {
			this._ownerGroup = group;
			this.name=_name;
			if(group!=null)
				group.items.add(this);
		}

		/// <summary> Display name of this train. </summary>
		public string name;

		/// <summary>
		/// TrainController that controls this train/train group.
		/// 
		/// TrainController for train groups are not directly used.
		/// Rather, they will be used only when the train controller
		/// that delegates the call to its parent is used.
		/// </summary>
		public TrainController controller;

		private TrainGroup _ownerGroup;
		
		/// <summary> TrainGroup to which this train belong. </summary>
		public TrainGroup ownerGroup { get { return _ownerGroup; } }

		/// <summary> Move this group to a new train group </summary>
		public void moveUnder( TrainGroup newGroup ) {
			if(ownerGroup!=null)
				_ownerGroup.items.remove(this);
			
			_ownerGroup = newGroup;
			_ownerGroup.items.add(this);
		}
	}
}
