using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using freetrain.world;
using freetrain.util;
using freetrain.framework.plugin;

namespace freetrain.finance.stock
{
	internal enum TargetType : int { ECONOMY, BUSINESS, COMPANY }
	internal enum RandTableType : int { UNKNOWN, TARGET, EFFECT, ACTION }
	internal enum EffectedField : int 
	{
		unknwon,
		trend,
		marketscale,
		capital,
		sales,
		brand,
		marketing,
		quality,
		rationality,
		development,
		stocktotal,
		stockshare
	}

	public interface ITrendTarget
	{
		string name { get; }
		Trend trend { get; }
	}

	[Serializable]
	public abstract class EventElement
	{
		public string id {get {return _id;} }
		protected string _id;
	}


	/// <summary>
	/// the abstract target class
	/// </summary>
	[Serializable]
	public abstract class Target : EventElement
	{
		abstract public string getTargetNames();
		// returns as SingleTarget class
		abstract public IEnumerator getEnumerator();
	}


	/// <summary>
	/// the basic target class
	/// </summary>
	[Serializable]
	public class SingleTarget : Target
	{
		private TargetType type;
		private string rid;
		//private string name;

		protected SingleTarget(){}
		
		public SingleTarget( Company com ) 
		{
			rid = com.id;
			type = TargetType.COMPANY;
		}

		public SingleTarget( BusinessType bType ) 
		{
			rid = bType.id;
			type = TargetType.BUSINESS;
		}

		// given node is <target.../>
		public SingleTarget(XmlNode e)
		{
			String str;
			try 
			{
				str = e.Attributes["type"].Value;
			} 
			catch {	throw new FormatException("missing target type"); }
			if( str.Equals("economy") )
			{
				type = TargetType.ECONOMY;
				return;
			}

			try 
			{
				rid = e.Attributes["rid"].Value;
			} 
			catch {	throw new FormatException("missing target rid"); }
            if( str.Equals("business") ) 
			{
				type = TargetType.BUSINESS;
			}
			else if( str.Equals("company") ) 
			{
				type = TargetType.COMPANY;
			}
			else
				throw new FormatException("invalid target type :"+str);
			Debug.WriteLine("read target:"+getTargetNames());
		}

		private ITrendTarget theObj
		{
			get
			{
				switch(type)
				{
					case TargetType.COMPANY:
						return getComapny();
					case TargetType.BUSINESS:
						return getBussiness();
					default:
						return Economy.theInstance;
				}
			}
		}
		private BusinessType getBussiness(){ return Economy.Businesses[rid]; }
		private Company getComapny(){ return Economy.Companies[rid]; }

		#region getter and setter of each field
		
		public Trend trend { get { return theObj.trend; } }
		public long marketscale 
		{ 
			get { return getBussiness()._mkt_scale; }
			set { getBussiness()._mkt_scale = value; }
		}
		public long capital 
		{ 
			get { return getComapny()._capital; }
			set { getComapny()._capital = value; }
		}
		public long sales 
		{ 
			get { return getComapny()._salesIdeal; }
			set { getComapny()._salesIdeal = value; }
		}
		public int brand 
		{ 
			get { return getComapny()._brand; }
			set { getComapny()._brand = value; }
		}
		public int marketing 
		{ 
			get { return getComapny()._marketing; }
			set { getComapny()._marketing = value; }
		}
		public int quality 
		{ 
			get { return getComapny()._quality; }
			set { getComapny()._quality = value; }
		}
		public int rationality 
		{ 
			get { return getComapny()._rationality; }
			set { getComapny()._rationality = value; }
		}
		public int development 
		{ 
			get { return getComapny()._development; }
			set { getComapny()._development = value; }
		}
		public int stocktotal 
		{ 
			get { return getComapny().stockData._numTotal; }
			set { getComapny().stockData._numTotal = value; }
		}
		public int stockshare
		{ 
			get { return stocktotal-getComapny().stockData._numInMarket; }
			set { getComapny().stockData._numInMarket = stocktotal-value; }
		}
		#endregion

		public override string getTargetNames() {
			return theObj.name;
		}

		public override IEnumerator getEnumerator() {
			ArrayList tmp = new ArrayList(1);
			tmp.Add(this);
			return tmp.GetEnumerator();
		}
	}


	/// <summary>
	/// the array of targets
	/// </summary>
	[Serializable]
	public class ArrayedTarget : Target
	{
		protected ArrayList array = new ArrayList(1);

