using IOHandlers.Settings.ScriptableObjects;
using Modules.Prompts;
using Multiplayer;
using System;
using TMPro;
using UI.Console.Channels.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public enum PanelState { FirstTimeShow, Waiting, Stopped, Ready }

    public class MultiplayerPanelUIHandler : MonoBehaviour
    {
        [SerializeField] Button _multiplayerBtn;
        [SerializeField] GameObject _multiplayerSettingsGo;

        [Space(10)]
        [SerializeField] TMP_InputField _ipAddIF;
        [SerializeField] TMP_InputField _portIF;
        [SerializeField] Button _startHostBtn;
        [SerializeField] Button _startClientBtn;
        [SerializeField] Button _stopBtn;
        [SerializeField] Button _startSimulationBtn;

        [Space(10)]
        [SerializeField] ConsoleMessageEventChannelSO writeMessageToConsoleChannel;
        [SerializeField] ConfigFilesSettingsSO configFilesSettingsSO;

        private void Start()
        {
            _multiplayerSettingsGo.SetActive(false);
        }

        private void OnEnable()
        {
            _multiplayerBtn.onClick.AddListener(OnClickMultiplayerBtn);
            _startHostBtn.onClick.AddListener(OnClickStartHostBtn);
            _startClientBtn.onClick.AddListener(OnClickStartClientBtn);
            _stopBtn.onClick.AddListener(OnClickStopBtn);
            _startSimulationBtn.onClick.AddListener(OnClickStartSimulationBtn);

            AppNetPortal.Instance.ClientConnected_EventHandler += OnClientConnected;
            AppNetPortal.Instance.ClientDisconnected_EventHandler += OnClientDisconnected;
        }

        private void OnDisable()
        {
            _multiplayerBtn.onClick.RemoveListener(OnClickMultiplayerBtn);
            _startHostBtn.onClick.RemoveListener(OnClickStartHostBtn);
            _startClientBtn.onClick.RemoveListener(OnClickStartClientBtn);
            _stopBtn.onClick.RemoveListener(OnClickStopBtn);
            _startSimulationBtn.onClick.RemoveListener(OnClickStartSimulationBtn);

            AppNetPortal.Instance.ClientConnected_EventHandler -= OnClientConnected;
            AppNetPortal.Instance.ClientDisconnected_EventHandler -= OnClientDisconnected;
        }

        private void OnClickMultiplayerBtn()
        {
            if (_multiplayerSettingsGo.activeSelf == false)
            {
                if (configFilesSettingsSO.inputFileFullFilePath == "")
                {
                    writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Please select an input file first" });
                    return;
                }
                else if (configFilesSettingsSO.settingsFileFullFilePath == "")
                {
                    writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Please select a settings file first" });
                    return;
                }
                else
                {
                    _multiplayerSettingsGo.SetActive(true);
                    _ipAddIF.text = AppNetPortal.Instance.GetLocalIPAddress();
                    _portIF.text = AppNetPortal.Instance.GetDefaultPortNo().ToString();

                    HandleBtns(PanelState.FirstTimeShow);
                }
            }
            else
            {
                OnClickStopBtn();
                _multiplayerSettingsGo.SetActive(false);
            }
        }

        private void OnClickStartHostBtn()
        {
            if (_ipAddIF.text != "" && _portIF.text != "")
            {
                if (AppNetPortal.Instance.StartHost(_ipAddIF.text, Int32.Parse(_portIF.text)) == 1)
                {
                    HandleBtns(PanelState.Waiting);
                }
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
            if (_ipAddIF.text != "" && _portIF.text != "")
            {
                if (AppNetPortal.Instance.StartClient(_ipAddIF.text, Int32.Parse(_portIF.text)) == 1)
                {
                    HandleBtns(PanelState.Waiting);
                }
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

        private void OnClickStopBtn()
        {
            if (AppNetPortal.Instance.StopClient() == 1)
            {
                HandleBtns(PanelState.Stopped);
            }
        }

        private void OnClickStartSimulationBtn()
        {
            MainMenuNetworkCallsHandler.Instance.LoadSimulationServerRpc();
        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            if (AppNetPortal.Instance.IsServer)
            {
                if (AppNetPortal.Instance.ConnectedClientCount == 2)
                    HandleBtns(PanelState.Ready);
            }
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            if (AppNetPortal.Instance.IsServer)
            {
                if (AppNetPortal.Instance.LocalClientId == e.ClientId)
                    HandleBtns(PanelState.Stopped);
                else
                    HandleBtns(PanelState.Waiting);
            }
            else
                HandleBtns(PanelState.Stopped);
        }

        private void HandleBtns(PanelState panelState)
        {
            switch(panelState)
            {
                case PanelState.FirstTimeShow:
                    _startHostBtn.interactable = true;
                    _startClientBtn.interactable = true;
                    _stopBtn.interactable = false;
                    _startSimulationBtn.interactable = false;
                    break;
                case PanelState.Waiting:
                    _startHostBtn.interactable = false;
                    _startClientBtn.interactable = false;
                    _stopBtn.interactable = true;
                    _startSimulationBtn.interactable = false;
                    break;
                case PanelState.Stopped:
                    _startHostBtn.interactable = true;
                    _startClientBtn.interactable = true;
                    _stopBtn.interactable = false;
                    _startSimulationBtn.interactable = false;
                    break;
                case PanelState.Ready:
                    _startHostBtn.interactable = false;
                    _startClientBtn.interactable = false;
                    _stopBtn.interactable = true;
                    _startSimulationBtn.interactable = true;
                    break;
            }
        }
    }
}
