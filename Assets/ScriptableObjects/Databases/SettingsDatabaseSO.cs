using Prompts.ScriptableObjects;
using ScriptableObjects.TargetDetection;
using ScriptableObjects.UAVs;
using UnityEngine;

namespace ScriptableObjects.Databases
{
	[CreateAssetMenu(fileName = "SettingsDatabaseSO", menuName = "Database/SettingsDatabaseSO")]

	public class SettingsDatabaseSO:ScriptableObject
	{

		public PromptSettingsSO promptSettings;
		public TargetDetectionSettingsSO targetDetectionSettings;
		public UavSettingsSO uavSettings;
	
	}
}