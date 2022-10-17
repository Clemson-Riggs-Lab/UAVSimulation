using System;
using System.Collections.Generic;
using ScriptableObjects.EventChannels;
using SyedAli.Main;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using Unity.VisualScripting;
using UnityEngine;
using WayPoints.Channels.ScriptableObjects;

namespace Multiplayer
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public readonly ulong ClientId;

        public ClientConnectedEventArgs(ulong clientId)
        {
            ClientId = clientId;
        }
    }

    public class ClientDisconnectedEventArgs : EventArgs
    {
        public readonly ulong ClientId;

        public ClientDisconnectedEventArgs(ulong clientId)
        {
            ClientId = clientId;
        }
    }

    public class AppNetPortal : PersistentSingleton<AppNetPortal>
    {
        public event EventHandler<ClientConnectedEventArgs> ClientConnected_EventHandler;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected_EventHandler;

        [SerializeField] NetworkManager _networkManager;

        public NetworkManager NetworkManager { get => _networkManager; }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _networkManager.OnClientConnectedCallback += OnClientConnected;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        }

        private void OnDestroy()
        {
            _networkManager.OnClientConnectedCallback -= OnClientConnected;
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        public int StartHost(string ipAddress, int portNumber)
        {
            UNetTransport uNT = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            uNT.ConnectAddress = ipAddress;
            uNT.ConnectPort = portNumber;
            uNT.ServerListenPort = portNumber;

            if (_networkManager.StartHost())
                return 1;
            else
                return 0;
        }

        public int StartClient(string ipAddress, int portNumber)
        {
            UNetTransport uNT = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            uNT.ConnectAddress = ipAddress;
            uNT.ConnectPort = portNumber;
            uNT.ServerListenPort = portNumber;

            if (_networkManager.StartClient())
                return 1;
            else
                return 0;
        }

        public int StopClient()
        {
            _networkManager.Shutdown();
            return 1;
        }

        private void OnClientConnected(ulong obj)
        {
            ClientConnected_EventHandler?.Invoke(this, new ClientConnectedEventArgs(obj));
        }

        private void OnClientDisconnected(ulong obj)
        {
            ClientDisconnected_EventHandler?.Invoke(this, new ClientDisconnectedEventArgs(obj));
        }
    }
}


