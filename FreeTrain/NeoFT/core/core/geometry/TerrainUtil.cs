using System;

namespace nft.core.geometry
{
	/// <summary>
	/// TerrainUtil の概要の説明です。
	/// </summary>
	public class TerrainUtil
	{
		static readonly short[] vtx_step = new short[]{0,1,2,2,4,4,4,4,8};
		static int[] workarray = new int[4];
		static int[] idx = new int[4];

		/// <summary>
		/// ボクセルの頂点を規定パターンに合うように補正する		/// 
		/// </summary>
		/// <param name="vertices"></param>
		/// <returns>頂点の最小値を返す</returns>
		static public int CorrectVoxelVertices(ref int[] vertices)
		{
			// MEMO:minまたはmaxと異なる値は、全てmed中間値とするべし
			vertices.CopyTo(workarray,0);
			for( int i=0; i<4; i++)
				idx[i] = i;
			Array.Sort(workarray,idx);
			int minimum = workarray[0];
			for( int i=0; i<4; i++){
				workarray[i]-=minimum;
				vertices[i]-=minimum;
			}

			// 全ての面が等しい
			if(workarray[0]==workarray[3])
				return minimum;
			// 最大値と最小値の差
			int gap = workarray[3]-workarray[0];
			// 最大値と最小値の差を0,1,2,4,8のいずれかに縮小補正
			if(gap>8) gap = 8;
			else gap = vtx_step[gap];

			int tmp;
			// 三点が最小値
			if(workarray[0]==workarray[2])
			{
				vertices[idx[3]]=workarray[0]+gap;
				return minimum;
			}
			// 三点が最大値
			if(workarray[1]==workarray[3])
			{
				vertices[idx[0]]=workarray[3]-gap;
				return minimum;
			}
			// 二点が最小値
			if(workarray[0]==workarray[1])
			{
				// 二点が最小値かつ二点が最大値
				if(workarray[2]==workarray[3])
				{
					// 高さの中点を基準にする
					int org = (workarray[1]+workarray[2]-gap)>>1;
					vertices[idx[0]]=vertices[idx[1]]=org;
					vertices[idx[2]]=vertices[idx[3]]=org+gap;
					return minimum;
				}
				// 二点が最小値かつ一点のみが最大値
				else
				{
					// 最小値を基準にする	
					tmp = workarray[0]+gap;
					vertices[idx[3]] = tmp;
					if( tmp < workarray[2] )
						vertices[idx[2]] = tmp;
					else
						vertices[idx[2]]=workarray[0]+(gap>>1);
					return minimum;
				}
			}
			// 二点が最大値かつ一点のみが最小値
			if(workarray[2]==workarray[3])
			{
				// 最大値を基準にする
				tmp =workarray[3]-gap;
				vertices[idx[0]] = tmp;
				if( workarray[1] < tmp )
					vertices[idx[1]] = tmp;
				else
					vertices[idx[1]]=vertices[idx[0]]+(gap>>1);
				return minimum;
			}
			else 
			{
				// 最大値と最小値が各一点づつ
				// 高さの中点を基準にする
				int org = (workarray[0]+workarray[3]-gap)>>1;
				vertices[idx[0]]=org;
				vertices[idx[3]]=org+gap;
				tmp = org+(gap>>1);
				if(workarray[1]<org)
					vertices[idx[1]] = org;
				else
					vertices[idx[1]] = tmp;
				if(vertices[idx[3]]<workarray[2])
					vertices[idx[2]]=vertices[idx[3]];
				else
					vertices[idx[2]] = tmp;
				//return minimum;
			}
			return minimum;
		}
	}
}
