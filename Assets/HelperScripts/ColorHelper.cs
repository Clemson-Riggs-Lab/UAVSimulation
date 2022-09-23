using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelperScripts
{
	public static class ColorHelper
	{
	
		private static List<string> _uniqueColors = new (){ "Red", "Green", "Blue", "Yellow", "Orange", "Purple", "Black",
			"White", "Grey", "Brown", "Cyan", "Magenta", "Teal", "Silver", "Clear", "Maroon", "Olive",
			"Green", "Navy", "Dark Blue", "Dark Green", "Dark Red", "Light Blue" };
		
		private static System.Random _rnd = new System.Random();
		public static Color GetUniqueColorFromId(int id)
		{
			return StringToColor(_uniqueColors[id]);
		}

		public static Color StringToColor(string color)
		{
			return ColorUtility.TryParseHtmlString(color, out var systemColor) ? systemColor : Color.white;
		}
	}
}