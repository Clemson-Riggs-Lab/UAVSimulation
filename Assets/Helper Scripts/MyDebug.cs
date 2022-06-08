using System;
using Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Helper_Scripts
{
	public static class MyDebug
	{
		/// <summary>
		/// This function is called on OnValidate to make sure that all serialized fields that need to be connected in the editor are actually connected
		/// This makes it easy to spot changes that break the code by removing a game object reference that is required by a script
		/// </summary>
		/// <param name="variable"></param> the serialized field that is required to be referenced in the editor
		/// <param name="go"></param> the game object on which the script resides. This is referenced so that clicking on the error in the console would point you to the object on which the script resides
		/// <typeparam name="T"></typeparam> any class
		public static void AssertComponentReferencedInEditor<T>(T variable, GameObject go) where T : class
		{
			if (variable == null)
			{
				var debugString =
					$"The instance of ({typeof(T).Name}) is `null` in the GameObject ({go.name}) in the scene" +
					$"{Environment.NewLine} Please reference a valid instance of ({typeof(T).Name}) in {go.name} then clear debugger Errors";
				Debug.LogError(debugString, go);
			}
		}

		public static void CheckIfReferenceExistsOrComponentExistsInGameObject<T1, T2>(T1 variable,T2 script, GameObject go) where T1 : class
		{
			if (variable != null) return;
			
			if(go.TryGetComponent<T1>(out variable)==false)//try to get the reference from the gameObject
			{
				var debugString =$" Reference to { typeof(T1).Name } component in {typeof(T2).Name} script is not initialized in editor and was not found on containing gameObject {go.name}" +
				                 $"{Environment.NewLine} Please reference a valid instance of ({typeof(T1).Name}) then clear debugger Errors";
				Debug.LogError(debugString,go);
			}
			else
			{
				var debugString =$" Reference to { typeof(T1).Name } component in {typeof(T2).Name} script is not initialized in editor but was found from the object {go.name}" +
				                 $"{Environment.NewLine} If this behavior is not intended, then please reference a valid instance of ({typeof(T1).Name}) then clear debugger Errors";
				Debug.LogWarning(debugString,go);
			}
		}

	}
}