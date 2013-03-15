using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace XmlCombiner
{
	/// <summary>
	/// Combines all plugin.xml into one file
	/// </summary>
	class XmlCombiner
	{
		[STAThread]
		static void Main( string[] args ) {
			if(args.Length==0) {
				Console.Error.WriteLine("Usage: XmlCombiner <plug-in dir> ...");
				Console.Error.WriteLine("  Concatenate all plugin.xml files into one file");
				return;
			}

			ArrayList list = new ArrayList();
			foreach( string dir in args )
				list.AddRange( Directory.GetDirectories(dir) );

			string[] subdirs = (string[])list.ToArray(typeof(string));
			
			XmlDocument dom = new XmlDocument();
			XmlElement root = dom.CreateElement("plugins");
			dom.AppendChild(root);

			for( int i=0; i<subdirs.Length; i++ ) {
				if( Path.GetFileName(subdirs[i]).Equals("CVS") )	continue;	// skip CVS directory
				
				XmlDocument plugin = new XmlDocument();
				XmlValidatingReader reader = new XmlValidatingReader(
					new XmlTextReader(new FileStream(
						Path.Combine(subdirs[i],"plugin.xml"), FileMode.Open ) ) );
				reader.ValidationType = ValidationType.None;

				plugin.Load( reader );
				root.AppendChild( dom.ImportNode( plugin.DocumentElement, true ) );
			}

			Console.WriteLine("<?xml version='1.0' encoding='UTF-8'?>");
			XmlTextWriter writer = new XmlTextWriter(Console.Out);
			writer.Formatting = Formatting.Indented;
			dom.WriteContentTo( writer );
		}
	}
}
