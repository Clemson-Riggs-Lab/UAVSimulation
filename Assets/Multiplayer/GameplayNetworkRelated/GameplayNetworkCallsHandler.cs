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
using static Multiplayer.GameplayNetworkCallsData;

namespace Multiplayer
{
    public class GameplayNetworkCallsHandler : NetworkBehaviour
    {
        public static GameplayNetworkCallsHandler Instance { get; private set; }

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
        public void ReroutingUAVOnServerRpc(ReroutingDataStruct reroutingDataStruct)
        {
            ReroutingUAVOnClientRpc(reroutingDataStruct);
        }

        [ClientRpc]
        public void ReroutingUAVOnClientRpc(ReroutingDataStruct reroutingDataStruct)
        {
            Debug.Log("UAV ID: " + reroutingDataStruct.UAVId + " , " + "Option No: " + reroutingDataStruct.OptionNo);
        }
    }
}


