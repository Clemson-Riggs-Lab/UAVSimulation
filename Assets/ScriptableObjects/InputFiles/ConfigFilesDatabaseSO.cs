using UnityEngine;

namespace ScriptableObjects.InputFiles
{
    [CreateAssetMenu(fileName = "ConfigFilesDatabase", menuName = "Database/ConfigFilesDatabase")]
    public class ConfigFilesDatabaseSO : ScriptableObject
    {
        public string inputFileFullFilePath = @"";
        public string settingsFileFullFilePath = @"";
    }
}

