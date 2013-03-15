using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Windows.Forms;
using freetrain.framework.plugin;
using freetrain.contributions.rail;

namespace freetrain.world.rail
{
	/// <summary>
	/// Default TrainController implementation that doesn't do anything
	/// interesting.
	/// </summary>
	[Serializable]
	public class SimpleTrainControllerImpl : TrainController, ISerializable
	{
		public static readonly TrainController theInstance = new SimpleTrainControllerImpl();

		private SimpleTrainControllerImpl() {
			name = "Default Diagram";
			//! name = "ディフォルトダイヤグラム";
		}

		public override JunctionRoute onJunction( Train train, JunctionRailRoad rr ) {
			return JunctionRoute.Straight;
		}

		public override TimeLength getStopTimeSpan( Train train, TrainHarbor harbor, int callCount ) {
			// stop 1 hour and go
			if(!(harbor is Station))	return TimeLength.ZERO;	// ignore everything but a station

			if( callCount==0 )	return TimeLength.fromMinutes(30);
			else				return TimeLength.ZERO;
		}

		public override void config( IWin32Window owner ) {
			MessageBox.Show( owner, "This diagram has no setting item", Application.ProductName,
			//! MessageBox.Show( owner, "このダイヤには設定項目はありません", Application.ProductName,
				MessageBoxButtons.OK, MessageBoxIcon.Information );
		}


		// we don't have any contribution and this controller is only used implicitly by default.
		public override TrainControllerContribution contribution { get { return null; } }

//		/// <summary> The sole instance of the contribution. </summary>
//		private static TrainControllerContribution theContribution = new SimpleTrainControllerContribution();
//
//		private class SimpleTrainControllerContribution : TrainControllerContribution {
//		}

	
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
