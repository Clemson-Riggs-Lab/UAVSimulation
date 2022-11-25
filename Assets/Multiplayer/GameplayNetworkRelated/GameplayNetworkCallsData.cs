using System;
using System.Collections.Generic;
using ScriptableObjects.EventChannels;
using SyedAli.Main;
using UAVs;
using UI.Console.Channels.ScriptableObjects;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints.Channels.ScriptableObjects;

namespace Multiplayer
{
    public enum CallerType
    {
        None, Host, Client
    }

    public enum CallType
    {
        ReroutingUAV, RerouteOptionRequested, RerouteOptionPreviewed, LeakFixed, TargetDetectClicked, TargetNotDetectedClicked, ChatResponseClicked, PauseBehaviour
    }

    public class ReroutingUAVEventArgs : EventArgs
    {
        public readonly int UavId;
        public readonly int OptionIndex;
        public readonly string LastReroutOptLsOrderBase;
        public readonly int PathId_0;
        public readonly int PathId_1;
        public readonly int PathId_2;

        public ReroutingUAVEventArgs(int uavId, int optionIndex, string lastReroutOptLsOrderBase, int pathId_0, int pathId_1, int pathId_2)
        {
            UavId = uavId;
            OptionIndex = optionIndex;
            LastReroutOptLsOrderBase = lastReroutOptLsOrderBase;
            PathId_0 = pathId_0;
            PathId_1 = pathId_1;
            PathId_2 = pathId_2;
        }
    }

    public class ReroutingPanelOpenEventArgs : EventArgs
    {
        public readonly int UavId;
        public readonly ulong LocalClientId;

        public ReroutingPanelOpenEventArgs(int uavId, ulong localClientId)
        {
            UavId = uavId;
            LocalClientId = localClientId;
        }
    }

    public class FixLeakEventArgs : EventArgs
    {
        public readonly int UavId;
        public readonly string ButtonText;

        public FixLeakEventArgs(int uavId, string buttonText)
        {
            UavId = uavId;
            ButtonText = buttonText;
        }
    }

    public class ReroutePreviewEventArgs : EventArgs
    {
        public readonly CallerType CallerType;
        public readonly ulong LocalClientId;
        public readonly int UavId;
        public readonly int OptionNumber;
        public readonly string LastReroutOptLsOrderBase;
        public readonly int PathId_0;
        public readonly int PathId_1;
        public readonly int PathId_2;

        public ReroutePreviewEventArgs(CallerType callerType, ulong localClientId, int uavId, int optionNumber, string lastReroutOptLsOrderBase, int pathId_0, int pathId_1, int pathId_2)
        {
            CallerType = callerType;
            LocalClientId = localClientId;
            UavId = uavId;
            OptionNumber = optionNumber;
            LastReroutOptLsOrderBase = lastReroutOptLsOrderBase;
            PathId_0 = pathId_0;
            PathId_1 = pathId_1;
            PathId_2 = pathId_2;
        }
    }    
    
    public class ReroutePathMadeEventArgs : EventArgs
    {
        public readonly ulong LocalClientId;
        public readonly int UavId;
        public readonly string LastReroutOptLsOrderBase;
        public readonly int PathId_0;
        public readonly int PathId_1;
        public readonly int PathId_2;

        public ReroutePathMadeEventArgs(ulong localClientId, int uavId, string lastReroutOptLsOrderBase, int pathId_0, int pathId_1, int pathId_2)
        {
            LocalClientId = localClientId;
            UavId = uavId;
            LastReroutOptLsOrderBase = lastReroutOptLsOrderBase;
            PathId_0 = pathId_0;
            PathId_1 = pathId_1;
            PathId_2 = pathId_2;
        }
    }

    public class GameplayNetworkCallsData : NetworkBehaviour
    {
        public struct NetworkString : INetworkSerializable
        {
            private FixedString32Bytes info;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref info);
            }

            public override string ToString()
            {
                return info.ToString();
            }

            public static implicit operator string(NetworkString s) => s.ToString();
            public static implicit operator NetworkString(string s) => new NetworkString() { info = new FixedString32Bytes(s) };
        }
    }
}

