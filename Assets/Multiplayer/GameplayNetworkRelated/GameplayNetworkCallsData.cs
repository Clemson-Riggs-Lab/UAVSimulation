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
    public class ReroutingUAVEventArgs : EventArgs
    {
        public readonly int UavId;
        public readonly int OptionIndex;
        public readonly string LastReroutOptLsOrderBase;

        public ReroutingUAVEventArgs(int uavId, int optionIndex, string lastReroutOptLsOrderBase)
        {
            UavId = uavId;
            OptionIndex = optionIndex;
            LastReroutOptLsOrderBase = lastReroutOptLsOrderBase;
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


