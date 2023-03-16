using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelperScripts
{
	public static class ColorHelper
	{
	
		private static readonly List<string> UniqueColors = new (){ "#FF5733", "#FFBD33", "#E8FF33", "#9FFF33", "#33FF7D",
			"#33FFD6", "#33B5FF", "#335EFF", "#7D33FF", "#D633FF", "#FF33AE", "#FF3357", "#9C3333", "#E8A1B1", "#A1E8B3",
			"#B3E8A1", "#A1B3E8", "#B1E8A1", "#E8A1CC", "#A1E8E2", "#E2E8A1", "#A1E2E8", "#E8A1C4", "#A1E8E8", "#E8C3A1",
			"#E8A1A1", "#A1E8A4", "#A4E8A1", "#A1A4E8", "#C8A1E8", "#E8A4B2", "#B2E8A4", "#A4B2E8", "#E8B2A4", "#B2A4E8",
			"#E8A4D5", "#D5A4E8", "#A4D5E8", "#FFE033", "#FFA433", "#33FFA2", "#33A2FF", "#A233FF", "#FF33D1", "#FF33A6",
			"#D10000", "#D17700", "#D1C400", "#9FD100", "#00D139", "#00D1B1", "#009FD1", "#2E00D1", "#9F00D1", "#D100B1",
			"#D11D00", "#D1A800", "#C0D100", "#00D14E", "#00D1C9", "#006CD1", "#6900D1", "#D100C9", "#D15A00", "#D1FF00",
			"#00FF19", "#00FFE3", "#0081FF", "#FF00D1", "#FF0078", "#780000", "#785000", "#787800", "#4C7800"};
		
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