using HelperScripts;
using ScriptableObjects.UAVs.Camera;
using ScriptableObjects.UAVs.FuelAndHealth;
using ScriptableObjects.UAVs.Navigation;
using ScriptableObjects.UAVs.Navigation.PathsRendering;
using UnityEngine;

namespace ScriptableObjects.UAVs
{
	[CreateAssetMenu(fileName = "UavSettings", menuName = "Settings/Uav Settings")]
	public class UavSettingsSO:ScriptableObject
	{
		public UavNamingScheme namingScheme=UavNamingScheme.HashtagNumberOffsetZero;
		
		public UavCameraSettingsSO uavCameraSettings;
		public NavigationSettingsSO navigationSettings;
		public FuelAndHealthSettingsSO fuelAndHealthSettings;
		public PathsRenderingSettingsSO pathsRenderingSettings;
		
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(uavCameraSettings,this);
			AssertionHelper.AssertAssetReferenced(navigationSettings,this);
			AssertionHelper.AssertAssetReferenced(fuelAndHealthSettings,this);
			AssertionHelper.AssertAssetReferenced(pathsRenderingSettings,this);
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