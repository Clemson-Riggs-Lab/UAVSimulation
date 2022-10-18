using System;
using System.Collections.Generic;
using Modules.Navigation.Submodules.Rerouting;
using ScriptableObjects.EventChannels;
using SyedAli.Main;
using UAVs;
using UI.Console.Channels.ScriptableObjects;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using WayPoints.Channels.ScriptableObjects;
using static Multiplayer.GameplayNetworkCallsData;
using static UnityEditor.Progress;

namespace Multiplayer
{
    public class GameplayNetworkCallsHandler : NetworkBehaviour
    {
        public event EventHandler<int> ReroutePanelOpen_NetworkEventHandler;
        public event EventHandler<int> ReroutePanelClose_NetworkEventHandler;
        public event EventHandler<ReroutingUAVEventArgs> ReroutingUAV_NetworkEventHandler;

        public event EventHandler<FixLeakEventArgs> FixLeak_NetworkEventHandler;

        public event EventHandler<int> TargetDetectClicked_NetworkEventHandler;
        public event EventHandler<int> TargetNotDetectClicked_NetworkEventHandler;

        public event EventHandler<NetworkString> ChatResponseClicked_NetworkEventHandler;

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

        #region Rerouting Related
        [ServerRpc(RequireOwnership = false)]
        public void ReroutingUAVOnServerRpc(int uavId, int optionNo)
        {
            ReroutingUAVOnClientRpc(uavId, optionNo);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReroutePanelOpenServerRpc(ulong localClientId, int uavId)
        {
            ReroutePanelOpenClientRpc(localClientId, uavId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReroutePanelCloseServerRpc(ulong localClientId, int uavId)
        {
            ReroutePanelCloseClientRpc(localClientId, uavId);
        }

        [ClientRpc]
        private void ReroutingUAVOnClientRpc(int uavId, int optionIndex)
        {
            Debug.Log("UAV ID: " + uavId + " , " + "Option No: " + optionIndex);

            ReroutingUAV_NetworkEventHandler?.Invoke(this, new ReroutingUAVEventArgs(uavId, optionIndex));
        }

        [ClientRpc]
        private void ReroutePanelOpenClientRpc(ulong localClientId, int uavId)
        {
            Debug.Log("UAV ID: " + uavId);

            if (AppNetPortal.Instance.NetworkManager.LocalClientId != localClientId)
                ReroutePanelOpen_NetworkEventHandler?.Invoke(this, uavId);
        }

        [ClientRpc]
        private void ReroutePanelCloseClientRpc(ulong localClientId, int uavId)
        {
            Debug.Log("UAV ID: " + uavId);

            if (AppNetPortal.Instance.NetworkManager.LocalClientId != localClientId)
                ReroutePanelClose_NetworkEventHandler?.Invoke(this, uavId);
        }
        #endregion

        #region FixLeak Related
        [ServerRpc(RequireOwnership = false)]
        public void FixLeakServerRpc(int uavId, NetworkString buttonText)
        {
            FixLeakClientRpc(uavId, buttonText);
        }

        [ClientRpc]
        private void FixLeakClientRpc(int uavId, NetworkString buttonText)
        {
            FixLeak_NetworkEventHandler?.Invoke(this, new FixLeakEventArgs(uavId, buttonText));
        }
        #endregion

        #region Target Detection Related
        [ServerRpc(RequireOwnership = false)]
        public void TargetDetectClickedServerRpc(int uavId)
        {
            TargetDetectClickedClientRpc(uavId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void TargetNotDetectClickedServerRpc(int uavId)
        {
            TargetNotDetectClickedClientRpc(uavId);
        }

        [ClientRpc]
        private void TargetDetectClickedClientRpc(int uavId)
        {
            TargetDetectClicked_NetworkEventHandler?.Invoke(this, uavId);
        }

        [ClientRpc]
        private void TargetNotDetectClickedClientRpc(int uavId)
        {
            TargetNotDetectClicked_NetworkEventHandler?.Invoke(this, uavId);
        }
        #endregion

        #region Chat Message Response Related
        [ServerRpc(RequireOwnership = false)]
        public void ChatReponseClickedServerRpc(NetworkString responseText)
        {
            ChatReponseClickedClientRpc(responseText);
        }

        [ClientRpc]
        private void ChatReponseClickedClientRpc(NetworkString responseText)
        {
            ChatResponseClicked_NetworkEventHandler?.Invoke(this, responseText);
        }
        #endregion
    }
}


