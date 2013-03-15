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
	/// <summary>
	/// The Trend Information
	/// </summary>
	[Serializable]
	public class Trend
	{				
		static public readonly int RANGE = 100000;
		static private readonly int COEFF = 10;
		static private int uptimes { get { return Economy.UpdateTimesInADay; } }
		
		public int currentLevel;
		private int turningLevel;
		private int targetLevel;
		public double attenuation = 0.3;

		public int reasonableLevel 
		{
			get {
				return targetLevel; 
			}
			set { 
				targetLevel = value;
				setTurningLevel();
			}
		}

		internal double velocity;
		internal double acceleration;
		private int _steps;
		public int stepCounts { get { return _steps; } }
		static private RandomEx rand { get { return Economy.rand; } }

		static public Trend randomGenerate(int cur_amp, int days )
		{
			return new Trend(rand.Next2D(cur_amp), 0, days);
		}

		static public Trend randomGenerate(int cur_amp, int reasn_amp, int days )
		{
			return new Trend(rand.Next2D(cur_amp), rand.Next2D(reasn_amp), days);
		}

		public Trend(int current, int reasonable, int days)
		{
			currentLevel = current;
			setParams(reasonable,days);
			velocity = rand.Next2D(Math.Abs(reasonable-current)/_steps);
		}

		public Trend()
		{
			currentLevel = 0;
			reasonableLevel = 0;
			acceleration = 0;
			velocity = 0;
			_steps = 0;
		}
		
		private void setTurningLevel()
		{
			turningLevel = (int)(targetLevel*(1-attenuation)+currentLevel*attenuation);
		}

		private void calcStepCount(int days)
		{
			_steps = days*uptimes;
			_steps *= _steps;
			_steps /= COEFF;
		}

		public void setGap( int gap, int days )
		{
			targetLevel = 0;
			currentLevel = checkOverflow(currentLevel - gap);
			calcStepCount(days);
		}

		public void setParams( int reasonable, int days )
		{
			reasonableLevel = checkOverflow(reasonable);
			calcStepCount(days);
		}

		// progress trend calculation
		public void progress()
		{
			//Debug.Write("$"+reasonableLevel+"-"+currentLevel+":"+velocity+":"+acceleration);
			//_steps++;

			int gap = turningLevel - currentLevel;
			acceleration = gap/_steps;
			double prv = velocity;
			velocity += acceleration;
			if( (prv * velocity) <= 0 )	setTurningLevel();
			currentLevel = (int)(currentLevel + velocity);
		}

		public long apply(long val)
		{
			int level = currentLevel;
			return (long)(val*(1.0+(level)/(RANGE*4.0)));
		}

		public long apply(long val, double intense)
		{
			int level = (int)(intense*currentLevel);
			return (long)(val*(1.0+(level)/(RANGE*4.0)));
		}

		private int checkOverflow(int source)
		{
			int safeRange = (int)(RANGE*0.8);
			if( source < -safeRange ) source = -safeRange;
			else if( source > safeRange ) source = safeRange;
			return source;
		}
	}
}
