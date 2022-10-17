using UnityEngine;

namespace IOHandlers.Settings.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ConfigFilesDatabase", menuName = "Database/ConfigFilesDatabase")]
    public class ConfigFilesSettingsSO : ScriptableObject
    {
        public string inputFileFullFilePath = @"";
        public string settingsFileFullFilePath = @"";
    }
}
