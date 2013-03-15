using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using freetrain.contributions.rail;
using freetrain.framework;
using freetrain.framework.plugin;

namespace freetrain.world.rail.manualtc
{
	/// <summary>
	/// Controller-based manual train controller
	/// </summary>
	[Serializable]
	internal class ManualTrainController : TrainController
	{
		public ManualTrainController( string _name, TrainControllerContribution _parent ) {
			this.name = _name;
			this.parent = _parent;
		}

		private readonly TrainControllerContribution parent;

		public override TrainControllerContribution contribution {
			get {
				return parent;
			}
		}

		[DllImport("User32.dll")]
		private static extern short GetAsyncKeyState( int vKey );

		// virtual key code (see Platform SDK)
		private const int VK_LEFT = 0x25;
		private const int VK_UP = 0x26;
		private const int VK_RIGHT = 0x27;
		private const int VK_DOWN = 0x28;
		private const int VK_SHIFT = 0x10;

		private bool isKeyPressed( int vKey ) {
			return GetAsyncKeyState(vKey)<0;
		}

//		private bool shallMoveForward() {
//			return isKeyPressed(VK_LEFT) || isKeyPressed(VK_RIGHT) || isKeyPressed(VK_UP);
//		}

		public override JunctionRoute onJunction( Train train, JunctionRailRoad railRoad ) {
			
			Direction dir = train.head.state.asInside().direction;

			if( isKeyPressed(VK_LEFT ) && railRoad.hasRail( dir.left ) )
				return JunctionRoute.Curve;
			if( isKeyPressed(VK_RIGHT) && railRoad.hasRail( dir.right) )
				return JunctionRoute.Curve;

			return JunctionRoute.Straight;
		}

		public override TimeLength getStopTimeSpan( Train train, TrainHarbor harbor, int callCount ) {
			return getStopTimeSpan(callCount);
		}

		private TimeLength getStopTimeSpan( int callCount ) {
			if( callCount==0 ) {
				if( isKeyPressed(VK_SHIFT) )
					return TimeLength.ZERO;	// pass this station
				else
					return TimeLength.fromMinutes(5);	// make a five minutes stop
			} else {
				// this train has been stopped at this station
				if( isKeyPressed(VK_UP) )
					return TimeLength.ZERO;	// move
				if( isKeyPressed(VK_DOWN) )
					return TimeLength.fromMinutes(-1);	// turn around

				return TimeLength.fromMinutes(5);	// make another five minutes stop and wait for the keyboard input
			}
		}



		public override void config( IWin32Window owner ) {
			MessageBox.Show(owner,
				"When you pass a junction, press left or right arrow to turn\n"+
				"When you reach a station, press shift to pass it\n"+
				"If you stop at a station, press up arrow to leave or down arrow to turn back\n",
				"How to control",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information );
				//! "ポイント通過時に左矢印または右矢印を押していると分岐します¥n"+
				//! "駅に差し掛かったときにSHIFTを押していると通過します¥n"+
				//! "駅に停車したら、上矢印を押すと発車し、下矢印で折り返します¥n",
				//! "操作方法",
		}
	}
}
