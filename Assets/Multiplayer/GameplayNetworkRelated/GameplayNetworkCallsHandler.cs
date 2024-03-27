using System;
using System.Collections.Generic;
using UAVs;
using Unity.Netcode;
using UnityEngine;
using static Multiplayer.GameplayNetworkCallsData;

namespace Multiplayer
{
    public class GameplayNetworkCallsHandler : NetworkBehaviour
    {
        public event EventHandler<ReroutingPanelOpenEventArgs> ReroutePanelOpen_NetworkEventHandler;
        public event EventHandler<int> ReroutePanelClose_NetworkEventHandler;
        public event EventHandler<ReroutingUAVEventArgs> ReroutingUAV_NetworkEventHandler;
        public event EventHandler<ReroutePreviewEventArgs> ReroutePreview_NetworkEventHandler;
        public event EventHandler<ReroutePathMadeEventArgs> ReroutePathMade_NetworkEventHandler;

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
        public void ReroutingUAVOnServerRpc(CallerType callerType, int uavId, int optionIndex, string lastReroutOptLsOrderBase, int pathId_0, int pathId_1, int pathId_2)
        {
            ReroutingUAVOnClientRpc(callerType, uavId, optionIndex, lastReroutOptLsOrderBase, pathId_0, pathId_1, pathId_2);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReroutePanelOpenServerRpc(CallerType callerType, ulong localClientId, int uavId)
        {
            ReroutePanelOpenClientRpc(callerType, localClientId, uavId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReroutePanelCloseServerRpc(ulong localClientId, int uavId)
        {
            ReroutePanelCloseClientRpc(localClientId, uavId);
        }        
        
        [ServerRpc(RequireOwnership = false)]
        public void ReroutePreviewServerRpc(CallerType callerType, ulong localClientId, int uavId, int optionNumber, string lastReroutOptLsOrderBase, int pathId_0, int pathId_1, int pathId_2)
        {
            ReroutePreviewClientRpc(callerType, localClientId, uavId, optionNumber, lastReroutOptLsOrderBase, pathId_0, pathId_1, pathId_2);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReroutePathsMadeServerRpc(ulong localClientId, int uavId, string lastReroutOptLsOrderBase, int pathId_0, int pathId_1, int pathId_2)
        {
            ReroutePathsMadeClientRpc(localClientId, uavId, lastReroutOptLsOrderBase, pathId_0, pathId_1, pathId_2);
        }

        [ClientRpc]
        private void ReroutingUAVOnClientRpc(CallerType callerType, int uavId, int optionIndex, string lastReroutOptLsOrderBase, int pathId_0, int pathId_1, int pathId_2)
        {
            if (_callCallerTypeDict.ContainsKey(CallType.ReroutingUAV) == false)
                _callCallerTypeDict.Add(CallType.ReroutingUAV, callerType);

            ReroutingUAV_NetworkEventHandler?.Invoke(this, new ReroutingUAVEventArgs(uavId, optionIndex, lastReroutOptLsOrderBase, pathId_0, pathId_1, pathId_2));

            _callCallerTypeDict.Remove(CallType.ReroutingUAV);
        }

        [ClientRpc]
        private void ReroutePanelOpenClientRpc(CallerType callerType, ulong localClientId, int uavId)
        {
            if (_callCallerTypeDict.ContainsKey(CallType.RerouteOptionRequested) == false)
                _callCallerTypeDict.Add(CallType.RerouteOptionRequested, callerType);

            ReroutePanelOpen_NetworkEventHandler?.Invoke(this, new ReroutingPanelOpenEventArgs(uavId, localClientId));

            _callCallerTypeDict.Remove(CallType.RerouteOptionRequested);
        }

        [ClientRpc]
        private void ReroutePanelCloseClientRpc(ulong localClientId, int uavId)
        {
            if (AppNetPortal.Instance.LocalClientId != localClientId)
                ReroutePanelClose_NetworkEventHandler?.Invoke(this, uavId);
        }

        [ClientRpc]
        private void ReroutePreviewClientRpc(CallerType callerType, ulong localClientId, int uavId, int optionNumber, string lastReroutOptLsOrderBase, int pathId_0, int pathId_1, int pathId_2)
        {
            if (_callCallerTypeDict.ContainsKey(CallType.RerouteOptionPreviewed) == false)
                _callCallerTypeDict.Add(CallType.RerouteOptionPreviewed, callerType);

            ReroutePreview_NetworkEventHandler?.Invoke(this, new ReroutePreviewEventArgs(callerType, localClientId, uavId, optionNumber, lastReroutOptLsOrderBase, pathId_0, pathId_1, pathId_2));

            _callCallerTypeDict.Remove(CallType.RerouteOptionPreviewed);
        }

        [ClientRpc]
        private void ReroutePathsMadeClientRpc(ulong localClientId, int uavId, string lastReroutOptLsOrderBase, int pathId_0, int pathId_1, int pathId_2)
        {
            if (AppNetPortal.Instance.LocalClientId != localClientId)
                ReroutePathMade_NetworkEventHandler?.Invoke(this, new ReroutePathMadeEventArgs(localClientId, uavId, lastReroutOptLsOrderBase, pathId_0, pathId_1, pathId_2));

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


