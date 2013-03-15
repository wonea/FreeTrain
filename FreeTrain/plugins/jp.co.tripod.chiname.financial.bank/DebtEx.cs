using System;
using System.Diagnostics;
using System.Windows.Forms;
using freetrain.world.accounting;
using freetrain.world;
using freetrain.framework;
using freetrain.controllers;

namespace freetrain.views.bank
{
	/// <summary> Extended Debt class </summary>
	[Serializable]
	public class DebtEx :  FloatingAsset
	{
		public DebtEx( long corpus, double interest, Time due, AccountGenre genre ):
			base(corpus,interest,due,genre) {
			initialze();
		}

		public DebtEx( long corpus, double interest, TimeLength unitPeriod, Time due, AccountGenre genre ):
			base(corpus,interest, unitPeriod, due,genre) {
			initialze();
		}
		
		protected void initialze()
		{
			manager.earn(_corpus, genre);
			clock.registerOneShot(new ClockHandler(warningBeforeDue),due-clock+TimeLength.fromDays(-30));
		}
		/// <summary> Fixed interest before repaying. </summary>
		protected long _interestFixedEx = 0;

		public override long interestToday { get{ return base.interestToday+_interestFixedEx; } }
		public override long interestDue	{ get{ return base.interestDue+_interestFixedEx; } }


		protected internal override void Repay( long _amount ) 
		{
			Debug.Assert( amountToday >= _amount );
			if( amountToday == _amount ) 
			{
				manager.spend(_amount,genre);
				Cancel();
			}
			else 
			{
				long old = _corpus;
				TimeLength span = clock-_lastUnitTime;
				// Fix interest for the amount being repayed.
				double intrToday = interestRate*span.totalMinutes/Time.YEAR;
				long realAmount = (long)(_amount/(1.0+intrToday));
				manager.spend(_amount,genre);
				_interestFixedEx += (_amount-realAmount);
				_corpus -= realAmount;
				if(onStatusChanging!=null)
					onStatusChanging(this,AssetStatus.CORPUS_CHANGING,old);
			}
		}
		
		// Show confirmation message
		public void warningBeforeDue()
		{
			// _corpuse is zero if the asset is already canceled.
			if( !canceled )
				MainWindow.showError("Loan repayment due soon.");
				//! MainWindow.showError("借入金の返済期限が近づいています。");
		}

		public override void onDue() 
		{
			base.onDue();
			// amountDue involves _interestFixedEx (which is already spend). so subtract it.
			manager.spend(amountDue-_interestFixedEx,genre);
			Cancel();
		}

	}
}
