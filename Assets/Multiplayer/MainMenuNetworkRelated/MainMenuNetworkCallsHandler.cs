using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public event EventHandler InputFileCompletelySent_NetworkEventHandler;

        [SerializeField] NetworkTransmitter _networkTransmitter;

        private string _inputJsonString = "";

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

        public void SendInputFile(string jsonStr)
        {
            StartCoroutine(SendDataInChunks(jsonStr));
        }

        private IEnumerator SendDataInChunks(string jsonStr)
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
                _networkTransmitter.SendBytesToClientsOnServerRpc(1, chunk);

                yield return new WaitForSeconds(0.1f);
            }

            InputFileCompletelySentOnServerRpc();

            yield return null;
        }

        private void OnDataChunkCompletelySent(int arg0, byte[] arg1)
        {
            Debug.Log("Data Completely Sent");
        }

        private void OnDataChunkCompletelyReceived(int arg0, byte[] arg1)
        {
            if (IsHost == false)
            {
                string jsonStr = Encoding.UTF8.GetString(arg1);
                _inputJsonString = _inputJsonString + jsonStr;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void InputFileCompletelySentOnServerRpc()
        {
            InputFileCompletelySentOnClientRpc();
        }

        [ClientRpc]
        private void InputFileCompletelySentOnClientRpc()
        {
            if (IsHost)
                InputFileCompletelySent_NetworkEventHandler?.Invoke(this, new EventArgs());

            if (IsHost == false)
            {
                Debug.Log("Input File Full Received");
                Debug.Log(_inputJsonString);
                NewInputFileReceived_NetworkEventHandler?.Invoke(this, _inputJsonString);
            }
        }
    }
}


