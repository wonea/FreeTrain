using System;

namespace freetrain.world
{
	/// <summary>
	/// Matrix that applies to directions. Immutable.
	/// </summary>
	[Serializable]
	public class Matrix
	{
		/// <summary>
		/// All the numbers are doubled to allow the matrix to perform 45-degree
		/// rotational transformation.
		/// </summary>
		private readonly int a,b,c,d;

		public Matrix( int a, int b, int c, int d ) {
			this.a=a*2; this.b=b*2; this.c=c*2; this.d=d*2;
		}

		public Matrix( float a, float b, float c, float d ) {
			this.a=(int)(a*2); this.b=(int)(b*2); this.c=(int)(c*2); this.d=(int)(d*2);
		}

		private Matrix( int a, int b, int c, int d, bool dummy ) {
			this.a=a; this.b=b; this.c=c; this.d=d;
		}

		public static Matrix operator + ( Matrix x, Matrix y ) {
			return new Matrix( x.a+y.a, x.b+y.b, x.c+y.c, x.d+y.d, false );
		}

		public static Matrix operator - ( Matrix x, Matrix y ) {
			return new Matrix( x.a-y.a, x.b-y.b, x.c-y.c, x.d-y.d, false );
		}

		public static Matrix operator * ( int s, Matrix x ) {
			return new Matrix( s*x.a, s*x.b, s*x.c, s*x.d, false );
		}

		public static Distance operator * ( Matrix x, Distance d ) {
			return new Distance(
					(x.a*d.x + x.b*d.y)/2,
					(x.c*d.x + x.d*d.y)/2,
					d.z );
		}

		#region constants
		/// <summary>
		/// Identity transformation
		/// </summary>
		public static readonly Matrix E = new Matrix(1,0,0,1);
		
		/// <summary>
		/// 90-degree left rotational transformation
		/// </summary>
		public static readonly Matrix L90 = new Matrix(0,1,-1,0);

		/// <summary>
		/// 90-degree right rotational transformation
		/// </summary>
		public static readonly Matrix R90 = new Matrix(0,-1,1,0);

		/// <summary>
		/// 180-degree rotational transformation
		/// </summary>
		public static readonly Matrix REVERSE = new Matrix(-1,0,0,-1);
		#endregion
	}
}
