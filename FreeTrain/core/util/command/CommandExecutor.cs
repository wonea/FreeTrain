using System;
using System.Collections;
using System.Timers;
using System.Windows.Forms;


namespace freetrain.util.command
{
	// Command Executor base class
	public abstract class CommandExecutor
	{
		protected readonly Hashtable hashInstances = new Hashtable();

		public virtual void InstanceAdded(object item, Command cmd) {
			hashInstances.Add(item, cmd);
		}

		protected Command GetCommandForInstance(object item) {
			return (Command)hashInstances[item];
		}

		// Interface for derived classed to implement
		public abstract void Enable(object item, bool bEnable);
		public abstract void Check(object item, bool bCheck);
		public abstract void SetText(object item, string text);
	}

	// Menu command executor
	public class MenuCommandExecutor : CommandExecutor
	{
		public override void InstanceAdded(object item, Command cmd)
		{
			MenuItem mi = (MenuItem)item;
			mi.Click += new System.EventHandler(menuItem_Click);

			base.InstanceAdded(item, cmd);
		}

		// State setters
		public override void Enable(object item, bool bEnable) {
			MenuItem mi = (MenuItem)item;
			mi.Enabled = bEnable;
		}

		public override void Check(object item, bool bCheck) {
			MenuItem mi = (MenuItem)item;
			mi.Checked = bCheck;
		}

		public override void SetText(object item, string text) {
			MenuItem mi = (MenuItem)item;
			mi.Text = text;
		}


		// Execution event handler
		private void menuItem_Click(object sender, System.EventArgs e)
		{
			Command cmd = GetCommandForInstance(sender);
			cmd.Execute();
		}
	}

	// Toolbar command executor
	public class ToolbarCommandExecutor : CommandExecutor {
		public override void InstanceAdded(object item, Command cmd) {
			ToolBarButton button = (ToolBarButton)item;
			ToolBarButtonClickEventHandler handler = 
				new ToolBarButtonClickEventHandler(toolbar_ButtonClick);

			// Attempt to remove the handler first, in case we have already 
			// signed up for the event in this toolbar
			button.Parent.ButtonClick -= handler;
			button.Parent.ButtonClick += handler;

			base.InstanceAdded(item, cmd);
		}


		// State setters
		public override void Enable(object item, bool bEnable) {
			ToolBarButton button = (ToolBarButton)item;
			button.Enabled = bEnable;
		}

		public override void Check(object item, bool bCheck) {
			ToolBarButton button = (ToolBarButton)item;
			button.Style = ToolBarButtonStyle.ToggleButton;
			button.Pushed = bCheck;
		}

		public override void SetText(object item, string text) {
			ToolBarButton button = (ToolBarButton)item;
			button.Text = text;
		}

		// Execution event handler
		private void toolbar_ButtonClick(   object                      sender, 
											ToolBarButtonClickEventArgs args)
		{
			Command cmd = GetCommandForInstance(args.Button);
			if(cmd!=null)
				cmd.Execute();
		}	
	}

	// Label command executor
	public class LabelCommandExecutor : CommandExecutor
	{
		// State setters
		public override void Enable(object item, bool bEnable) {}
		public override void Check(object item, bool bCheck) {}	// no support for them

		public override void SetText(object item, string text) {
			((Label)item).Text = text;
		}
	}

	// Status bar panel command executor
	public class StatusBarPanelCommandExecutor : CommandExecutor
	{
		// State setters
		public override void Enable(object item, bool bEnable) {}
		public override void Check(object item, bool bCheck) {}	// no support for them

		public override void SetText(object item, string text) {
			((StatusBarPanel)item).Text = text;
		}
	}

	// Button command executor
	public class ButtonCommandExecutor : CommandExecutor
	{
		// State setters
		public override void Enable(object item, bool bEnable) {
			((Button)item).Enabled = bEnable;
		}

		public override void Check(object item, bool bCheck) {}	// no support for them

		public override void SetText(object item, string text) {
			((Button)item).Text = text;
		}

		public override void InstanceAdded(object item, Command cmd) {
			((Button)item).Click += new EventHandler(onClick);
			base.InstanceAdded(item, cmd);
		}

		private void onClick( object sender, EventArgs args ) {
			Command cmd = GetCommandForInstance(sender);
			if(cmd!=null)
				cmd.Execute();
		}
	}
}