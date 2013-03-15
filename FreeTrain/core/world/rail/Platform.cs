using System;
using System.Diagnostics;
using System.Drawing;
using org.kohsuke.directdraw;
using freetrain.contributions.rail;
using freetrain.framework;
using freetrain.world.accounting;
using freetrain.util;

namespace freetrain.world.rail
{
	/// <summary>
	/// Platform that trains can stop by.
	/// </summary>
	[Serializable]
	public abstract class Platform : PlatformHost
	{
		protected Platform( Location loc, Direction d, int len ) {
			this.location = loc;
			this.direction = d;
			this.length = len;
			this.name = string.Format("Platform {0,2:d}",iota++);
			//! this.name = string.Format("ホーム{0,2:d}",iota++);
			this.bellSound = DepartureBellContribution.DEFAULT;
			World.world.clock.registerRepeated( new ClockHandler(onClockPerDay), TimeLength.ONEDAY );
			
			// attach to the nearby station.
			foreach( PlatformHost h in listHosts()) {
				if( h is Station ) {
					host = h;
					break;
				}
			}
		}


		/// <summary> Name of the platform if any. </summary>
		public string name;

		/// <summary> Location of the base of this platform. </summary>
		public readonly Location location;

		/// <summary> Direction of this platform. </summary>
		public readonly Direction direction;
		
		/// <summary> Length of the platform. </summary>
		public readonly int length;

		/// <summary> Parent host of this platform. </summary>
		private PlatformHost _host=null;

		/// <summary>
		/// Set of child Platforms that are connected to a station through this platform.
		/// </summary>
		private readonly Set nodes = new Set();
		
		/// <summary> Departure bell sound. May not be null. </summary>
		public DepartureBellContribution bellSound;



		/// <summary> Other end of the platform. </summary>
		public Location otherEnd {
			get {
				Location l = location;
				l.x += direction.offsetX * length;
				l.y += direction.offsetY * length;
				return l;
			}
		}



		
		/// <summary> Host of this platform, or null if this is disconnected. </summary>
		internal protected PlatformHost host {
			get { return _host; }
			set {
				if(_host==value)	return;

				// remove from the current parent
				if(_host!=null)	_host.removeNode(this);
				_host = null;

				// notify nodes that this host is going to be destroyed.
				// we need to copy it into array because nodes will be updated
				// as we notify children
				foreach( Platform p in nodes.toArray(typeof(Platform)) )
					p.onHostDisconnected();
				Debug.Assert(nodes.isEmpty);

				// see if we can add to the new host.
				if( value!=null && value.hostStation!=null ) {
					_host = value;
					if(_host!=null)	_host.addNode(this);
				}
				
				// update the warning icon
				World.world.onVoxelUpdated(location);
			}
		}

		/// <summary> Host station of this platform, or null if this is isolated. </summary>
		public Station hostStation {
			get {
				if( host!=null )	return host.hostStation;
				else				return null;
			}
		}

		public abstract bool canRemove { get; }

		#region Entity implementation
		public bool isSilentlyReclaimable { get { return false; } }
		public bool isOwned { get { return true; } }

		// TODO: value?
		public int entityValue { get { return 0; } }

		public virtual void remove() {
			World.world.clock.unregister( new ClockHandler(onClockPerDay) );
			if(onEntityRemoved!=null)	onEntityRemoved(this,null);
		}
		public event EventHandler onEntityRemoved;

		public virtual object queryInterface(Type aspect) {
			return null;
		}
		#endregion


		internal void onHostDisconnected() {
			host = null;
		}

		public void onClockPerDay() {
			// charge the cost
			AccountManager.theInstance.spend( 18*length, AccountGenre.RAIL_SERVICE );
		}





		/// <summary>
		/// Lists available platform hosts for this platform.
		/// </summary>
		internal protected abstract PlatformHost[] listHosts();


		/// <summary> Processes a click event. </summary>
		public void onClick() {
			new PlatformPropertyDialog(this).ShowDialog(MainWindow.mainWindow);
		}



		/// <summary>
		/// Implementation for the listHosts() method.
		/// Lists available platform hosts for this platform.
		/// </summary>
		protected PlatformHost[] listHosts( int range ) {
			Set result = new Set();

			Location loc1 = Location.min(
				location + new Distance(-range,-range,0),
				otherEnd + new Distance(-range,-range,0));

			Location loc2 = Location.max(
				location + new Distance( range, range,0),
				otherEnd + new Distance( range, range,0));

			// scan the rectangle inside this region
			for( int z=location.z-1; z<=location.z+1; z++ ) {
				for( int y=loc1.y; y<=loc2.y; y++ ) {
					for( int x=loc1.x; x<=loc2.x; x++ ) {
						Station st = Station.get(x,y,z);
						if(st!=null)	result.add(st);
						Platform pt = Platform.get(x,y,z);
						if(pt!=null && pt!=this && pt.hostStation!=null)
							result.add(pt);
					}
				}
			}

			// find hosts below and above this platform
			Location loc = location;
			for( int i=0; i<length; i++, loc+=direction ) {
				for( int z=0; z<World.world.size.z; z++ ) {
					Station st = Station.get(loc.x,loc.y,z);
					if(st!=null)	result.add(st);
					Platform pt = Platform.get(loc.x,loc.y,z);
					if(pt!=null && pt!=this && pt.hostStation!=null)
						result.add(pt);
				}
			}


			return (PlatformHost[])result.toArray(typeof(PlatformHost));
		}

		public void addNode( Platform child ) {
			nodes.add(child);
		}

		public void removeNode( Platform child ) {
			nodes.remove(child);
		}

		public override string ToString() { return name; }



	//
	// static methods/fields
	//


		/// <summary>
		/// sequence number generator for automatic name generation.
		/// </summary>
		private static int iota=1;

		/// <summary> Warning icon. </summary>
		protected static readonly Surface warningIcon =
			ResourceUtil.loadTimeIndependentSystemSurface("caution.bmp");


		public static Platform get( Location loc ) {
			Platform p = FatPlatform.get(loc);
			if(p!=null)	return p;

			// TODO: check slim platform
			return ThinPlatform.get(loc);
		}

		public static Platform get( int x, int y, int z ) { return get(new Location(x,y,z)); }

	
	}
}
