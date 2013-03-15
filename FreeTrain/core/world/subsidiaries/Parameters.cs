using System;

namespace freetrain.world.subsidiaries
{
	internal sealed class Parameters
	{
		private Parameters() {}	// no instanciation

		/// <summary>
		/// Compute the profit of the given structure.
		/// </summary>
		public static long profit( long structurePrice, long landPrice ) {
			long T = structurePrice;
			if( T < landPrice )
				return (long)(maxProfitPrime(T)*2*(landPrice-T)+maxProfit(T));
			else
				return (long)(maxProfitPrime(T)/2*(landPrice-T)+maxProfit(T));
		}

		public static long operationCost( long structurePrice, long landPrice ) {
			return profit(structurePrice,0) + landPrice;
		}

		public static long sales( long structurePrice, long landPrice ) {
			return operationCost(structurePrice,landPrice) + profit(structurePrice,landPrice);
		}


		/// <summary>
		/// Max profit curve. 
		/// </summary>
		private static double maxProfit( long landPrice ) {
			landPrice++;	// avoid log(0)
			return C * landPrice * Math.Log(landPrice) + D;
		}
		
		/// <summary>
		/// Derivative of maxProfit
		/// </summary>
		private static double maxProfitPrime( long landPrice ) {
			landPrice++;	// avoid log(0)
			return C + C * Math.Log(landPrice);
		}

		private const double C = 0.01;
		private const double D = 0;

	}
}
