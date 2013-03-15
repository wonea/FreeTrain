using System;
using freetrain.util.command;

namespace PicturePreviewer
{
	internal delegate int IntGetter();
	internal delegate void IntSetter(int value);

	internal class IntCommandHandler
	{
		private readonly IntGetter getter;
		private readonly IntSetter setter;
		private readonly int i;
		internal IntCommandHandler( Command cmd, IntGetter _getter, IntSetter _setter, int _i ) {
			this.getter = _getter;
			this.setter = _setter;
			this.i = _i;
			cmd.addUpdateHandler(  new CommandHandler(update) );
			cmd.addExecuteHandler( new CommandHandler(execute) );
		}

		private void update( Command cmd ) {
			cmd.Checked = (getter()==i);
		}

		private void execute( Command cmd ) {
			setter(i);
		}
	}
}
