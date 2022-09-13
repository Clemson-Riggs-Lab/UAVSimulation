using ScriptableObjects.EventChannels;
using ScriptableObjects.InputFiles;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class SceneLoader : MonoBehaviour
    {
        public ConfigFilesDatabaseSO configFilesDatabaseSO;
        public ConsoleMessageEventChannelSO WriteMessageToConsoleChannel;
        public void LoadScene(string sceneName)
        {
            
            if (configFilesDatabaseSO.inputFileFullFilePath=="")
            {
                WriteMessageToConsoleChannel.RaiseEvent("", new(){color = "red",doAnimate = true,text="\n Please select an input file first"});
                return;
            }
            else if (configFilesDatabaseSO.settingsFileFullFilePath=="")
            {
                WriteMessageToConsoleChannel.RaiseEvent("", new(){color = "red",doAnimate = true,text="\n Please select a settings file first"});
                return;
            }
            else 
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
