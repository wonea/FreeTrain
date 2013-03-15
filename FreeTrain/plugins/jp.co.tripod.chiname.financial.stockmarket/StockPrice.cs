using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using freetrain.world;
using freetrain.world.accounting;

namespace freetrain.finance.stock
{
	public delegate void MinMaxListener(int val);

	[Serializable]
	public class StockPrice : IStockPrice
	{
		public int low { get { return _low; } }
		public int high { get { return _high; } }
		public int start { get { return _start; } }
		public int end { get { return _end; } }
		private int _low;
		private int _high;
		private int _start;
		private int _end;

		[NonSerialized]
		public MinMaxListener onMinChange;
		[NonSerialized]
		public MinMaxListener onMaxChange;

		private StockPrice(){}

		public StockPrice( int init_val ) 
		{
			init(init_val);
		}

		public StockPrice( int init_val ,MinMaxListener minL, MinMaxListener maxL ) 
		{
			onMinChange = minL;
			onMaxChange = maxL;
			init(init_val);
		}

		//		public StockPrice( StockPrice source ) {
		//			_low = source._low;
		//			_high = source._high;
		//			_start = source._start;
		//			_end = source._end;
		//		}

		public void merge( StockPrice other ) 
		{
			this._end = other._end;
			if(this._high < other._high) 
			{
				this._high = other._high;
				if( onMaxChange!= null ) onMaxChange(this._high);
			}
			if(this._low > other._low) 
			{
				this._low = other._low;
				if( onMinChange!= null ) onMinChange(this._low);
			}
		}

		public void merge(int price) 
		{
			_end = price;
			if(_high < price) 
			{
				_high = price;
				if( onMaxChange!= null ) onMaxChange(this._high);
			}
			if(_low > price) 
			{
				_low = price;
				if( onMinChange!= null ) onMinChange(this._low);
			}
		}

		public void init( int start )
		{
			_start = start;
			_high = start;
			_low = start;
			_end = start;
			if( onMaxChange!= null ) onMaxChange(start);
			if( onMinChange!= null ) onMinChange(start);
		}

	}

	/// <summary>
	/// StockPriceLogger の概要の説明です。
	/// </summary>
	[Serializable]
	public class StockPriceLogger
	{
		// dayly data
		private ArrayList dayly = null;
		private DataRange daylyRange = new DataRange(0,0);
		// monthly data
		private ArrayList monthly = null;
		private DataRange monthlyRange = new DataRange(0,0);
		private readonly int _datacounts;
		[NonSerialized]
		public DataUpdateListener onUpdate = null;

		public StockPriceLogger(int size) 
		{
			_datacounts = size+1;
			dayly = new ArrayList(_datacounts);
			monthly = new ArrayList(_datacounts);
			for(int i = 0; i<_datacounts; i++ ) 
			{
				dayly.Add( new StockPrice(	0,
					new MinMaxListener(onDaylyMinimumChange),
					new MinMaxListener(onDaylyMaximumChange)) );
				monthly.Add( new StockPrice(0,
					new MinMaxListener(onMonthlyMinimumChange),
					new MinMaxListener(onMonthlyMaximumChange)) );
			}
		}

		#region min-max delegates
		public void onDaylyMinimumChange(int val) {	daylyRange.isMin(val); }
		public void onDaylyMaximumChange(int val) {	daylyRange.isMax(val); }
		public void onMonthlyMinimumChange(int val) { monthlyRange.isMin(val); }
		public void onMonthlyMaximumChange(int val)	{ monthlyRange.isMax(val); }
		#endregion

		public IStockPrice this[XAxisStyle scale, int index]
		{
			get 
			{				
				ArrayList src = null;
				switch( scale ) 
				{
					case XAxisStyle.MONTHLY:
						src = monthly;
						break;
					case XAxisStyle.DAILY:
						src = dayly;
						break;
				}
				if( src == null )
					return null;
				else 
					return (StockPrice)src[index];
			}
		}

		public DataRange getDataRange(XAxisStyle scale) 
		{
			if( scale == XAxisStyle.MONTHLY )
				return monthlyRange;
			else
				return daylyRange;
		}

		public void daylyEnter( Time date, int start ) 
		{
			try 
			{
				removeLast(dayly,ref daylyRange); 
			} 
			catch(ArgumentOutOfRangeException e) {}
			dayly.Insert(0, new StockPrice(
				start,
				new MinMaxListener(onDaylyMinimumChange),
				new MinMaxListener(onDaylyMaximumChange)
				));

			if( date.day == 1 ) 
			{
				try 
				{ 
					removeLast(monthly,ref monthlyRange); 
				} 
				catch(ArgumentOutOfRangeException e) {}
				monthly.Insert(0, new StockPrice(
					start,
					new MinMaxListener(onMonthlyMinimumChange),
					new MinMaxListener(onMonthlyMaximumChange)
					));
			}
			if( onUpdate != null )
				onUpdate(0,_datacounts-1);

		}

		public void daylyExit( StockPrice price ) 
		{
			// detatch min-max listeners
			StockPrice p_day  = (StockPrice)dayly[0];
			p_day.merge(price);
			p_day.onMaxChange = null;
			p_day.onMinChange = null;
			StockPrice p_mon  = (StockPrice)monthly[0];
			p_mon.merge(price);
			p_mon.onMaxChange = null;
			p_mon.onMinChange = null;
		}

		private void removeLast( ArrayList array, ref DataRange range ) 
		{
			Debug.Assert( array.Count == _datacounts );
			StockPrice rmv = (StockPrice)array[_datacounts-1];
			// check min-max values and update DataRange
			if( rmv.high >= range.max )
			{
				range.max = 0;
				for( int i=0; i< _datacounts-1; i++ ) 
				{
					StockPrice p = (StockPrice)array[i];
					range.isMax(p.high);
				}
			}
			if( rmv.low <= range.min )
			{
				range.min = range.max;
				for( int i=0; i< _datacounts-1; i++ ) 
				{
					StockPrice p = (StockPrice)array[i];
					range.isMin(p.low);
				}
			}
			rmv.onMaxChange = null;
			rmv.onMinChange = null;
			array.RemoveAt(_datacounts-1);
		}

		internal protected void restoreListener()
		{
			StockPrice p_day  = (StockPrice)dayly[0];
			p_day.onMaxChange = new MinMaxListener(onDaylyMaximumChange);
			p_day.onMinChange = new MinMaxListener(onDaylyMinimumChange);
			StockPrice p_mon  = (StockPrice)monthly[0];
			p_mon.onMaxChange = new MinMaxListener(onMonthlyMaximumChange);
			p_mon.onMinChange = new MinMaxListener(onMonthlyMinimumChange);
		}
	}	
}
