using System;
using nft.framework;
using nft.core.schedule;

namespace nft.core.schedule
{
	public delegate void SeasonEventHandler( int minAltitude, int maxAltitude );
	public delegate void WeatherEventHandler( Weather newWeather );
	public delegate void DayNightEventHandler( DayNight newDayNight );

	/// <summary>
	/// IOverrideTimeTable の概要の説明です。
	/// </summary>
	public interface IClimateController : IHasNameAndID
	{		
		Weather CurWeather { get; }
		Season CurSeason { get; }
		DayNight CurDayNight { get; }
		MajorBiome GetMainBiome( int altitude );
		MajorBiome GetSubBiome( int altitude );
		void SetClock( Clock c );
		void ReleaseClock();
		SeasonEventHandler OnSeasonChanged { get; set; }
		WeatherEventHandler OnWeatherChanged { get; set; }
		DayNightEventHandler OnDayNightChanged { get; set; }
	}
}
