using System;
using System.Collections;
using System.Timers;
using System.Windows.Forms;

namespace freetrain.util.command
{
	public delegate void CommandHandler(Command cmd);
	public delegate void CommandHandlerNoArg();

	public class Command
	{
		// Members
		public readonly CommandInstanceList commandInstances;
		public readonly CommandManager      manager;

		// state of this command
		private bool enabled =true;
		private bool check =false;

		/// <summary> Application specified tag value. </summary>
		public object tag;

		// Constructor
		public Command( CommandManager manager ) {
			this.manager = manager;
			this.commandInstances = new CommandInstanceList(this);

			manager.commands.add(this);
		}

		// Methods to trigger events
		public virtual void Execute() {
			if (OnExecute == null)	return;	// nothing to do

			Update();
			
			if(Enabled)
				OnExecute(this);			// make sure that the command is enabled before execution
		}

		internal protected virtual void Update() {
			if (OnUpdate != null)
				OnUpdate(this);
		}

		/// <summary> Enables or disables this command. </summary>
		public bool Enabled {
			get {
				return enabled;
			}
			set {
				enabled = value;
				foreach(object instance in commandInstances) {
					manager.GetCommandExecutor(instance).Enable(
						instance, enabled);
				}
			}
		}

		/// <summary> Adds or removes a check from this command. </summary>
		public bool Checked {
			get {
				return check;
			}
			set {
				check = value;
				foreach(object instance in commandInstances) {
					manager.GetCommandExecutor(instance).Check(
						instance, check);
				}
			}
		}

		/// <summary> Sets the text of this command. </summary>
		public string Text {
			set {
				foreach(object instance in commandInstances) {
					manager.GetCommandExecutor(instance).SetText(
						instance, value);
				}
			}
		}

		// Events
		public event CommandHandler OnUpdate;
		public event CommandHandler OnExecute;


		public Command addExecuteHandler( CommandHandler h ) {
			OnExecute += h;
			return this;
		}
		public Command addExecuteHandler( CommandHandlerNoArg h ) {
			return addExecuteHandler(new CommandHandler(new NoArgAdaptor(h).handle));
		}
		/// <summary>
		/// Registers an execute handler that invokes a new dialog.
		/// </summary>
		public Command addDialogExecuteHandler( Type dialogClass, IWin32Window owner ) {
			return addExecuteHandler(new CommandHandler(new DialogExecutor(dialogClass,owner).handle));
		}


		public Command addUpdateHandler( CommandHandler h ) {
			OnUpdate += h;
			return this;
		}

		// adaptor classes
		private class NoArgAdaptor {
			internal NoArgAdaptor( CommandHandlerNoArg h ) {
				this.handler = h;
			}
			private readonly CommandHandlerNoArg handler;
			public void handle( Command cmd ) { handler(); }
		}
		private class DialogExecutor {
			internal DialogExecutor( Type dialogClass, IWin32Window owner ) {
				this.dialogClass = dialogClass;
				this.owner = owner;
			}
			private readonly Type dialogClass;
			private readonly IWin32Window owner;
			public void handle( Command cmd ) {
				Form dlg = (Form)Activator.CreateInstance(dialogClass);
				dlg.ShowDialog(owner);
			}
		}





		//
		// Nested collection class
		//
		public class CommandInstanceList : System.Collections.CollectionBase {
			internal CommandInstanceList(Command acmd) : base() {
				command = acmd;
			}

			private Command command;

			public void Add(object instance) {
				this.List.Add(instance);
			}

			public void AddAll( params object[] items ) {
				foreach (object item in items)
					this.Add(item);
			}

			public void Remove(object instance) {
				this.List.Remove(instance);
			}

			public object this[int index] {
				get {
					return this.List[index];
				}
			}

			protected override void OnInsertComplete(   System.Int32    index, 
														System.Object   value)
			{
				command.manager.GetCommandExecutor(value).InstanceAdded(
					value, command);
			}
		}
	}
}
