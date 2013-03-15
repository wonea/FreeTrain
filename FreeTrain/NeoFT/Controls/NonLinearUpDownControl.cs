using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

namespace nft.controls
{
	public enum NonLinearUpDownStyle { ZeroCorrectedLinearStep, GeometricStep, FirstDigitStep };

	/// <summary>
	/// NonLinierUpDownControl の概要の説明です。
	/// </summary>
	public class NonLinearUpDownControl : NumericUpDown
	{
		public override void DownButton()
		{
			int d = (int)base.Value;
			int i = (int)base.Increment;
			switch(style)
			{
				case NonLinearUpDownStyle.ZeroCorrectedLinearStep:
				{
					int mod = d%i;
					if(mod>0) d-=mod;
					else d-=i;
					break;
				}
				case NonLinearUpDownStyle.GeometricStep:
				{
					double e = Math.Log(d,i);
					int n = (int)Math.Floor(e);
					if(e==n) d /= i;
					else d = (int)Math.Pow(i,n);
					break;
				}
				case NonLinearUpDownStyle.FirstDigitStep:
				{
					double e = Math.Log10(d);
					int n = (int)Math.Floor(e);
					int b = (int)Math.Pow(10,n);
					int mod = d%b;
					if(mod>0)
						d-=mod;
					else if(d==b)
						d-=b/10;
					else
						d-=b;
					break;
				}
			}
			base.Value = Math.Max(d,base.Minimum);
		}
		public override void UpButton()
		{
			int d = (int)base.Value;
			int i = (int)base.Increment;
			switch(style)
			{
				case NonLinearUpDownStyle.ZeroCorrectedLinearStep:
				{
					int mod = d%i;
					if(mod>0) d+=i-mod;
					else d+=i;
					break;
				}
				case NonLinearUpDownStyle.GeometricStep:
				{
					double e = Math.Log(d,i);
					int n = (int)Math.Floor(e);
					if(e==n) d *= i;
					else d = (int)Math.Pow(i,n+1);
					break;
				}
				case NonLinearUpDownStyle.FirstDigitStep:
				{
					double e = Math.Log10(d);
					int n = (int)Math.Floor(e);
					int b = (int)Math.Pow(10,n);
					int mod = d%b;
					if(mod>0)
						d+=i-mod;
					else 
						d+=b;
					break;
				}
			}
			base.Value = Math.Min(d,base.Maximum);
		}
		protected NonLinearUpDownStyle style = NonLinearUpDownStyle.ZeroCorrectedLinearStep;
		public NonLinearUpDownStyle Style 
		{
			get{ return style; }
			set{ style = value; }
		}
	}
}