		protected ArrayedTarget(){}

		internal ArrayedTarget(XmlNode node)
		{
			IEnumerator ie = node.ChildNodes.GetEnumerator();
			while(ie.MoveNext())
			{
				XmlNode cn = (XmlNode)ie.Current;

				if(cn.Name.Equals("target"))
				{
					array.Add(new SingleTarget(cn));
				}
				else if(cn.Name.Equals("randomize")) 
				{
					if( cn.Attributes["type"].Value.Equals("target") )
						array.Add(new RandomTarget(cn));
				}
			}
		}

		public override string getTargetNames() 
		{
			int n = array.Count;
			string result = "";
			switch(n)
			{
				case 0:
					break;
				case 1:
					result = this[0].getTargetNames();
					break;
				case 2:
					result = this[0].getTargetNames()+"‚Æ"+this[1].getTargetNames();
					break;
				default:
					result = this[0].getTargetNames()+"‚È‚Ç";
					break;
			}
			return result;
		}

		public override IEnumerator getEnumerator()
		{
			return array.GetEnumerator();
		}

		/// <summary>
		/// give actions count
		/// </summary>
		public int Count { get { return array.Count; } }

		internal Target this[int index]
		{
			get { return (Target)array[index]; }
		}
	}

	
	/// <summary>
	/// randomized target class
	/// </summary>
	[Serializable]
	public class RandomTarget : Target
	{
		private RandomTable table;
		private Target latestTarget=null;

		// given node is <randomize type="action".../>
		internal RandomTarget(XmlNode node) 
		{
			table = RandomTable.CreateTable( node, RandTableType.TARGET);
		}

		private object Parse(XmlNode node) 
		{
			ArrayedTarget array = new ArrayedTarget(node);
			// if single action, regists non-arrayed action object.
			if( array.Count == 1 )
				return array[0];
			else
				return array;
		}

		public override string getTargetNames() 
		{
			if( latestTarget == null )
				return "";
			else
				return latestTarget.getTargetNames(); 
		}
		public override IEnumerator getEnumerator()
		{
			latestTarget = (Target)table.select();
			return latestTarget.getEnumerator();
		}
	}


	/// <summary>
	/// the abstract effect class
	/// </summary>
	[Serializable]
	public abstract class Effect : EventElement
	{
		public abstract void apply(Target target);
	}


	/// <summary>
	/// the basic effect class
	/// </summary>
	[Serializable]
	public class SingleEffect : Effect
	{
		private EffectedField field;
		private bool byRatio;
		private double mValue;
		private double dispersion;
		private int days;
		static private RandomEx rand = new RandomEx();

		protected SingleEffect(){}

		public SingleEffect(XmlNode e)
		{
			String str;
			try 
			{
				str = e.Attributes["on"].Value;
			} 
			catch {	throw new FormatException("missing attribute 'on'"); }
			field = convert(str);
			if( field == EffectedField.unknwon )
				throw new FormatException("invalid field name in attribute 'on' :"+str);

			try 
			{
				str = e.Attributes["by"].Value;
			} 
			catch {	throw new FormatException("missing attribute 'by'"); }
			if( str.Equals("ratio") )
				byRatio = true;
			else if( str.Equals("absolute") )
				byRatio = false;
			else
				throw new FormatException("invalid value in attribute 'by' :"+str);

			try 
			{
				str = e.Attributes["value"].Value;
				try 
				{
					mValue = double.Parse(str);
				} 
				catch {	throw new FormatException("invalid value in attribute 'value' :"+str); }
			} 
			catch {	throw new FormatException("missing attribute 'value'"); }

			try 
			{
				str = e.Attributes["dispersion"].Value;
				try 
				{
					dispersion = double.Parse(str);
				} 
				catch {	throw new FormatException("invalid value in attribute 'dispersion' :"+str); }
			} 
			catch {	throw new FormatException("missing attribute 'dispersion'"); }
			
			if( field == EffectedField.trend ) 
			{
				try 
				{
					str = e.Attributes["days"].Value;
					try 
					{
						days = int.Parse(str);
					} 
					catch {	throw new FormatException("invalid value in attribute 'days' :"+str); }
				} 
				catch {	throw new FormatException("missing attribute 'days'"); }
			}
			Debug.WriteLine("read effect:"+field.ToString()+(byRatio?"/r":"/a")+mValue.ToString()+dispersion.ToString());
		}

