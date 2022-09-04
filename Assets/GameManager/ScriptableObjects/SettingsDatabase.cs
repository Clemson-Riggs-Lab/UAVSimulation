using Chat.ScriptableObjects;
using Prompts.ScriptableObjects;
using TargetDetection.ScriptableObjects;
using UAVs.Camera.ScriptableObjects;
using UAVs.Navigation.ScriptableObjects;
using UAVs.Sub_Modules.Fuel.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsDatabase", menuName = "Database/SettingsDatabase")]

public class SettingsDatabase:ScriptableObject
{

	public PromptSettingsSO promptSettings;
	public TargetDetectionSettingsSO targetDetectionSettings;
	public UavSettingsSO uavSettings;
	
}