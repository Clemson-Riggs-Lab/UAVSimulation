using HelperScripts;
using ScriptableObjects.UAVs.Camera;
using ScriptableObjects.UAVs.FuelAndHealth;
using ScriptableObjects.UAVs.Navigation;
using ScriptableObjects.UAVs.Navigation.PathsRendering;
using UnityEngine;

namespace ScriptableObjects.UAVs
{
	[CreateAssetMenu(fileName = "UavSettingsDatabase", menuName = "Settings/Uav Settings Database")]
	public class UavSettingsDatabaseSO:ScriptableObject
	{
		public UavGeneralSettingsSO uavGeneralSettings;
		public UavCameraPanelSettingsSO uavCameraPanelSettings;
		public NavigationSettingsSO navigationSettings;
		public FuelAndHealthSettingsSO fuelAndHealthSettings;
		public PathsRenderingSettingsSO pathsRenderingSettings;
		
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(uavCameraPanelSettings,this);
			AssertionHelper.AssertAssetReferenced(navigationSettings,this);
			AssertionHelper.AssertAssetReferenced(fuelAndHealthSettings,this);
			AssertionHelper.AssertAssetReferenced(pathsRenderingSettings,this);
			
			AssertionHelper.AssertAssetReferenced(uavGeneralSettings,this);
		}
		
		public enum UavNamingScheme
		{
			HashtagNumber,
			UavAndNumber,
			HashtagNumberOffsetZero,
			UavAndNumberOffsetZero,
			Letter,
			NatoName
			
		}
	}
	
}