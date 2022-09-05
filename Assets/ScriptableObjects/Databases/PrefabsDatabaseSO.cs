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
	}
}