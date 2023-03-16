using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace HelperScripts
{
	public static class Extensions
	{
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			return gameObject.TryGetComponent<T>(out var t) ? t : gameObject.AddComponent<T>();
		}
		
		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source?.IndexOf(toCheck, comp) >= 0;
		}
		
		public static void Shuffle<T>(this IList<T> list, Random shuffleRandomGenerator)  
		{  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = shuffleRandomGenerator.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}
		
	}
}