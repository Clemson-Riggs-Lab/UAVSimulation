using System;
using System.Collections.Generic;
using ScriptableObjects.EventChannels;
using SyedAli.Main;
using UI.Console.Channels.ScriptableObjects;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints.Channels.ScriptableObjects;

namespace Multiplayer
{
    public class MainMenuNetworkCallsData : NetworkBehaviour
    {
        public struct NetworkString : INetworkSerializable
        {
            private FixedString4096Bytes info;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref info);
            }

            public override string ToString()
            {
                return info.ToString();
            }

            public static implicit operator string(NetworkString s) => s.ToString();
            public static implicit operator NetworkString(string s) => new NetworkString() { info = new FixedString4096Bytes(s) };
        }
    }
}


