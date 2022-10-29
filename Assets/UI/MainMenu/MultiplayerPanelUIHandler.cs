using IOHandlers;
using IOHandlers.Settings.ScriptableObjects;
using Modules.Prompts;
using Multiplayer;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UI.Console.Channels.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static IOHandlers.ConfigFilesHandler;

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

        [Space(10)]
        [SerializeField] ConfigFilesHandler _configFilesHandler;

        private void Start()
        {
            _multiplayerSettingsGo.SetActive(false);

            MainMenuNetworkCallsHandler.Instance.BothFilesCompletelySent_NetworkEventHandler += OnBothFilesCompletelySentNetworkEventHandler;
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
                }
                if (configFilesSettingsSO.settingsFileFullFilePath == "")
                {
                    writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Please select a settings file first" });
                }

                _multiplayerSettingsGo.SetActive(true);
                _ipAddIF.text = AppNetPortal.Instance.GetLocalIPAddress();
                _portIF.text = AppNetPortal.Instance.GetDefaultPortNo().ToString();

                HandleBtns(PanelState.FirstTimeShow);
            }
            else
            {
                OnClickStopBtn();
                _multiplayerSettingsGo.SetActive(false);
            }
        }

        private void OnClickStartHostBtn()
        {
            if (_configFilesHandler.GetFilesInfoFromWorkDir(FilesType.Input).Count != 0 && _configFilesHandler.GetFilesInfoFromWorkDir(FilesType.Settings).Count != 0 && _ipAddIF.text != "" && _portIF.text != "")
            {
                if (AppNetPortal.Instance.StartHost(_ipAddIF.text, Int32.Parse(_portIF.text)) == 1)
                {
                    HandleBtns(PanelState.Waiting);
                }
            }
            else if (configFilesSettingsSO.inputFileFullFilePath == "")
            {
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Please select an input file first" });
            }
            else if (configFilesSettingsSO.settingsFileFullFilePath == "")
            {
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Please select a settings file first" });
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
                {
                    List<FileInfo> inputFileInfoLs = _configFilesHandler.GetFilesInfoFromWorkDir(ConfigFilesHandler.FilesType.Input);
                    string inputJsonStr = File.ReadAllText(inputFileInfoLs[0].FullName);

                    MainMenuNetworkCallsHandler.Instance.SendInputFile(inputJsonStr);

                    List<FileInfo> settingsFileInfoLs = _configFilesHandler.GetFilesInfoFromWorkDir(ConfigFilesHandler.FilesType.Settings);
                    string settingsJsonStr = File.ReadAllText(settingsFileInfoLs[0].FullName);

                    MainMenuNetworkCallsHandler.Instance.SendSettingsFile(settingsJsonStr);
                }
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

        private void OnBothFilesCompletelySentNetworkEventHandler(object sender, EventArgs e)
        {
            writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Input and Settings Files Downloaded From Host" });
            HandleBtns(PanelState.Ready);
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
