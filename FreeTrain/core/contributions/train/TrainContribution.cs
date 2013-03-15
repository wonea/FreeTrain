using System;
using System.Drawing;
using System.Xml;
using freetrain.framework.plugin;
using freetrain.framework.graphics;
using freetrain.world;
using freetrain.world.rail;

namespace freetrain.contributions.train
{
	/// <summary>
	/// Train type
	/// </summary>
	[Serializable]
	public abstract class TrainContribution : Contribution
	{
		protected TrainContribution( string _id ) : base("train",_id) {}

		/// <summary>Display name of this train type, such as "series 01500 Blue Line"</summary>
		public abstract string name { get; }

		/// <summary>nick name of train, such as "Blue Line"</summary>
		public abstract string nickName { get; }

		/// <summary>Author who created this contribution.</summary>
		public abstract string author { get; }

		/// <summary>Company name that operates this train, such as "MBTA".</summary>
		public abstract string companyName { get; }

		/// <summary>Type name of train, such as "series 01500"</summary>
		public abstract string typeName { get; }

		public abstract string description { get; }

		/// <summary>Price of the train .</summary>
		public abstract int price( int length );

		/// <summary>Inverse of speed. # of minutes to go for one pixel.</summary>
		public abstract int minutesPerVoxel { get; }

		/// <summary> Fare of this train. </summary>
		public abstract int fare { get; }

		/// <summary>
		/// Creates a new train by designating TrainCarContributions for each car.
		/// </summary>
		public abstract TrainCarContribution[] create( int length );


		public string speedDisplayName {
			get {
				switch(minutesPerVoxel) {
				case 1:	return "Highest";
				case 2:	return "High";
				case 3: return "Medium";
				case 4: return "Low";
				default:	return "Lowest";
				//! case 1:	return "超高速";
				//! case 2:	return "高速";
				//! case 3: return "中速";
				//! case 4: return "低速";
				//! default:	return "超低速";
				}
			}
		}

		public override string ToString() { return name; }

		/// <summary>
		/// Builds a nice preview of a train.
		/// </summary>
		public PreviewDrawer createPreview( Size pixelSize ) {
			PreviewDrawer pd = new PreviewDrawer( pixelSize, new Size(1,3), 0 );

			// build up rail like
			//
			//     /~~~~~
			//    /
			for( int x=-10; x<0; x++ )
				pd.draw( RailPattern.get( Direction.WEST, Direction.EAST ), x, 0 );
			pd.draw( RailPattern.get( Direction.WEST, Direction.SOUTHEAST ), 0, 0 );
			for( int x=1; x<=10; x++ )
				pd.draw( RailPattern.get( Direction.NORTHWEST, Direction.SOUTHEAST ), x, x );

			TrainCarContribution[] cars = create(5);
			if( cars==null ) {
				for( int i=6; cars==null && i<15; i++ )
					cars = create(i);
				for( int i=4; cars==null && i>0; i-- )
					cars = create(i);
				if( cars==null )
					return pd;	// no preview
			}

			int[] pos = new int[]{ -2,0, -1,0, 0,0, 1,1, 2,2 };
			int[] angle = new int[]{ 12,12,13,14,14 };  
			int[] offset = new int[]{ 0,0, 0,0, 4,+2, 0,0, 0,0 };

			using( DrawContext dc = new DrawContext(pd.surface) ) {
				for( int i=4; i>=0; i-- ) {
					if( cars.Length<=i )
						continue;		// no car

					Point pt = pd.getPoint( pos[i*2], pos[i*2+1] );
					cars[i].draw( pd.surface,
						new Point( pt.X+offset[i*2], pt.Y+offset[i*2+1]-9 ), angle[i] );
				}
			}

			return pd;
		}
	}
}
