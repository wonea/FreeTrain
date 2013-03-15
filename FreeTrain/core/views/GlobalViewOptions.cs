using System;
using System.Diagnostics;
using freetrain.world;
using freetrain.framework.graphics;


namespace freetrain.views
{
	public delegate void OptionChangedHandler();
	public enum NightSpriteMode{ AlignClock, AlwaysDay, AlwaysNight };
	/// <summary>
	/// GlobalViewOptions の概要の説明です。
	/// </summary>
	[Serializable]
	public class GlobalViewOptions
	{

		public GlobalViewOptions()
		{
		}
		[NonSerialized]
		public OptionChangedHandler OnViewOptionChanged;

		protected NightSpriteMode _nightSpriteMode = NightSpriteMode.AlignClock;

		public NightSpriteMode nightSpriteMode
		{
			get{ return _nightSpriteMode; }
			set
			{
				_nightSpriteMode = value;
				PictureManager.reset();
				World.world.onAllVoxelUpdated();
				if(OnViewOptionChanged!=null){
					Debug.WriteLine( "###"+OnViewOptionChanged.GetInvocationList().Length );
					OnViewOptionChanged();
				}
			}

		}
		
		public bool useNightView
		{
			get
			{				
				if(nightSpriteMode==NightSpriteMode.AlignClock)
					return World.world.clock.dayOrNight==DayNight.Night;
				else
					return nightSpriteMode==NightSpriteMode.AlwaysNight;
			}
		}	
	}
}
