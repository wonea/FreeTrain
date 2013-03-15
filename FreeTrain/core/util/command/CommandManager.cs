/******************************************************************************
Dictionary
-------------------------------------------------------------------------------
CommandManager          -   Global service that manages a collection of 
                            commands

Command                 -   A conceptual representation for an application
                            operation (ie.  Save, Edit, Load, etc...)
                            
Command Instance        -   A UI element associated with a command (ie, Menu
                            item, Toolbar Item, etc...).  A command can have
                            multiple instances
                            
Command Type            -   A UI class that can house a command Instance (ie
                            "System.Windows.Forms.MenuItem",
                            "System.Windows.Forms.ToolbarItem" )

CommandExecutor         -   An object that can handle all communication between
                            the command manager and a particular command instance
                            for a particular command type.

UpdateHandler           -   Event handler for the Commands Update event.
******************************************************************************/
using System;
using System.Collections;
using System.Timers;
using System.Windows.Forms;

namespace freetrain.util.command
{
public class CommandManager : System.ComponentModel.Component
{
    // Commands Property: Fetches the Command collection
    internal readonly Set        commands = new Set();
    private readonly Hashtable           hashCommandExecutors = new Hashtable(); 

    // Constructor
    public CommandManager() {
        // Setup idle processing
        Application.Idle += new EventHandler(this.OnIdle);

        // By default, menus and toolbars are known
        RegisterCommandExecutor( "System.Windows.Forms.MenuItem", new MenuCommandExecutor());
        RegisterCommandExecutor( "System.Windows.Forms.ToolBarButton", new ToolbarCommandExecutor());
		RegisterCommandExecutor( "System.Windows.Forms.Label", new LabelCommandExecutor() );
		RegisterCommandExecutor( "System.Windows.Forms.Button", new ButtonCommandExecutor() );
		RegisterCommandExecutor( "System.Windows.Forms.StatusBarPanel", new StatusBarPanelCommandExecutor() );
    }

    // Command Executor association methods
    internal void RegisterCommandExecutor( string strType, CommandExecutor executor ) {
        hashCommandExecutors.Add(strType, executor);
    }

    internal CommandExecutor GetCommandExecutor(object instance) {
        return (CommandExecutor)hashCommandExecutors[instance.GetType().ToString()];
    }

	public void updateAll() {
		foreach( Command cmd in commands )
			cmd.Update();
	}

    //  Handler for the Idle application event.
    private void OnIdle(object sender, EventArgs args) {
		updateAll();
    }
}

}  // end namespace
