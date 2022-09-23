using IOHandlers.Settings.ScriptableObjects;
using UI.Console.Channels.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class SceneLoader : MonoBehaviour
    {
        public ConfigFilesSettingsSO configFilesSettingsSO;
        public ConsoleMessageEventChannelSO writeMessageToConsoleChannel;
        public void LoadScene(string sceneName)
        {
            
            if (configFilesSettingsSO.inputFileFullFilePath=="")
            {
                writeMessageToConsoleChannel.RaiseEvent("", new(){color = "red",doAnimate = true,text="\n Please select an input file first"});
                return;
            }
            else if (configFilesSettingsSO.settingsFileFullFilePath=="")
            {
                writeMessageToConsoleChannel.RaiseEvent("", new(){color = "red",doAnimate = true,text="\n Please select a settings file first"});
                return;
            }
            else 
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
