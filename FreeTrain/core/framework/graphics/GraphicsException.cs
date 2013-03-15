using System;

namespace freetrain.framework.graphics
{
	public class GraphicsException : Exception
	{
		public GraphicsException( string msg, Exception nested )
			: base(msg,nested) {}

		public GraphicsException( string msg )
			: base(msg) {}
	}
}