		private EffectedField convert(string str)
		{
			if( str.Equals("trend") ) return EffectedField.trend;
			if( str.Equals("marketscale") ) return EffectedField.marketscale;
			if( str.Equals("capital") ) return EffectedField.capital;
			if( str.Equals("sales") ) return EffectedField.sales;
			if( str.Equals("brand") ) return EffectedField.brand;
			if( str.Equals("marketing") ) return EffectedField.marketing;
			if( str.Equals("quality") ) return EffectedField.quality;
			if( str.Equals("rationality") ) return EffectedField.rationality;
			if( str.Equals("development") ) return EffectedField.development;
			if( str.Equals("stocktotal") ) return EffectedField.stocktotal;
			if( str.Equals("stockshare") ) return EffectedField.stockshare;
			return EffectedField.unknwon;
		}

		
		public override void apply(Target target)
		{
			IEnumerator ie = target.getEnumerator();
			double v = mValue;
			v += rand.NextDouble2D(dispersion,100);
			while( ie.MoveNext() )
			{
				SingleTarget sgl = (SingleTarget)ie.Current;
				switch( field )
				{
						//case EffectedField.unknwon:
					case EffectedField.trend:
						apptyVal(v,sgl.trend);
						break;
					case EffectedField.marketscale:
						sgl.marketscale=apptyVal(v,sgl.marketscale);
						break;
					case EffectedField.capital:
						sgl.capital=apptyVal(v,sgl.capital);
						break;
					case EffectedField.sales:
						sgl.sales=apptyVal(v,sgl.sales);
						break;
					case EffectedField.brand:
						sgl.brand=apptyVal(v,sgl.brand);
						break;
					case EffectedField.marketing:
						sgl.marketing=apptyVal(v,sgl.marketing);
						break;
					case EffectedField.quality:
						sgl.quality=apptyVal(v,sgl.quality);
						break;
					case EffectedField.rationality:
						sgl.rationality=apptyVal(v,sgl.rationality);
						break;
					case EffectedField.development:
						sgl.development=apptyVal(v,sgl.development);
						break;
					case EffectedField.stocktotal:
						sgl.stocktotal=apptyVal(v,sgl.stocktotal);
						break;
					case EffectedField.stockshare:
						sgl.stockshare=apptyVal(v,sgl.stockshare);
						break;
				}
			}
		}
		
		#region applyVal() methods
		private int apptyVal( double v, int n )
		{
			if( byRatio ) 
				n = (int)(n*v);
			else 
				n = (int)(n+v);
			return n;
		}
		private long apptyVal( double v, long n )
		{
			if( byRatio ) 
				n = (long)(n*v);
			else 
				n = (long)(n+v);
			return n;
		}
		private void apptyVal( double v, Trend t )
		{
			int n = t.reasonableLevel;
			n += Trend.RANGE;
			if( byRatio ) 
				n = (int)(v*n);
			else 
				n = (int)(v+n);
			t.setParams(n-=Trend.RANGE,days);
		}
		#endregion
	}


	/// <summary>
	/// the array of effects
	/// </summary>
	[Serializable]
	public class ArrayedEffect : Effect
	{
		protected ArrayList array = new ArrayList(1);

		protected ArrayedEffect(){}
	
		internal ArrayedEffect(XmlNode node)
		{
			IEnumerator ie = node.ChildNodes.GetEnumerator();
			while(ie.MoveNext())
			{
				XmlNode cn = (XmlNode)ie.Current;

				if(cn.Name.Equals("effect"))
				{
					array.Add(new SingleEffect(cn));
				}
				else if(cn.Name.Equals("randomize")) 
				{
					if( cn.Attributes["type"].Value.Equals("effect") )
						array.Add(new RandomEffect(cn));
				}
			}
		}

		/// <summary>
		/// apply all effects
		/// </summary>
		/// <param name="target">target to be applied</param>
		public override void apply(Target target) 
		{
			int n = array.Count;
			for( int i=0; i<n; i++ )
				((Effect)array[i]).apply(target);
		}

		/// <summary>
		/// give actions count
		/// </summary>
		public int Count { get { return array.Count; } }

		internal Effect this[int index]
		{
			get { return (Effect)array[index]; }
		}
	}


	/// <summary>
	/// randomized effect class
	/// </summary>
	[Serializable]
	public class RandomEffect : Effect
	{
		private RandomTable table;

		// given node is <randomize type="action".../>
		internal RandomEffect(XmlNode node) 
		{
			table = RandomTable.CreateTable(node, RandTableType.EFFECT);
		}

