using System;
using System.Diagnostics;
using freetrain.world.accounting;
using freetrain.world;

namespace freetrain.views.bank
{
	public delegate void AssetChangeListener(FloatingAsset sender, AssetStatus status, long param);
	/// <summary>
	/// FloatingAssets の概要の説明です。
	/// </summary>
	[Serializable]
	public abstract class FloatingAsset //: Debt
	{
		//[NonSerialized]
		public AssetChangeListener onStatusChanging;

		static protected AccountManager manager { get { return World.world.account; } }
		static protected Clock clock { get { return World.world.clock; } }

		/// <summary> The corpus amount </summary>		
		public long corpus{ get{ return _corpus; } }
		/// <summary> Due date. </summary>
		public Time due { get{ return _due; } }
		public readonly AccountGenre genre;
		/// <summary> Interest rate per year </summary>
		public double interestRate{	get{ return _interest; } }
		/// <summary> Interest amount today </summary>
		public virtual long interestToday 
		{ 
			get{				
				double units = pastTime.totalMinutes/_unitPeriod.totalMinutes;
				double perunit = _unitPeriod.totalMinutes/Time.YEAR;
				return (long)(_corpus*interestRate*perunit*units); 
			} 
		}

		/// <summary> Interest amount at due date </summary>
		public virtual long interestDue	{ 
			get	{ 				
				double years = _totalPeriod.totalMinutes/Time.YEAR;
				return (long)(_corpus*interestRate*years);
			}
		}

		/// <summary> Total amount today </summary>
		public virtual long amountToday	{ get{ return _corpus+interestToday; } }
		/// <summary> Total amount at due date </summary>
		public virtual long amountDue	{ get{ return _corpus+interestDue; } }
		/// <summary> Starting date </summary>
		public virtual Time begining { get{ return _begining; } }
		
		protected long _corpus;
		protected double _interest;
		protected Time _due;
		protected Time _begining;
		protected readonly TimeLength _totalPeriod;
		/// <summary> Unit period for interest calculation. </summary>
		protected readonly TimeLength _unitPeriod;
		public readonly ClockHandler h_unit = null;
		protected Time _lastUnitTime;
		protected bool canceled = true;

		/// <summary> TimeLength from the begining to the due date </summary>
		public TimeLength totalPeriod { get { return _totalPeriod; } }
		/// <summary> TimeLength before the due date </summary>
		public TimeLength restTime { get { return due-clock; } }
		/// <summary> TimeLength before the due date </summary>
		public TimeLength pastTime { get { return clock-_begining; }	}

		public FloatingAsset( long corpus, double interest, Time due, AccountGenre genre )
			//: base(corpus,due,genre)
		{
			this._corpus = corpus;
			this._interest = interest;
			this._begining = clock+TimeLength.ZERO;
			this._due = due;
			this._totalPeriod = restTime;
			this.genre = genre;
			this._unitPeriod = _totalPeriod;
			this._lastUnitTime = clock+TimeLength.ZERO;
			//manager.addDebt(this);
			clock.registerOneShot( new ClockHandler(onDue), _totalPeriod );
		}

		public FloatingAsset( long corpus, double interest, TimeLength unitPeriod, Time due, AccountGenre genre )
			: this(corpus,interest,due,genre)
		{			
			this._unitPeriod = unitPeriod;
			Debug.Assert( unitPeriod.totalMinutes < totalPeriod.totalMinutes );
			h_unit = new ClockHandler(onUnitPeriodEnd);
			clock.registerRepeated( h_unit, _unitPeriod );
		}
		
		// called on due date.
		public virtual void onDue() {
			if( h_unit != null ) {
				clock.unregister(h_unit);
			}
			if(onStatusChanging!=null)
				onStatusChanging(this,AssetStatus.BECOME_DUE,0);
			//base.onDue();
		}

		// called on unit period for interest calculation.
		public virtual void onUnitPeriodEnd() {
			if(_lastUnitTime == clock ) return;
			_lastUnitTime = clock+TimeLength.ZERO;
			if(onStatusChanging!=null)
				onStatusChanging(this,AssetStatus.UNIT_PERIOD_END,0);
			//base.onDue();
		}

		protected internal virtual void Repay( long _amount ){}

		protected internal virtual void Cancel() 
		{
			//if( amount < 0 ) return;
			if(onStatusChanging!=null)
				onStatusChanging(this,AssetStatus.CANCELING,0);
			canceled = true;
			//manager.removeDebt(this);
		}

	}
	
	public enum AssetStatus{
		NEW_ASSET,BECOME_DUE,UNIT_PERIOD_END,STATUS_UPDATING,CORPUS_CHANGING,CANCELING}

}
