using System;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using freetrain.framework.plugin;
using freetrain.world;
using freetrain.world.accounting;
using freetrain.util;

namespace freetrain.views.bank
{
	public delegate void BankBusinessHourListener();

	public enum BankStatus : int { OPEN,CLOSE,HOLIDAY } 
	
	/// <summary>
	/// Bank.
	/// This object is part of the serialized game data. Parameters that
	/// shouldn't be serialized should go to BankConfig.
	/// </summary>
	[Serializable]
	public sealed class BankModule
	{

		static private AccountManager manager { get { return World.world.account; } }
		static private Clock clock { get { return World.world.clock; } }

		internal readonly Set debts = new Set(); 
		internal readonly Set deposits = new Set();
		internal long _totalDebt = 0;
		internal long _totalDeposit = 0;

		public IEnumerator getDebts(){ return debts.GetEnumerator(); }
		public IEnumerator getDeposits(){ return deposits.GetEnumerator(); }
		public long totalDebt{ get{ return _totalDebt; } }
		public long totalDeposit{ get{ return _totalDeposit; } }

		[NonSerialized]
		public BankBusinessHourListener onBusinesStatusChanging;
		

		internal static void init() {
			World.onNewWorld += new EventHandler(onNewWorld);
		}

	
		private static BankModule _theInstance = null;
		public static BankModule theInstance { get { return _theInstance; } }
		
		// don't allow instanciation from external objects
		private BankModule() {
			// Set clock timer
			clock.registerRepeated(
				new ClockHandler(statusClockHandler),
				TimeLength.untilTomorrow(),
				TimeLength.ONEDAY );

			clock.registerRepeated(
				new ClockHandler(statusClockHandler),
				TimeLength.untilTomorrow()+
					TimeLength.fromHours(BankConfig.openHour),
				TimeLength.ONEDAY );

			if( BankConfig.openHour%24 != BankConfig.closeHour%24 ) {
				clock.registerRepeated(
					new ClockHandler(statusClockHandler),
					TimeLength.untilTomorrow()+
						TimeLength.fromHours(BankConfig.closeHour),
					TimeLength.ONEDAY );
			}
		}


		private static void onNewWorld(object sender,EventArgs a) {
			BankBusinessHourListener listener = null;
			if( _theInstance != null )
				listener = _theInstance.onBusinesStatusChanging;

			_theInstance = (BankModule)World.world.otherObjects["{227E053A-6667-43fe-8CE1-26EB55CE6A56}"];
			if( _theInstance==null )
				World.world.otherObjects["{227E053A-6667-43fe-8CE1-26EB55CE6A56}"] =  _theInstance =
					new BankModule();
			// restoring listener registrations (may be a BankbookWindow).
			if( listener != null )
				_theInstance.onBusinesStatusChanging = listener;
			// restore registrations for BankbookListHelper
			IEnumerator e1 = _theInstance.debts.GetEnumerator();
			BankbookListHelper.restoreData();
		}


		public void statusClockHandler() 
		{
			if(onBusinesStatusChanging!=null)
				onBusinesStatusChanging();
		}
		
		/// <summary>
		/// Returns the current status of the bank.
		/// </summary>
		public BankStatus status {
			get {
				if(BankConfig.isHoliday(clock))
					return BankStatus.HOLIDAY;
				if( BankConfig.openHour <= clock.hour && clock.hour < BankConfig.closeHour )
					return BankStatus.OPEN;
				else
					return BankStatus.CLOSE;
			}
		}

		// recalculate total amount of debts and deposit.
		public void recalcTotalAssets(){
			IEnumerator e1 = debts.GetEnumerator();
			_totalDebt = 0;
			while(e1.MoveNext()) {
				_totalDebt += ((FloatingAsset)e1.Current).amountDue;
			}
			IEnumerator e2 = debts.GetEnumerator();
			_totalDeposit = 0;
			while(e2.MoveNext()) 
			{
				_totalDeposit += ((FloatingAsset)e2.Current).amountDue;
			}
		}

		// The upper limit of debt amount available.
		public long getDebtLimit() {
			return 5000000-totalDebt;
		}
		
		// Returns interest for deposit (in percent).
		public double GetDepositInterest(TimeLength period)
		{
			long n = period.totalMinutes/Time.YEAR;			
			return (double)n/400;
		}

		// Returns interest for debt (in percent).
		public double GetDebtInterest(TimeLength period)
		{
			long n = period.totalMinutes/Time.YEAR;
			return 0.045+(double)n/200;
		}

		// Called when asset status changed.
		public void onDebtStatusChanged(FloatingAsset sender, AssetStatus status, long param) {
			if(status == AssetStatus.CANCELING ) {
				recalcTotalAssets();
				debts.remove(sender);
			}
		}

		// Called when asset status changed.
		public void onDepositStatusChanged(FloatingAsset sender, AssetStatus status, long param) {
			if(status == AssetStatus.CANCELING ) {
				deposits.remove(sender);
				recalcTotalAssets();
			}
		}

		public void borrow(long amount, TimeLength period )
		{
			if( !BankConfig.canBorrow ) return;
			double interest = GetDebtInterest(period);
			Time due = clock+period;
			DebtEx item = new DebtEx(amount,interest,due,AccountGenre.OTHERS);
			debts.add(item);
			BankbookListHelper.addNewDebt(item);
			_totalDebt += amount;
			item.onStatusChanging += new AssetChangeListener(onDebtStatusChanged);
		}

		public void repay(FloatingAsset item, long amount)
		{
			if( !BankConfig.canRepay ) return;
			item.Repay(amount);
			_totalDebt -= amount;
		}

		public void deposit(long amount, TimeLength period )
		{
			if( !BankConfig.canDeposit ) return;
			double interest = GetDepositInterest(period);
			Time due = clock+period;
			FixedDeposit item = new FixedDeposit(amount,interest,due,AccountGenre.OTHERS);
			deposits.add(item);
			BankbookListHelper.addNewDeposit(item);
			_totalDeposit += amount;
			item.onStatusChanging += new AssetChangeListener(onDepositStatusChanged);
		}

		public void cancelDeposit(FloatingAsset item, bool immediately )
		{
			if( !BankConfig.canCancel ) return;
			if( immediately )
				item.Cancel();
			else
				((FixedDeposit)item).CancelAtDue();
		}
	}
}
