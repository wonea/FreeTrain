using System;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using freetrain.framework.plugin;
using freetrain.util;

namespace freetrain.finance.stock
{

	[Serializable]
	public class BusinessTypes 
	{
		private Hashtable hash = new Hashtable();
		[NonSerialized]
		private ArrayList array = new ArrayList();

		public void Add(BusinessType type)
		{
			hash.Add(type.id,type);
			array.Add(type);
			Debug.WriteLine(type.id+":"+type.name);
		}

		internal void onWorldLoaded() {

			// reconstruct 'array' field and 'companies' field of each BusinessType 
			// because it is non-serialized.
			array = new ArrayList();
			IDictionaryEnumerator de = hash.GetEnumerator();
			while(de.MoveNext()) {
				BusinessType type = (BusinessType)de.Value;
				type.companies = new Set();
			}
			array.AddRange(hash.Values);
		}

		// progress trend calculation
		internal void progress() {
			IDictionaryEnumerator de = hash.GetEnumerator();
			while(de.MoveNext()) 
			{
				BusinessType type = (BusinessType)de.Value;
				type.trend.progress();
			}			
		}

		public BusinessType this[string id]
		{
			get 
			{
				if( hash.ContainsKey(id) )
					return (BusinessType)hash[id];
				else
					return BusinessType.NullType;
			}
		}

		public BusinessType this[int index]
		{
			get 
			{
				if( array.Count > index )
					return (BusinessType)array[index];
				else
					return BusinessType.NullType;
			}
		}

		public void ResetSuppliers()	{
			
			IDictionaryEnumerator de = hash.GetEnumerator();
			while(de.MoveNext()) {
				BusinessType bt = (BusinessType)de.Value;
				bt._mkt_supplied = 0;
				bt._totalScore = 0;
			}
		}

		public IEnumerator GetEnumerator() {
			return hash.Values.GetEnumerator();
		}

		public int Count {get {return array.Count;} }

		public void Clear() {
			hash.Clear();
		}
	}

	[Serializable]
	public class BusinessType : ITrendTarget
	{
		public readonly string _name;
		internal Trend _trend;
		public readonly string id;

		#region ITrendTarget
		public string name { get{ return _name; }}
		public Trend trend { get{ return _trend;}}
		#endregion

		
		public long marketScale { get { return _mkt_scale; } }
		internal double _acc_fixed;
		internal long _mkt_scale;
		[NonSerialized]
		internal long _mkt_supplied = 0;
		[NonSerialized]
		internal long _totalScore = 0;
		internal int _cycle;
		internal int _amp;
		internal int _depend = 8;
		internal int _progress = 2;
		internal int weightBrand = 5;
		internal int weightMarketing = 5;
		internal int weightQuality = 5;
		internal int weightTotal = 15;
		public Set entries { get { return companies; } }
		[NonSerialized]
		internal Set companies = new Set();

		private BusinessType()
		{
			_name = "Other";
			//! _name = "その他";
			_mkt_scale = 10000000;
		}
		
		public BusinessType( XmlNode node )
		{
			try 
			{
				id = node.Attributes["id"].Value;
				_name = node.Attributes["name"].Value;
				XmlElement account = (XmlElement)XmlUtil.selectSingleNode(node,"account");
				_acc_fixed = double.Parse(account.Attributes["fixed"].Value);
				XmlElement market = (XmlElement)XmlUtil.selectSingleNode(node,"market");
				_mkt_supplied = 0;
				_mkt_scale = long.Parse(market.Attributes["scale"].Value);
				_cycle = int.Parse(market.Attributes["cycle"].Value);
				_amp = int.Parse(market.Attributes["amplitude"].Value);
				_depend = int.Parse(market.Attributes["ex_dependency"].Value);
				_progress = int.Parse(market.Attributes["progressivity"].Value);

				XmlElement weight = (XmlElement)XmlUtil.selectSingleNode(node,"weight");
				weightBrand = int.Parse(weight.Attributes["brand"].Value);
				weightMarketing = int.Parse(weight.Attributes["marketing"].Value);
				weightQuality = int.Parse(weight.Attributes["quality"].Value);
				weightTotal = weightBrand+weightMarketing+weightQuality;
				if( weightTotal == 0 ) weightTotal = 1;
			} 
			catch 
			{
				throw new XmlException("invalid business type definition : "+id+name,null);
			}
			_trend = Trend.randomGenerate(_amp*Trend.RANGE/2,_amp*Trend.RANGE/2,_cycle/2);
		}

		private static BusinessType _Null = null;
		public static BusinessType NullType	{
			get {
				if(_Null == null )
					_Null = new BusinessType();
				return _Null;
			}
		}
	}
}
