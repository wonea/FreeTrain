using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Windows.Forms;
using freetrain.contributions.rail;
using freetrain.framework.plugin;

namespace freetrain.world.rail
{
	// NO THIS DOES NOT WORK.

	/// <summary>
	/// A TrainController implementation that delegates all of the methods
	/// to the TrainController of the parent train group.
	/// </summary>
	[Serializable]
	public class DelegationTrainControllerImpl : TrainController, ISerializable
	{
		public static readonly TrainController theInstance = new DelegationTrainControllerImpl();

		private DelegationTrainControllerImpl() {
			name = "Use diagram settings from the new group";
			//! name = "親グループのダイヤ設定を利用";
		}

		public override JunctionRoute onJunction( Train train, JunctionRailRoad rr ) {
			return findController(train).onJunction(train,rr);
		}

		public override TimeLength getStopTimeSpan( Train train, TrainHarbor harbor, int callCount ) {
			return findController(train).getStopTimeSpan(train,harbor,callCount);
		}

		/// <summary>
		/// Finds the TrainController that can actually do something.
		/// </summary>
		private TrainController findController( TrainItem ti ) {
			while( ti!=null ) {
				if( ti.controller!=theInstance )	return ti.controller;
				ti = ti.ownerGroup;
			}
			// even the root controller delegates to the parent.
			// use the default one
			return SimpleTrainControllerImpl.theInstance;
		}

		public override void config( IWin32Window owner ) {
			MessageBox.Show( owner, "There is no setting item for this diagram", Application.ProductName,
			//! MessageBox.Show( owner, "このダイヤには設定項目はありません", Application.ProductName,
				MessageBoxButtons.OK, MessageBoxIcon.Information );
		}


		// we don't have any contribution and this controller is only used implicitly by default.
		public override TrainControllerContribution contribution { get { return null; } }

	
		
		public void GetObjectData( SerializationInfo info, StreamingContext context) {
			info.SetType(typeof(ReferenceImpl));
		}
		[Serializable]
		internal sealed class ReferenceImpl : IObjectReference {
			public object GetRealObject(StreamingContext context) {
				return theInstance;
			}
		}
	}
}
