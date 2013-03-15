using System;
using System.Collections;
using System.Xml;
using freetrain.framework;
using freetrain.framework.plugin;

namespace freetrain.contributions.train
{
	/// <summary>
	/// Parameterized train contribution implementation
	/// where an user can specify (a) head, (b) tail, and (c) other intermediate
	/// cars separately.
	/// </summary>
	[Serializable]
	public class PatternTrainImpl : AbstractTrainContributionImpl
	{
		/// <summary>
		/// Parses a train contribution from a DOM node.
		/// </summary>
		/// <exception cref="XmlException">If the parsing fails</exception>
		public PatternTrainImpl( XmlElement e ) : base(e) {
			config = (XmlElement)XmlUtil.selectSingleNode(e,"config");
		}

		/// <summary>
		/// &lt;config> element in the plug-in xml file.
		/// </summary>
		private XmlElement config;

		/// <summary>
		/// Map from a char 'x' to TrainCarContribution
		/// </summary>
		private readonly IDictionary cars = new Hashtable();

		/// <summary>
		/// Map from length to its composition.
		/// </summary>
		private readonly IDictionary compositions = new Hashtable();

		protected internal override void onInitComplete() {
			base.onInitComplete();

			XmlNodeList lst = config.SelectNodes("car");
			foreach( XmlElement e in lst )
				cars.Add( e.Attributes["char"].Value[0], getCarType(e) );
			
			lst = config.SelectNodes("composition");
			foreach( XmlElement e in lst )
				loadComposition(e);

			config = null;
		}

		private TrainCarContribution getCarType( XmlElement e ) {
			string idref = e.Attributes["ref"].Value;
			if(idref==null)	throw new FormatException("ref attribute is missing");
			//! if(idref==null)	throw new FormatException("ref属性がありません");

			TrainCarContribution contrib = (TrainCarContribution)Core.plugins.getContribution(idref);
			if(contrib==null)	throw new FormatException(
				string.Format( "id='{0}' is missing TrainCar contribution", idref ));
                //! string.Format( "id='{0}'のTrainCarコントリビューションがありません", idref ));

			return contrib;
		}

		private void loadComposition( XmlElement e ) {
			string comp = e.InnerText;
			ArrayList a = new ArrayList();

			while(comp.Length!=0) {
				char head = comp[0];
				comp = comp.Substring(1);

				if( Char.IsWhiteSpace(head) )
					continue;	// ignore whitespace

				// otherwise look up a table
				TrainCarContribution car = (TrainCarContribution)cars[head];
				if(car==null)
					throw new FormatException("The following characters are undefined: "+head);
					//! throw new FormatException("次の文字は定義されていません:"+head);
				a.Add(car);
			}

			compositions.Add( a.Count, a.ToArray(typeof(TrainCarContribution)) );
		}

		public override TrainCarContribution[] create( int length ) {
			TrainCarContribution[] r = (TrainCarContribution[])compositions[length];
			if(r==null)		return null;
			else			return (TrainCarContribution[])r.Clone();
		}

	}
}
