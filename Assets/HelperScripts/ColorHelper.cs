using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelperScripts
{
	public static class ColorHelper
	{
	
		private static readonly List<string> UniqueColors = new (){ "#8b4513",
			"#228b22","#808000","#483d8b","#008b8b","#4682b4","#000080",
			"#9acd32","#7f007f","#8fbc8f","#ffa500","#ffff00", "#7fff00",
			"#8a2be2", "#00ff7f","#e9967a","#dc143c","#00ffff", "#0000ff",
			"#da70d6","#ff00ff","#1e90ff", "#db7093","#f0e68c", "#90ee90",
			"#add8e6","#ff1493","#7b68ee"};
		
		private static System.Random _rnd = new System.Random();
		public static Color GetUniqueColorFromId(int id)
		{
			return StringToColor(UniqueColors[id% UniqueColors.Count]);
		}

		public static Color StringToColor(string color)
		{
			return ColorUtility.TryParseHtmlString(color, out var systemColor) ? systemColor : Color.white;
		}
	}
}