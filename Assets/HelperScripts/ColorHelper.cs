using System;
using UnityEngine;

namespace HelperScripts
{
	public static class ColorHelper
	{
		// generate a color based on id such that no two id's will generate the same color.
		//the colors have to be contrasting enough to be seen.
		// id is a number between 0 and 20.
		// random number generator
		private static System.Random rnd = new System.Random();
		public static Color GetUniqueColorFromId(int id)
		{
			var hue = (float)id / (float)16;
			return Color.HSVToRGB(hue, 1, 1);
		} 
		 
		public static Color GetRandomUniqueColor()
		{
			//random number picker
			var id = rnd.Next(0, 20);
			return	GetUniqueColorFromId(id);
		} 
		
		public static Color StringToColor(string color)
		{
			return ColorUtility.TryParseHtmlString(color, out var systemColor) ? systemColor : Color.white;
		}
	}
}