using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IOHandlers;
using ScriptableObjects.EventChannels;
using SyedAli.Main;
using UI.Console.Channels.ScriptableObjects;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using WayPoints.Channels.ScriptableObjects;
using static Multiplayer.MainMenuNetworkCallsData;

namespace Multiplayer
{
    public class MainMenuNetworkCallsHandler : NetworkBehaviour
    {
        public event EventHandler<string> NewInputFileReceived_NetworkEventHandler;
        public event EventHandler<string> NewSettingsFileReceived_NetworkEventHandler;
        public event EventHandler InputFileCompletelySent_NetworkEventHandler;
        public event EventHandler SettingFileCompletelySent_NetworkEventHandler;

        [SerializeField] NetworkTransmitter _networkTransmitter;

        [SerializeField] ConsoleMessageEventChannelSO writeMessageToConsoleChannel;

        private string _inputJsonString = "";
        private string _settingsJsonString = "";

        private float _delayTime = 0.5f;

        public static MainMenuNetworkCallsHandler Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _networkTransmitter.OnDataComepletelySent += OnDataChunkCompletelySent;
            _networkTransmitter.OnDataCompletelyReceived += OnDataChunkCompletelyReceived;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void LoadSimulationServerRpc()
        {
            LoadSimulationClientRpc();
        }

        [ClientRpc]
        private void LoadSimulationClientRpc()
        {
            if (IsServer)
                NetworkManager.SceneManager.LoadScene("SimulationScene", LoadSceneMode.Single);
        }

        #region Input File Related
        public void SendInputFile(string jsonStr)
        {
            StartCoroutine(SendInputDataInChunks(jsonStr));
        }

        private IEnumerator SendInputDataInChunks(string jsonStr)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);

            int leftOverLen = bytes.Length;
            int chunkSize = 10000;

            for (int i = 0; i < bytes.Length;)
            {
                int len;
                if (leftOverLen > chunkSize)
                {
                    len = chunkSize;
                    leftOverLen = leftOverLen - chunkSize;
                }
                else
                    len = leftOverLen;

                int j = 0;
                byte[] chunk = new byte[len];
                while (j < chunk.Length && i < bytes.Length)
                {
                    chunk[j] = bytes[i++];
                    j++;
                }
                _networkTransmitter.SendBytesToClientsOnServerRpc(1, chunk); // 1 is for Input File

                yield return new WaitForSeconds(_delayTime);
            }

            InputFileCompletelySentOnServerRpc();

            yield return null;
        }

        [ServerRpc(RequireOwnership = false)]
        public void InputFileCompletelySentOnServerRpc()
        {
            InputFileCompletelySentOnClientRpc();
        }

        [ClientRpc]
        private void InputFileCompletelySentOnClientRpc()
        {
            if (IsHost == false)
            {
                Debug.Log("Input File Full Received");
                Debug.Log(_inputJsonString);
                NewInputFileReceived_NetworkEventHandler?.Invoke(this, _inputJsonString);
            }

            if (IsHost)
                InputFileCompletelySent_NetworkEventHandler?.Invoke(this, new EventArgs());
        }
        #endregion

        #region Settings File Related
        public void SendSettingsFile(string jsonStr)
        {
            StartCoroutine(SendSettingsDataInChunks(jsonStr));
        }

        private IEnumerator SendSettingsDataInChunks(string jsonStr)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);

            int leftOverLen = bytes.Length;
            int chunkSize = 10000;

            for (int i = 0; i < bytes.Length;)
            {
                int len;
                if (leftOverLen > chunkSize)
                {
                    len = chunkSize;
                    leftOverLen = leftOverLen - chunkSize;
                }
                else
                    len = leftOverLen;

                int j = 0;
                byte[] chunk = new byte[len];
                while (j < chunk.Length && i < bytes.Length)
                {
                    chunk[j] = bytes[i++];
                    j++;
                }
                _networkTransmitter.SendBytesToClientsOnServerRpc(2, chunk); // 2 -> TranmissionId for Settings File

                yield return new WaitForSeconds(_delayTime);
            }

            SettingsFileCompletelySentOnServerRpc();

            yield return null;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SettingsFileCompletelySentOnServerRpc()
        {
            SettingsFileCompletelySentOnClientRpc();
        }

        [ClientRpc]
        private void SettingsFileCompletelySentOnClientRpc()
        {
            if (IsHost == false)
            {
                Debug.Log("Settings File Full Received");
                Debug.Log(_settingsJsonString);
                NewSettingsFileReceived_NetworkEventHandler?.Invoke(this, _settingsJsonString);

                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Input and Settings Files Downloaded From Host" });
            }

            if (IsHost)
                SettingFileCompletelySent_NetworkEventHandler?.Invoke(this, new EventArgs());
        }
        #endregion

        private void OnDataChunkCompletelySent(int transmissionId, byte[] data)
        {
            Debug.Log("Data Completely Sent For Tranmission Id: " + transmissionId);
        }

        private void OnDataChunkCompletelyReceived(int transmissionId, byte[] data)
        {
            if (IsHost == false)
            {
                if (transmissionId == 1) // 1 -> Transmission Id for Input File
                {
                    string jsonStr = Encoding.UTF8.GetString(data);
                    _inputJsonString = _inputJsonString + jsonStr;
                }

                if (transmissionId == 2) // 2 -> Transmission Id for Settings File
                {
                    string jsonStr = Encoding.UTF8.GetString(data);
                    _settingsJsonString = _settingsJsonString + jsonStr;
                }
            }
        }
    }
}


