using IOHandlers.Settings.ScriptableObjects;
using TMPro;
using UI.Console.Channels.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class SceneLoader : MonoBehaviour
    {
        public ConfigFilesSettingsSO configFilesSettingsSO;
        public ConsoleMessageEventChannelSO writeMessageToConsoleChannel;
        public TMP_InputField participantNumberText;
        public TMP_InputField trialNumberText;
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
            if (participantNumberText.text == "")
            {
                writeMessageToConsoleChannel.RaiseEvent("", new(){color = "red",doAnimate = true,text="\n Please enter a participant number first"});
                return;
            }
            else if (trialNumberText.text == "")
            {
                writeMessageToConsoleChannel.RaiseEvent("", new(){color = "red",doAnimate = true,text="\n Please enter a trial number first"});
                return;
            }
            else
            {
                configFilesSettingsSO.participantNumber = int.Parse(participantNumberText.text);
                configFilesSettingsSO.trialNumber = trialNumberText.text;
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
