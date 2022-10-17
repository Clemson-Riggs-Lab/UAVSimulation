using System;
using System.Collections.Generic;
using ScriptableObjects.EventChannels;
using SyedAli.Main;
using UI.Console.Channels.ScriptableObjects;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints.Channels.ScriptableObjects;

namespace Multiplayer
{
    public class GameplayNetworkCallsData : NetworkBehaviour
    {
        public struct ReroutingDataStruct : INetworkSerializable
        {
            public int UAVId;
            public int OptionNo;

            public ReroutingDataStruct(int uavId, int optionNo)
            {
                UAVId = uavId;
                OptionNo = optionNo;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref UAVId);
                serializer.SerializeValue(ref OptionNo);
            }
        }
    }
}


