using Chat.ScriptableObjects;
using UAVs.Camera.ScriptableObjects;
using UAVs.Fuel.ScriptableObjects;
using UAVs.Navigation.ScriptableObjects;
using UnityEngine;

public class SettingsManager:MonoBehaviour
{
	public static SettingsManager Instance { get; private set; }
	
	public UavCameraSettingsSO uavCameraSettings;
	public FuelAndHealthSettingsSO fuelAndHealthSettings;
	public NavigationSettingsSO navigationSettings;
	public ChatSettingsSO chatSettings;
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