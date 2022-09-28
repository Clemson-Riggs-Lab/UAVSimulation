using System;
using UnityEngine;

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
	}
}