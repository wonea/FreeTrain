using System;
using System.Diagnostics;
using freetrain.world.accounting;
using freetrain.world;

namespace freetrain.views.bank
{
	/// <summary>
	/// Fixed time deposit
	/// </summary>
	[Serializable]
	public class FixedDeposit : FloatingAsset
	{
		static private BankModule bank { get { return BankModule.theInstance; } }

		public FixedDeposit( long corpus, double interest, Time due, AccountGenre genre ):
			base(corpus,interest,due,genre) {
			initialize();
		}

		public FixedDeposit( long corpus, double interest, TimeLength unitPeriod, Time due, AccountGenre genre ):
			base(corpus,interest, unitPeriod, due,genre) {
			initialize();
		}
		protected void initialize()
		{
			minInterestRate = bank.GetDepositInterest(TimeLength.fromMinutes(Time.YEAR));
			manager.spend(_corpus,genre);
		}

		protected bool auto_continue = true;

		public bool isContinue{ get{ return auto_continue; } }

		/// <summary>
		/// Cancel at due date. 
		/// (if this method is not called, the deposit will be continued.)
		/// </summary>
		public void CancelAtDue(){ 
			auto_continue = false; 
			if(onStatusChanging!=null)
				onStatusChanging(this,AssetStatus.STATUS_UPDATING,0);
		}

		protected double minInterestRate;

		public override long interestToday {
			get{
				double years = Math.Floor((double)pastTime.totalMinutes/Time.YEAR);
				return (long)(_corpus*minInterestRate*years); 
			}
		}

		public override void onDue() 
		{
			base.onDue();
			if( auto_continue )
			{
				this._corpus = amountDue;
				this._interest = bank.GetDepositInterest(_totalPeriod);
				minInterestRate = bank.GetDepositInterest(TimeLength.fromMinutes(Time.YEAR));
				this._begining = clock+TimeLength.ZERO;
				this._due += _totalPeriod;
				this._lastUnitTime = clock+TimeLength.ZERO;
				if(onStatusChanging!=null)
					onStatusChanging(this,AssetStatus.STATUS_UPDATING,0);
				clock.registerOneShot( new ClockHandler(onDue), _totalPeriod );				
			}
			else
				Cancel();
		}

		public override void onUnitPeriodEnd() {}

		// cannot repay!
		protected internal override void Repay( long _amount ) { Debug.Assert( false );	}

		protected internal override void Cancel() 
		{	
			if( clock.totalMinutes >= _due.totalMinutes )
				manager.earn(amountDue,genre);
			else
				manager.earn(amountToday,genre);
			base.Cancel();
		}
	}
}
