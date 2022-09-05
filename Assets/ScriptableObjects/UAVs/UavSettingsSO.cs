using ScriptableObjects.UAVs.Camera;
using ScriptableObjects.UAVs.FuelAndHealth;
using ScriptableObjects.UAVs.Navigation;
using UnityEngine;

namespace ScriptableObjects.UAVs
{
	[CreateAssetMenu(fileName = "UavSettings", menuName = "Settings/Uav Settings")]
	public class UavSettingsSO:ScriptableObject
	{
		public UavCameraSettingsSO uavCameraSettings;
		public NavigationSettingsSO navigationSettings;
		public FuelAndHealthSettingsSO fuelAndHealthSettings;
		
	}
}