		public override void apply(Target target)
		{
			((Effect)table.select()).apply(target);
		}
	}

	
	/// <summary>
	/// the abstract action class
	/// </summary>
	[Serializable]
	public abstract class Action : EventElement
	{
		protected string msgLatest = "";
		public string getLatestEventMessage(){ return msgLatest; }
		public abstract void execute();
	}

	/// <summary>
	/// the single action, the set of 'target' and 'effect'
	/// </summary>
	[Serializable]
	public class SingleAction : Action
	{
		private Target target;
		private Effect effect;
		private string msgTempl = "";

		protected SingleAction(){}

		// given node is <action.../>
		internal SingleAction(XmlNode node) 
		{
			ArrayedTarget a_target = new ArrayedTarget(node);
			if( a_target.Count == 1 )
				target = a_target[0];
			else
				target = a_target;

			ArrayedEffect a_effect = new ArrayedEffect(node);
			if( a_effect.Count == 1 )
				effect = a_effect[0];
			else
				effect = a_effect;
			XmlNode dsc = node.SelectSingleNode("description");
			if( dsc != null )
				msgTempl = dsc.InnerText;
		}

		public override void execute()
		{			
			effect.apply(target);
			msgLatest = (string)msgTempl.Clone();
			msgLatest = msgLatest.Replace("%target%",target.getTargetNames());
		}
	}


	/// <summary>
	/// the array of actions
	/// </summary>
	[Serializable]
	public class ArrayedAction : Action
	{
		protected ArrayList array = new ArrayList(1);

		protected ArrayedAction(){}

		internal ArrayedAction(XmlNode node)
		{
			IEnumerator ie = node.ChildNodes.GetEnumerator();
			while(ie.MoveNext())
			{
				XmlNode cn = (XmlNode)ie.Current;

				if(cn.Name.Equals("action"))
				{
					array.Add(new SingleAction(cn));
				}
				else if(cn.Name.Equals("randomize")) 
				{
					array.Add(new RandomAction(cn));
				}
			}
		}

		/// <summary>
		/// execute each actions
		/// </summary>
		public override void execute()
		{			
			msgLatest = "";
			int n = array.Count;
			for( int i=0; i<n; i++ ) 
			{
				((Action)array[i]).execute();
				msgLatest += ((Action)array[i]).getLatestEventMessage()+"\n";
			}
			if( msgLatest.Length > 0 )
				msgLatest.Remove(msgLatest.Length-1,1);
		}

		/// <summary>
		/// give actions count
		/// </summary>
		public int Count { get { return array.Count; } }

		internal Action this[int index]
		{
			get { return (Action)array[index]; }
		}

	}


	/// <summary>
	/// randomized action class
	/// </summary>
	[Serializable]
	public class RandomAction : Action
	{
		private RandomTable table;

		// given node is <randomize type="action".../>
		internal RandomAction(XmlNode node) 
		{
			table = RandomTable.CreateTable(node, RandTableType.ACTION);
		}

		/// <summary>
		/// execute the random selected action
		/// </summary>
		public override void execute()
		{		
			Action eevt = (Action)table.select();
			eevt.execute();
			msgLatest = eevt.getLatestEventMessage();
		}

	}


	/// <summary>
	/// the EconomicalEvent class
	/// </summary>
	[Serializable]
	public class EconomicalEvent : ArrayedAction
	{
		public int frequency = -1;
		protected ArrayedAction follows;

		// given node is <define type="event".../>
		internal EconomicalEvent(XmlNode node) : base(node)
		{
			_id = node.Attributes["id"].Value;
			if(_id==null)
				throw new FormatException("event id is missing");
			IEnumerator ie = node.ChildNodes.GetEnumerator();
			while(ie.MoveNext())
			{
				XmlNode cn = (XmlNode)ie.Current;
				if(cn.Name.Equals("frequency")) 
				{
					string str = cn.InnerText;
					if(str==null) str="0";
					try { frequency = int.Parse(str); }	
					catch {}
					if( frequency == -1 || frequency > 7)
						throw new FormatException("invalid <frequency> :"+str);
				}
				else if(cn.Name.Equals("follow_events")) 
				{
					follows = new ArrayedAction(cn);
				}
			}

		}

		public Action followedEvent{ get { return follows; } }

		static public EconomicalEvent Parse(XmlNode node)
		{
			return new EconomicalEvent(node);
		}

	}

