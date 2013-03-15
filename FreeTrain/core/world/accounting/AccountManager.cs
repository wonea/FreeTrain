using System;
using System.Windows.Forms;
using freetrain.framework;
using freetrain.util;

namespace freetrain.world.accounting
{
	public delegate void AccountListener();
//	public delegate void AccountTransactionListener( long delta, AccountGenre genre );

	/// <summary>
	/// Maintains accounting and financing.
	/// </summary>
	[Serializable]
	public class AccountManager
	{
		/// <summary> Debts. </summary>
		private readonly Set debts = new Set();
		
		/// <summary>
		/// This event is fired everytime there's a change
		/// in the account. Parameters are not used.
		/// </summary>
		public static AccountListener onAccountChange;
		
		/// <summary>
		/// Obtain a reference to the sole instance.
		/// </summary>
		public static AccountManager theInstance {
			get {
				return World.world.account;
			}
		}

		/// <summary>
		/// The current liquid assets.
		/// One can think of this as cold cash
		/// (though in reality a company never really has cash.)
		/// 
		/// The game is over if the liquid assets goes below zero.
		/// </summary>
		private long _liquidAssets;
		
		/// <summary>
		/// Total amount of debts.
		/// </summary>
		private long _totalDebts;
//
//		/// <summary> Fired when there is a transaction. </summary>
//		public event AccountTransactionListener onEarned;
//		public event AccountTransactionListener onSpent;


		public long liquidAssets	{ get { return _liquidAssets; } }
		public long totalDebts		{ get { return _totalDebts; } }



		public AccountManager() : this(1500*10000) {}

		public AccountManager( long initialLiquidAssets ) {
			this._liquidAssets = initialLiquidAssets;
		}

		private void transact( long delta, AccountGenre genre ) {
			_liquidAssets -= delta;
			if( _liquidAssets < 0 ) {
				// TODO: go bunkrupt
				MessageBox.Show( MainWindow.mainWindow, "You are bankrupt. Proceeding with more funds." );
				//! MessageBox.Show( MainWindow.mainWindow, "破産しました。お金を増やして続行します" );
				_liquidAssets += 100000000;
			}
		}

		public void spend( long delta, AccountGenre genre ) {
			transact(delta,genre);
			genre.history.spend(delta);
			if(genre.onUpdate!=null)		genre.onUpdate();
			if(onAccountChange!=null)		onAccountChange();
		}

		public void earn( long delta, AccountGenre genre ) {
			transact(-delta,genre);
			genre.history.earn(delta);
			if(genre.onUpdate!=null)		genre.onUpdate();
			if(onAccountChange!=null)		onAccountChange();
		}

		internal void addDebt( Debt debt ) {
			debts.add(debt);
			updateTotalDebts();
		}

		internal void removeDebt( Debt debt ) {
			debts.remove(debt);
			updateTotalDebts();
		}

		private void updateTotalDebts() {
			long sum = 0;
			foreach( Debt d in debts )
				sum += d.amount;
			
			_totalDebts = sum;
			if(onAccountChange!=null)		onAccountChange();
		}
	}
}
