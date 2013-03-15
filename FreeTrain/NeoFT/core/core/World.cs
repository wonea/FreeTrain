using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using nft.framework;
using nft.core.game;
using nft.core.structure;
using nft.util;
using nft.impl.game;

namespace nft.core
{
	/// <summary>
	/// Map of the entire world, consists of one or more <code>Districts</code>. 
	/// </summary>
	[Serializable]
	public class World : IHasNameAndID, IEnumerable
	{
		public World(Size size, int districtUnitSize)
		{
			if( districtUnitSize<=0 )
				throw new ArgumentException("Wrong size paramater","districtUnitSize");
			if( size.Width<=0 || size.Height<=0 )
				throw new ArgumentException("Wrong size paramater","size");
			this.unitSize = districtUnitSize;
			this.size = size;
			this.districtMap = new int[size.Width,size.Height];
		}

		public World(Size size) : this(size,MapGeneService.DefaultUnitSize)
		{	
		}

		public World() : this(new Size(1,1),MapGeneService.DefaultUnitSize)
		{
		}

		#region IHasNameAndID メンバ

		public string ID 
		{
			get	{ return "World:"+ShortID;	}
		}

		internal string ShortID
		{
			get	{ return string.Format("{0:X}", this.GetHashCode()); }
		}

		public string Name
		{
			get	{ return name; }
		}
		protected string name;

		#endregion

		/// <summary>
		/// The Unit voxel count of Districts size which consist in this world.
		/// </summary>
		public int UnitSize
		{
			get { return unitSize; }
		}
		protected readonly int unitSize;

		/// <summary>
		/// The entire size of this World(in UnitSize).
		/// </summary>
		public Size SizeInGrid
		{
			get { return size; }
		}
		protected readonly Size size;

		public Size SizeInVoxel
		{
			get { return new Size(size.Width*unitSize,size.Height*unitSize); }
		}
		

		/// <summary>
		/// Get <code>District</code> at specified grid position.
		/// </summary>
		public IDistrict this[int wx, int wy]
		{
			get
			{
				int n = -1;
				try
				{
					n = districtMap[wx,wy];
					return (IDistrict)districts[n];
				}
				catch(IndexOutOfRangeException)
				{
					if(n<0 || n>=districts.Count)
						Debug.WriteLine(string.Format("wrong index:{0} for districtMap. May be not initialized.",n));
					else
						Debug.WriteLine(string.Format("World[{0},{1}] is out of range.",wx,wy));
					return null;
				}
			}
		}
		protected int[,] districtMap;

		#region IEnumerable メンバ

		public IEnumerator GetEnumerator()
		{
			return districts.GetEnumerator();
		}
		protected ArrayList districts = new ArrayList();
		#endregion

		public void CreateDistricts(ITerrainMap terrain, Rectangle[] _districts, bool silent )
		{
			if(!silent && !(_districts.Length>0))
			{// show confirmation message
				string text = Core.resources["world.create_districts_prompt"].stringValue;
				if(!UIUtil.ConfirmMessage(text,UIMessageType.warning,UIInformLevel.normal))
					return;
			}
			// reset map
			for( int y=0; y<districtMap.GetUpperBound(1); y++ )
				for( int x=0; x<districtMap.GetUpperBound(0); x++ )
					districtMap[x,y] = -1;

			for( int i=0; i<_districts.Length; i++ )
			{
				Rectangle r = _districts[i];
				ProvisionalDistrict dist = new ProvisionalDistrict(this, terrain, r );
				int n = districts.Add(dist);
				for(int y = r.Top; y<=r.Bottom; y++ )		
					for(int x = r.Left; x<=r.Right; x++ )
					{
						if(districtMap[x,y]!=-1)
							throw new ArgumentException("detect overraped rectangles in districts array.");
						districtMap[x,y] = n;
					}
			}
		}

		internal protected void Replace( IDistrict newone, int wx, int wy, bool silent )
		{
			IDistrict old = this[wx,wy];
			if(!old.WorldLocation.Equals(new Point(wx,wy)) || !old.SizeInGrid.Equals(newone.SizeInGrid))
				throw new ArgumentException("cannnot replace. Location or Size does not match.");
			if(!silent && !(old is ProvisionalDistrict))
			{// show confirmation message
				string text = Core.resources["world.district_replace_prompt"].stringValue;
				text = string.Format(text,old.Name);
				if(!UIUtil.ConfirmMessage(text,UIMessageType.warning,UIInformLevel.normal))
					return;
			}
			districts[districts.IndexOf(old)]=newone;
		}

	}

	internal class ProvisionalDistrict : District
	{
		public ProvisionalDistrict(World w, ITerrainMap map, Rectangle area ) : base(w,map,area)
		{
		}

		public Lot[] Lots
		{
			get{ return lots; }
			set{ lots = value; }
		}
		private Lot[] lots;
	}

}
