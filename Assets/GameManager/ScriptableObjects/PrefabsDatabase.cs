using UnityEngine;

[CreateAssetMenu(fileName = "PrefabsDatabase", menuName = "Database/PrefabsDatabase")]
public class PrefabsDatabase: ScriptableObject
{
	[Space(20)]
	public GameObject uavPrefab;
	public GameObject waypointPrefab;
	public GameObject targetBoxPrefab;
	public GameObject fuelAndHealthPanelPrefab;
	public GameObject chatBoxPanelPrefab;
	public GameObject responseButtonPrefab;
}