	/// <summary>
	/// randomized table abstract class
	/// </summary>
	[Serializable]
	internal abstract class RandomTable
	{
		static protected Random rand = new Random();
		protected RandTableType _type;
		public RandTableType type{ get { return _type; } }
		
		static internal RandomTable CreateTable(XmlNode node, RandTableType tableType) 
		{
			RandomTable newTable;
			try {
				string attr = node.Attributes["for_each"].Value;
				if( tableType != RandTableType.TARGET )
					throw new FormatException("The 'for_each' attribute is available only in the target definition.");
				else
					newTable = new ForEachTable( node, tableType);
			} catch (NullReferenceException nre) {
				newTable = new BranchedTable( node, tableType);
			}
			newTable._type = tableType;
			return newTable;
		}

		public abstract object select();
	}

	/// <summary>
	/// randomized table class
	/// </summary>
	[Serializable]
	internal class BranchedTable : RandomTable
	{
		private Hashtable hash = new Hashtable();
		private int weight_total = 0;

		[Serializable]
		private class _Item 
		{
			public int weight;
			public object obj;
			public _Item( object _obj, int _weight )
			{
				this.weight = _weight;
				this.obj = _obj;
			}
		}

		internal BranchedTable(XmlNode node, RandTableType rtType) 
		{
			_type = rtType;
			IEnumerator ie = node.ChildNodes.GetEnumerator();
			while(ie.MoveNext())
			{
				XmlNode cn = (XmlNode)ie.Current;
				if(cn.Name.Equals("branch")) 
				{
					int weight = 1;
					string str = null;
					try {
						str = cn.Attributes["weight"].Value;
					} 
					catch{}
					if(str!=null) 
					{
						try 
						{ 
							weight = int.Parse(str);
							if( weight <1)
								throw new FormatException("invalid weight :"+str);
						}
						catch 
						{
							throw new FormatException("invalid weight :"+str);
						}
					}
					add( Parse(cn, rtType), weight);
				}
			}
		}

		private object Parse(XmlNode node, RandTableType rtType) 
		{
			switch( rtType ) 
			{
				case RandTableType.ACTION:
					ArrayedAction aArray = new ArrayedAction(node);
					// if single action, regists non-arrayed action object.
					if( aArray.Count == 1 )
						return aArray[0];
					else
						return aArray;				
				case RandTableType.EFFECT:
					ArrayedEffect eArray = new ArrayedEffect(node);
					// if single action, regists non-arrayed action object.
					if( eArray.Count == 1 )
						return eArray[0];
					else
						return eArray;
				case RandTableType.TARGET:
					ArrayedTarget tArray = new ArrayedTarget(node);
					// if single action, regists non-arrayed action object.
					if( tArray.Count == 1 )
						return tArray[0];
					else
						return tArray;
				default:
					throw new FormatException("Invalid type attribute in the randomized tag");
					break;
			}
			return null;
		}

		public void add( object obj, int weight ) 
		{
			hash.Add(obj,new _Item(obj,weight)); 
			weight_total += weight;
		}

		public void remove( object obj ) {
			_Item target = (_Item)hash[obj];
			if( target != null ) {
				weight_total-=target.weight;
				hash.Remove(obj); 
			}
		}

		public void removeAll() {
			hash.Clear();
			weight_total = 0;
		}

		public override object select() 
		{
			int t = rand.Next(weight_total);
			IEnumerator e = hash.Values.GetEnumerator();
			while( e.MoveNext() )
			{
				_Item i = (_Item)e.Current;
				if( t<i.weight )
					return i.obj;
				else
					t-=i.weight;
			}
			// unexpected loop out
			Debug.Assert(false);
			return null;
		}
	}

	[Serializable]
	internal class ForEachTable : RandomTable 
	{
		private TargetType target;

		internal ForEachTable(XmlNode node, RandTableType rtType) {
			_type = rtType;
			string attr = node.Attributes["for_each"].Value;
			if( attr.Equals("business")) target = TargetType.BUSINESS;
			else if( attr.Equals("company")) target = TargetType.COMPANY;
			else throw new FormatException("invalid 'for_each' target :"+attr);
		}

		public override object select() {
			int n;
			if( target == TargetType.COMPANY ) {
				n = Economy.Companies.Count;
				return new SingleTarget( Economy.Companies[rand.Next(n)] );
			}
			else {
				n = Economy.Businesses.Count;
				return new SingleTarget(Economy.Businesses[rand.Next(n)] );
			}
		}
	}
}
