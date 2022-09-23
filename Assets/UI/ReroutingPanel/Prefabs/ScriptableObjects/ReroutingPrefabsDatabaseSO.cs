using HelperScripts;
using UnityEngine;

namespace UI.ReroutingPanel.Prefabs.ScriptableObjects
{
	[CreateAssetMenu(fileName = "ReroutingPrefabsDatabase", menuName = "Database/Prefabs/Rerouting Prefabs Database")]
	public class ReroutingPrefabsDatabaseSO:ScriptableObject
	{
		[Space(20)]
		public GameObject rerouteButtonPrefab;
		public GameObject reroutingOptionsPanelPrefab;
		public GameObject reroutingOptionRowPrefab;
		
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(rerouteButtonPrefab, this);
			AssertionHelper.AssertAssetReferenced(reroutingOptionsPanelPrefab, this);
			AssertionHelper.AssertAssetReferenced(reroutingOptionRowPrefab, this);
		}
		
	}
}