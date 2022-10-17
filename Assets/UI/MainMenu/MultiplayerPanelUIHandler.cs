using IOHandlers.Settings.ScriptableObjects;
using Modules.Prompts;
using System;
using TMPro;
using UI.Console.Channels.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MultiplayerPanelUIHandler : MonoBehaviour
    {
        [SerializeField] Button _multiplayerBtn;
        [SerializeField] GameObject _multiplayerSettingsGo;

        [Space(10)]
        [SerializeField] TMP_InputField _ipAddIF;
        [SerializeField] TMP_InputField _portIF;
        [SerializeField] Button _startHostBtn;
        [SerializeField] Button _startClientBtn;

        [Space(10)]
        [SerializeField] ConsoleMessageEventChannelSO writeMessageToConsoleChannel;

        private void Start()
        {
            _multiplayerSettingsGo.SetActive(false);
        }

        private void OnEnable()
        {
            _multiplayerBtn.onClick.AddListener(OnClickMultiplayerBtn);
            _startHostBtn.onClick.AddListener(OnClickStartHostBtn);
            _startClientBtn.onClick.AddListener(OnClickStartClientBtn);
        }

        private void OnDisable()
        {
            _multiplayerBtn.onClick.RemoveListener(OnClickMultiplayerBtn);
            _startHostBtn.onClick.RemoveListener(OnClickStartHostBtn);
            _startClientBtn.onClick.RemoveListener(OnClickStartClientBtn);
        }

        private void OnClickMultiplayerBtn()
        {
            _multiplayerSettingsGo.SetActive(true);
        }

        private void OnClickStartHostBtn()
        {
            if (_ipAddIF.text != "" && _portIF.text != "")
            {

            }
            else if (_ipAddIF.text == "")
            {
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Please Input Ip Address" });
            }
            else if (_portIF.text == "")
            {
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Please Input Port Number" });
            }
        }

        private void OnClickStartClientBtn()
        {
            if (_ipAddIF.text != null && _portIF.text != null)
            {

            }
            else if (_ipAddIF.text == "")
            {
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Please Input Ip Address" });
            }
            else if (_portIF.text == "")
            {
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Please Input Port Number" });
            }
        }
    }
}
