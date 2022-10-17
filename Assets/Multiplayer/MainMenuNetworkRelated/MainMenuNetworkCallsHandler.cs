using System;
using System.Collections.Generic;
using ScriptableObjects.EventChannels;
using SyedAli.Main;
using UI.Console.Channels.ScriptableObjects;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using WayPoints.Channels.ScriptableObjects;

namespace Multiplayer
{
    public class MainMenuNetworkCallsHandler : NetworkBehaviour
    {
        public static MainMenuNetworkCallsHandler Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
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
        public void LoadSimulationClientRpc()
        {
            SceneManager.LoadScene("SimulationScene");
        }
    }
}


