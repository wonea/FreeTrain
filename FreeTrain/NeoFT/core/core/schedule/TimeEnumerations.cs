using System;

namespace nft.core.schedule
{
	// Yearly seasons
	public enum Season : byte 
	{ Spring, Summer, Autumn, Winter, Dry, Rainy }

	// Each 1/3 part of a month
	public enum TripartiteMonth : byte
	{ Early, Middle, Late }

	// Daily time zone.
	public enum DayNight : byte 
	{　Daybreak,Morning,Afternoon,Evening,EarlyNight,Midnight }

	// Weather types
	// Cloudy is an intermediate state, so assign low level of fine as Cloudy.
	public enum Weather : byte
	{ Fine, Rain, Thunder, Snow, Tempest, Sandstorm, Foggy }

	
	public enum MajorBiome : byte
	{ Tropical, Monsoon, Warm, Dessert, Savanna, Prairie, Mediterranean, Taiga, Tundra, Alpine }
/*
	１ Antarctica 南極域 
	２ Main Taiga 主要タイガ 
	３ Cool Conifer 冷帯針葉樹林 
	４ Cool Mixed 冷帯混合林 
	５ Warm deciduous 温帯落葉樹林 
	６ Warm mixed 温帯混合林 
	７ Warm conifer 温帯針葉樹林 
	８ Tropical Montane 熱帯山地林 
	９ Tropical Seasonal 熱帯季節林 
	１０ Equatorial Evergreen 赤道常緑樹林 
	１１ Cool Crops 冷帯穀倉地帯 
	１２ Warm Crops 温帯穀倉地帯 
	１３ Tropical Dry Forest 熱帯乾燥林 
	１４ Paddylands 稲作地 
	１５ Warm Irrigated 温帯潅漑地 
	１６ Cool Irrigated 冷帯潅漑地 
	１７ Cold Irrigated 寒帯潅漑地 
	１８ Cool Grass/Shrub 冷帯草地／低木地 
	１９ Warm Grass/Shrub 温帯草地／低木地 
	２０ Highland Shrub 高地低木地 
	２１ Med. Grazing ？放牧地 
	２２ Semiarid Woods 半乾燥林地 
	２３ Siberian Parks シベリア公園地？ 
	２４ Heaths, Moors ヒース、荒野 
	２５ Succulent Thorns 多液いばら林 
	２６ Northern Taiga 北タイガ 
	２７ Tropical Savanna 熱帯サバンナ 
	２８ Cool Field/Woods 冷帯平原／林地 
	２９ Warm Field/Woods 温帯平原／林地 
	３０ Warm Forest/Field 温帯森林／林地 
	３１ Cool Forest/Field 冷帯森林／林地 
	３２ Southern Taiga 南タイガ 
	３３ Eastern？ Southern Taiga 東南タイガ？ 
	３４ Tropical Montane 熱帯山地林？ 
	３５ Marsh, Swamp 湿地、沼地 
	３６ Mangroves マングローブ林 
	３７ Low Scrub 低雑木 
	３８ Bogs, Bog Woods 沼、沼地林 
	３９ Hot Desert 高温砂漠 
	４０ Cool Desert 冷帯砂漠 
	４１ Wooded Tundra 林地ツンドラ 
	４２ Tundra ツンドラ 
	４３ Sand Desert 砂漠 
	４４ Polar Desert 極域砂漠 
*/
}
