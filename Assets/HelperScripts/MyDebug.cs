using System;
using UnityEngine;

namespace HelperScripts
{
	public static class MyDebug
	{
		/// <summary>
		/// This function is called on OnValidate to make sure that all serialized fields that need to be connected in the editor are actually connected
		/// This makes it easy to spot changes that break the code by removing a game object reference that is required by a script
		/// </summary>
		/// <param name="uavsContainer"></param>
		/// <param name="variable"></param>
		/// <param name="go"></param>
		/// the serialized field that is required to be referenced in the editor
		/// the game object on which the script resides. This is referenced so that clicking on the error in the console would point you to the object on which the script resides
		/// <typeparam name="T1"></typeparam> a class reference that is required to be referenced in the editor
		/// <typeparam name="T2"></typeparam> the calling script
		public static void AssertComponentReferencedInEditor<T1,T2>(T1 uavsContainer, T2 variable, GameObject go) where T1 : class
		{
			if (variable == null)
			{
				var debugString =
					$"The reference to ({typeof(T1).Name}) in script {typeof(T2).Name} is `null` in the GameObject ({go.name}) in the scene" +
					$"{Environment.NewLine} Please add a valid reference to a ({typeof(T1).Name}) in {go.name} then clear debugger Errors";
				Debug.LogError(debugString, go);
			}
		}

		/// <summary>
		/// Similar to above but also trys to find the component in the containing game object,
		/// if found it will return the component, if not it will return an error
		/// </summary>
		/// <param name="variable"></param>
		/// <param name="script"></param>
		/// <param name="go"></param>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
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
				                 $"{Environment.NewLine} If this behavior is not intended, please remove the added reference then reference a valid instance of ({typeof(T1).Name}) then clear debugger Errors";
				Debug.LogWarning(debugString,go);
			}
		}

	}
}