using UAVs.Camera.ScriptableObjects;
using UAVs.Navigation.ScriptableObjects;
using UAVs.Sub_Modules.Fuel.ScriptableObjects;
using UnityEngine;

namespace Chat.ScriptableObjects
{
	[CreateAssetMenu(fileName = "UavSettings", menuName = "Settings/Uav Settings")]
	public class UavSettingsSO:ScriptableObject
	{
		public UavCameraSettingsSO uavCameraSettings;
		public NavigationSettingsSO navigationSettings;
		public FuelAndHealthSettingsSO fuelAndHealthSettings;
		
	}
}