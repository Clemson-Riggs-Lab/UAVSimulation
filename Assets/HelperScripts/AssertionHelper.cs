using System;
using UAVs.Navigation;
using UnityEngine;

namespace HelperScripts
{
	public static class AssertionHelper
	{
		/// <summary>
		/// This function is called on OnValidate to make sure that all serialized fields that need to be connected in the editor are actually connected
		/// This makes it easy to spot changes that break the code by removing a game object reference that is required by a script
		/// </summary>
		/// <param name="field"></param>
		/// <param name="containingScript"></param> the serialized field that is required to be referenced in the editor
		/// <param name="go"></param> the game object on which the script resides. This is referenced so that clicking on the error in the console would point you to the object on which the script resides
		/// <typeparam name="T1"></typeparam> a class reference that is required to be referenced in the editor
		/// <typeparam name="T2"></typeparam> the calling script
		public static void AssertComponentReferencedInEditor<T1,T2>(T1 field, T2 containingScript, GameObject go) where T2 : class
		{
			if (field == null)
			{
				var debugString =
					$"The reference to ({typeof(T1).Name}) in script {typeof(T2).Name} is `null` in the GameObject ({go.name}) in the scene" +
					$"{Environment.NewLine} Please add a valid reference to a ({typeof(T1).Name}) in {go.name} then clear debugger Errors";
				Debug.LogError(debugString, go);
			}
		}

		/// <summary>
		/// This function is called on objects after trying to get their reference from the Game Manager script to make sure that the object is referenced in the game manager 
		/// </summary>
		/// <param name="field"></param>
		/// <param name="containingScript"></param>
		/// <param name="go"></param>
		/// <typeparam name="T1"></typeparam> 
		/// <typeparam name="T2"></typeparam> the calling script
		public static void AssertObjectReferenceObtainedFromGameManager<T1,T2>(T1 field,T2 containingScript, GameObject go) where T2 : class
		{
			if (field == null)
			{
				var debugString =
					$"The reference to ({typeof(T1).Name}) in script {typeof(T2).Name} is `null` in the GameObject ({go.name}) since it couldn't find it in the GameManager script" +
					$"{Environment.NewLine} Please add a valid reference to a ({typeof(T1).Name}) in the Game Manager script then clear debugger Errors";
				Debug.LogError(debugString, GameManager.Instance.gameObject);
			}
		}
		
		/// <summary>
		/// This function is called on a prefab reference after trying to get it from the Prefabs Manager script to make sure that the object is referenced in the prefabs manager 
		/// </summary>
		/// <param name="field"></param>
		/// <param name="name"></param>
		/// <param name="containingScript"></param>
		/// <param name="go"></param>
		/// <typeparam name="T1"></typeparam> 
		/// <typeparam name="T2"></typeparam> the calling script
		public static void AssertPrefabReferenceObtainedFromPrefabsManager<T1,T2>(T1 field,string name,T2 containingScript, GameObject go) where T2 : class
		{
			if (field == null)
			{
				var debugString =
					$"The reference to ({name}) prefab in script {typeof(T2).Name} is `null` in the GameObject ({go.name}) since it couldn't find it in the PrefabManager script" +
					$"{Environment.NewLine} Please add a valid reference to a ({typeof(T1).Name}) in the Prefab Manager script then clear debugger Errors";
				Debug.LogError(debugString, GameManager.Instance.prefabsDatabase);
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
		
		
		public static void AssertAssetReferenced<T1,T2>(T1 field, T2 containingScript) where T2 : class
		{
			if (field != null) return;
			//else
			var debugString =
				$"The reference to ({typeof(T1).Name}) in asset {typeof(T2).Name} is `null` ";
				Debug.LogError(debugString);
		}
	}
}