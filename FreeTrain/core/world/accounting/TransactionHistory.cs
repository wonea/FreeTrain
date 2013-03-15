using System;

namespace freetrain.world.accounting
{
	[Serializable]
	public abstract class TransactionSummary {
		public abstract long sales { get; }
		public abstract long expenditures { get; }
		public long balance { get { return sales-expenditures; } }
	}

	/// <summary>
	/// Records the summary of past transactions.
	/// </summary>
	[Serializable]
	public class TransactionHistory
	{
		[Serializable]
		private class Recorder {
			private long _dayTotal;
			private long _monthTotal;
			private long _yearTotal;
			private readonly Clock clock = World.world.clock;

			internal Recorder() {
				// align the clock to 0:00am
				clock.registerRepeated(
					new ClockHandler(onClock),
					TimeLength.fromMinutes(
						TimeLength.ONEDAY.totalMinutes-(clock.totalMinutes%TimeLength.ONEDAY.totalMinutes)),
					TimeLength.ONEDAY );
			}

			internal long dayTotal   { get { return _dayTotal; } }
			internal long monthTotal { get { return _dayTotal+_monthTotal; } }
			internal long yearTotal  { get { return monthTotal+_yearTotal; } }

			internal void add( long delta ) {
				_dayTotal += delta;
			}

			public void onClock() {
				_monthTotal += _dayTotal;
				_dayTotal = 0;
				if( clock.day==1 ) {
					_yearTotal += _monthTotal;
					_monthTotal = 0;
					if( clock.month==4 ) {
						_yearTotal = 0;
					}
				}
			}
		}

		// used to record sales and expenditures
		private readonly Recorder sales = new Recorder();
		private readonly Recorder expenditures = new Recorder();
		
		// expose those information to outside
		public readonly TransactionSummary day;
		public readonly TransactionSummary month;
		public readonly TransactionSummary year;

		/// <summary>
		/// Record transactions of the given genre.
		/// </summary>
		public TransactionHistory() {
			day		= new DayTransactionSummary(this);
			month	= new MonthTransactionSummary(this);
			year	= new YearTransactionSummary(this);
		}

		internal void earn( long delta ) {
			sales.add(delta);
		}

		internal void spend( long delta ) {
			expenditures.add(delta);
		}

		[Serializable]
		private class DayTransactionSummary : TransactionSummary {
			private readonly TransactionHistory history;

			internal DayTransactionSummary( TransactionHistory _history ) {
				this.history = _history;
			}

			public override long sales { get { return history.sales.dayTotal; } }
			public override long expenditures { get { return history.expenditures.dayTotal; } }
		}

		[Serializable]
		private class MonthTransactionSummary : TransactionSummary {
			private readonly TransactionHistory history;

			internal MonthTransactionSummary( TransactionHistory _history ) {
				this.history = _history;
			}

			public override long sales { get { return history.sales.monthTotal; } }
			public override long expenditures { get { return history.expenditures.monthTotal; } }
		}

		[Serializable]
		private class YearTransactionSummary : TransactionSummary {
			private readonly TransactionHistory history;

			internal YearTransactionSummary( TransactionHistory _history ) {
				this.history = _history;
			}

			public override long sales { get { return history.sales.yearTotal; } }
			public override long expenditures { get { return history.expenditures.yearTotal; } }
		}

	}
}
