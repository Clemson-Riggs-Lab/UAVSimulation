using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Multiplayer.GameplayNetworkCallsData;

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

        public event EventHandler<bool> PauseBehaviour_NetworkEventHandler;

        public static GameplayNetworkCallsHandler Instance { get; private set; }

        private Dictionary<CallType, CallerType> _callCallerTypeDict = new Dictionary<CallType, CallerType>();

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

        public CallerType GetCallerType(CallType callType)
        {
            return _callCallerTypeDict[callType];
        }

        #region Rerouting Related
        [ServerRpc(RequireOwnership = false)]
        public void ReroutingUAVOnServerRpc(CallerType callerType, int uavId, int optionIndex, string lastReroutOptLsOrderBase)
        {
            ReroutingUAVOnClientRpc(callerType, uavId, optionIndex, lastReroutOptLsOrderBase);
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
        private void ReroutingUAVOnClientRpc(CallerType callerType, int uavId, int optionIndex, string lastReroutOptLsOrderBase)
        {
            _callCallerTypeDict.Add(CallType.ReroutingUAV, callerType);

            ReroutingUAV_NetworkEventHandler?.Invoke(this, new ReroutingUAVEventArgs(uavId, optionIndex, lastReroutOptLsOrderBase));

            _callCallerTypeDict.Remove(CallType.ReroutingUAV);
        }

        [ClientRpc]
        private void ReroutePanelOpenClientRpc(ulong localClientId, int uavId)
        {
            Debug.Log("UAV ID: " + uavId);

            if (AppNetPortal.Instance.LocalClientId != localClientId)
                ReroutePanelOpen_NetworkEventHandler?.Invoke(this, uavId);
        }

        [ClientRpc]
        private void ReroutePanelCloseClientRpc(ulong localClientId, int uavId)
        {
            Debug.Log("UAV ID: " + uavId);

            if (AppNetPortal.Instance.LocalClientId != localClientId)
                ReroutePanelClose_NetworkEventHandler?.Invoke(this, uavId);
        }
        #endregion

        #region FixLeak Related
        [ServerRpc(RequireOwnership = false)]
        public void FixLeakServerRpc(CallerType callerType, int uavId, NetworkString buttonText)
        {
            FixLeakClientRpc(callerType, uavId, buttonText);
        }

        [ClientRpc]
        private void FixLeakClientRpc(CallerType callerType, int uavId, NetworkString buttonText)
        {
            _callCallerTypeDict.Add(CallType.LeakFixed, callerType);
            FixLeak_NetworkEventHandler?.Invoke(this, new FixLeakEventArgs(uavId, buttonText));
            _callCallerTypeDict.Remove(CallType.LeakFixed);
        }
        #endregion

        #region Target Detection Related
        [ServerRpc(RequireOwnership = false)]
        public void TargetDetectClickedServerRpc(CallerType callerType, int uavId)
        {
            TargetDetectClickedClientRpc(callerType, uavId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void TargetNotDetectClickedServerRpc(CallerType callerType, int uavId)
        {
            TargetNotDetectClickedClientRpc(callerType, uavId);
        }

        [ClientRpc]
        private void TargetDetectClickedClientRpc(CallerType callerType, int uavId)
        {
            _callCallerTypeDict.Add(CallType.TargetDetectClicked, callerType);
            TargetDetectClicked_NetworkEventHandler?.Invoke(this, uavId);
            _callCallerTypeDict.Remove(CallType.TargetDetectClicked);
        }

        [ClientRpc]
        private void TargetNotDetectClickedClientRpc(CallerType callerType, int uavId)
        {
            _callCallerTypeDict.Add(CallType.TargetNotDetectedClicked, callerType);
            TargetNotDetectClicked_NetworkEventHandler?.Invoke(this, uavId);
            _callCallerTypeDict.Remove(CallType.TargetNotDetectedClicked);
        }
        #endregion

        #region Chat Message Response Related
        [ServerRpc(RequireOwnership = false)]
        public void ChatReponseClickedServerRpc(CallerType callerType, NetworkString responseText)
        {
            ChatReponseClickedClientRpc(callerType, responseText);
        }

        [ClientRpc]
        private void ChatReponseClickedClientRpc(CallerType callerType, NetworkString responseText)
        {
            _callCallerTypeDict.Add(CallType.ChatResponseClicked, callerType);
            ChatResponseClicked_NetworkEventHandler?.Invoke(this, responseText);
            _callCallerTypeDict.Remove(CallType.ChatResponseClicked);
        }
        #endregion

        #region Simulation Controls Related
        [ServerRpc(RequireOwnership = false)]
        public void PauseBehaviourServerRpc(bool pauseBehaviour)
        {
            PauseBehaviourClientRpc(pauseBehaviour);
        }

        [ClientRpc]
        private void PauseBehaviourClientRpc(bool pauseBehaviour)
        {
            PauseBehaviour_NetworkEventHandler?.Invoke(this, pauseBehaviour);
        }
        #endregion
    }
}


