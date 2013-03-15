using System;

namespace RoadSimulator
{
	public enum Direction : int {NORTH, EAST, SOUTH, WEST};

	public class DirConvertor
	{
		public static Direction rotR(Direction src)
		{
			int d = (int)src;
			d+=1;
			d&=3;
			return (Direction)d;
		}
		public static Direction rotL(Direction src)
		{
			int d = (int)src;
			d+=3;
			d&=3;
			return (Direction)d;
		}
		public static Direction reverse(Direction src)
		{
			int d = (int)src;
			d+=2;
			d&=3;
			return (Direction)d;
		}
	}
	/// <summary>
	/// 
	/// </summary>
	public class Road
	{
		public Road()
		{
			// 
			// TODO: コンストラクタ ロジックをここに追加してください。
			//
		}
		private int[] level = new int[4]{9999,9999,9999,9999};
		public int getLevel(Direction d)
		{
			return level[(int)d];
		}

		public int getMinLevel()
		{
			int a = level[0];
			for( int i=1; i<4; i++ )
				if( a>level[i]) a = level[i];
			return a;
		}

		public void setLevel(Direction d, int lv )
		{
			level[(int)d] = lv;
		}

		public void setLevelOver(Direction d, int lv)
		{
			if(getLevel(d)>lv)
				setLevel(d,lv);
		}
	}
}
