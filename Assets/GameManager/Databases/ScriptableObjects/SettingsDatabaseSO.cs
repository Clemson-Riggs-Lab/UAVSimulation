using HelperScripts;
using Modules.FuelAndHealth.Settings.ScriptableObjects;
using Modules.Navigation.Settings.ScriptableObjects;
using Modules.Navigation.Submodules.Rerouting.Settings.ScriptableObjects;
using Modules.NoFlyZone.Settings.ScriptableObjects;
using Modules.Prompts.Settings.ScriptableObjects;
using Modules.TargetDetection.Settings.ScriptableObjects;
using UAVs.Settings.ScriptableObjects;
using UI.FuelAndHealthPanel.Settings.ScriptableObjects;
using UI.Paths_Rendering.Settings.ScriptableObjects;
using UI.ReroutingPanel.Settings.ScriptableObjects;
using UI.UavCameraAndTargetDetectionPanel.Settings.ScriptableObjects;
using UnityEngine;
using WayPoints.Settings.ScriptableObjects;

namespace Databases.ScriptableObjects
{
	[CreateAssetMenu(fileName = "SettingsDatabase", menuName = "Database/SettingsDatabase")]

	public class SettingsDatabaseSO:ScriptableObject
	{
		
		public WaypointSettingsSO waypointSettings;
		public PromptSettingsSO promptSettings;
		public TargetDetectionSettingsSO targetDetectionSettings;
		public UavSettingsSO uavSettings;
		public ReroutingSettingsSO reroutingSettings;
		public ReroutingPanelSettingsSO reroutingPanelSettings;
		public NFZSettingsSO nfzSettings;
		public UavCameraAndTargetDetectionPanelSettingsSO uavCameraAndTargetDetectionPanelSettings;
		public FuelAndHealthPanelSettingsSO fuelAndHealthPanelSettings;
		public NavigationSettingsSO navigationSettings;
		public FuelSettingsSO fuelSettings;
		public PathsRenderingSettingsSO pathsRenderingSettings;
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(waypointSettings,this);
			AssertionHelper.AssertAssetReferenced(promptSettings,this);
			AssertionHelper.AssertAssetReferenced(targetDetectionSettings,this);
			AssertionHelper.AssertAssetReferenced(uavSettings,this);
			AssertionHelper.AssertAssetReferenced(reroutingSettings,this);
			AssertionHelper.AssertAssetReferenced(reroutingPanelSettings,this);
			AssertionHelper.AssertAssetReferenced(nfzSettings,this);
			AssertionHelper.AssertAssetReferenced(uavCameraAndTargetDetectionPanelSettings,this);
			AssertionHelper.AssertAssetReferenced(fuelAndHealthPanelSettings,this);
			AssertionHelper.AssertAssetReferenced(navigationSettings,this);
			AssertionHelper.AssertAssetReferenced(fuelSettings,this);
			AssertionHelper.AssertAssetReferenced(pathsRenderingSettings,this);
		}
	}
}