using UnityEngine;

namespace HelperScripts
{
	public static class Extensions
	{
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			return gameObject.TryGetComponent<T>(out var t) ? t : gameObject.AddComponent<T>();
		}
	}
}