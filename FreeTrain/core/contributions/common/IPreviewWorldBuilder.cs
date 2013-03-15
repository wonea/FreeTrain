using System;
using System.Drawing;
using System.Collections;
using freetrain.world;

namespace freetrain.contributions.common
{
	/// <summary>
	/// IPreviewWorldBuilder の概要の説明です。
	/// </summary>
	public interface IPreviewWorldBuilder
	{
		World CreatePreviewWorld(Size minsizePixel, IDictionary options);
	}
}
