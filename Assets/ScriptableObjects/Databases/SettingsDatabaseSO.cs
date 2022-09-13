using HelperScripts;
using Prompts.ScriptableObjects;
using ScriptableObjects.NoFlyZone;
using ScriptableObjects.TargetDetection;
using ScriptableObjects.UAVs;
using ScriptableObjects.UAVs.Navigation;
using ScriptableObjects.UAVs.Navigation.Rerouting;
using ScriptableObjects.Waypoints;
using UnityEngine;

namespace ScriptableObjects.Databases
{
	[CreateAssetMenu(fileName = "SettingsDatabase", menuName = "Database/SettingsDatabase")]

	public class SettingsDatabaseSO:ScriptableObject
	{
		
		public WaypointSettingsSO waypointSettings;
		public PromptSettingsSO promptSettings;
		public TargetDetectionSettingsSO targetDetectionSettings;
		public UavSettingsDatabaseSO uavSettingsDatabase;
		public ReroutingSettingsSO reroutingSettings;
		public NFZSettingsSO nfzSettings;
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(waypointSettings,this);
			AssertionHelper.AssertAssetReferenced(promptSettings,this);
			AssertionHelper.AssertAssetReferenced(targetDetectionSettings,this);
			AssertionHelper.AssertAssetReferenced(uavSettingsDatabase,this);
			AssertionHelper.AssertAssetReferenced(reroutingSettings,this);
			AssertionHelper.AssertAssetReferenced(nfzSettings,this);
		}
	}
}