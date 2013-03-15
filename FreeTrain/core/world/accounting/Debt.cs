using System;

namespace freetrain.world.accounting
{
	/// <summary>
	/// Payable.
	/// </summary>
	[Serializable]
	public class Debt
	{
		public Debt( long amount, Time due, AccountGenre genre ) {
			this.amount = amount;
			this.due = due;
			this.genre = genre;

			manager.addDebt(this);
			World.world.clock.registerOneShot( new ClockHandler(onDue), span );
		}


		/// <summary> Amount due. </summary>
		public readonly long amount;

		/// <summary> Due date. </summary>
		public readonly Time due;

		/// <summary> Genre. </summary>
		public readonly AccountGenre genre;

		/// <summary> TimeLength before the due date </summary>
		public TimeLength span {
			get { return due-World.world.clock; }
		}

		/// <summary>
		/// Called automatically by the clock when the time comes to
		/// return the debt.
		/// </summary>
		public virtual void onDue() {
			manager.spend(amount,genre);
			manager.removeDebt(this);
		}

		private AccountManager manager { get { return World.world.account; } }
	}
}
