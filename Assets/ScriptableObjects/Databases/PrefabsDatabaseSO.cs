using HelperScripts;
using UnityEngine;

namespace ScriptableObjects.Databases
{
	[CreateAssetMenu(fileName = "PrefabsDatabaseSO", menuName = "Database/PrefabsDatabaseSO")]
	public class PrefabsDatabaseSO: ScriptableObject
	{
		[Space(20)]
		public GameObject uavPrefab;
		public GameObject waypointPrefab;
		public GameObject targetBoxPrefab;
		public GameObject fuelAndHealthPanelPrefab;
		public GameObject uavCameraPrefab;
		public GameObject chatBoxPanelPrefab;
		public GameObject responseButtonPrefab;
		public GameObject rerouteButtonPrefab;
		public GameObject reroutingOptionsPanelsPrefab;
		
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(waypointPrefab,this);
			AssertionHelper.AssertAssetReferenced(targetBoxPrefab,this);
			AssertionHelper.AssertAssetReferenced(fuelAndHealthPanelPrefab,this);
			AssertionHelper.AssertAssetReferenced(uavCameraPrefab,this);
			AssertionHelper.AssertAssetReferenced(chatBoxPanelPrefab,this);
			AssertionHelper.AssertAssetReferenced(responseButtonPrefab,this);
			AssertionHelper.AssertAssetReferenced(rerouteButtonPrefab,this);
			AssertionHelper.AssertAssetReferenced(reroutingOptionsPanelsPrefab,this);
		}
	}
}