using HelperScripts;
using Prompts.ScriptableObjects;
using ScriptableObjects.NoFlyZone;
using ScriptableObjects.TargetDetection;
using ScriptableObjects.UAVs;
using ScriptableObjects.UAVs.Navigation;
using ScriptableObjects.UAVs.Navigation.Rerouting;
using UnityEngine;

namespace ScriptableObjects.Databases
{
	[CreateAssetMenu(fileName = "SettingsDatabase", menuName = "Database/SettingsDatabase")]

	public class SettingsDatabaseSO:ScriptableObject
	{

		public PromptSettingsSO promptSettings;
		public TargetDetectionSettingsSO targetDetectionSettings;
		public UavSettingsSO uavSettings;
		public ReroutingSettingsSO reroutingSettings;
		public NFZSettingsSO nfzSettings;
		private void OnEnable()
		{
			AssertAllReferencesAreNotNull();
		}

		private void AssertAllReferencesAreNotNull()
		{
			AssertionHelper.AssertAssetReferenced(promptSettings,this);
			AssertionHelper.AssertAssetReferenced(targetDetectionSettings,this);
			AssertionHelper.AssertAssetReferenced(uavSettings,this);
			AssertionHelper.AssertAssetReferenced(reroutingSettings,this);
			AssertionHelper.AssertAssetReferenced(nfzSettings,this);
		}
	}
}