using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using freetrain.framework;

namespace freetrain.framework.plugin
{
	/// <summary>
	/// Allows objects from plug-ins to be de-serialized.
	/// </summary>
	public class PluginSerializationBinder : SerializationBinder
	{
		public override System.Type BindToType(string assemblyName, string typeName) {
			Type t;
			
			t = Type.GetType(typeName);
			if(t!=null)		return t;

			Trace.WriteLine("binding "+typeName);

			// try assemblies of plug-ins
			foreach( Contribution cont in Core.plugins.publicContributions ) {
				 Assembly asm = cont.assembly;
				if(asm!=null) {
					t = asm.GetType(typeName);
					if(t!=null)	return t;
				}
			}
			Trace.WriteLine("not found");
			return null;
		}
	}
}
