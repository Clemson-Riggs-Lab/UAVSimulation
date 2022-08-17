using UnityEngine;

public class PrefabsManager: MonoBehaviour
{
	public static PrefabsManager Instance { get; private set; }

	[Space(20)]
	public GameObject uavPrefab;
	public GameObject waypointPrefab;
	public GameObject fuelAndHealthPanelPrefab;
	public GameObject chatBoxPanelPrefab;
	public GameObject responseButtonPrefab;
	private void Awake() 
	{ 
		// If there is an instance, and it's not me, delete myself.
    
		if (Instance != null && Instance != this) 
		{ 
			Destroy(this); 
		} 
		else 
		{ 
			Instance = this; 
		} 
	}
		
}