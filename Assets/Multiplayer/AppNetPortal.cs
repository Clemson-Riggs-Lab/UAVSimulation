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
    public class ClientConnectedEventArgs : EventArgs
    {
        public readonly ulong ClientId;
        public readonly int NumberOfClients;

        public ClientConnectedEventArgs(ulong clientId, int numberOfClients)
        {
            ClientId = clientId;
            NumberOfClients = numberOfClients;
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

        [Space(10)]
        [SerializeField] ConsoleMessageEventChannelSO writeMessageToConsoleChannel;

        private int _numberOfClients = 0;

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
            {
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Host Started. Waiting for Other Client" });
                return 1;
            }
            else
            {
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Unable to start Host" });
                return 0;
            }
        }

        public int StartClient(string ipAddress, int portNumber)
        {
            UNetTransport uNT = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            uNT.ConnectAddress = ipAddress;
            uNT.ConnectPort = portNumber;
            uNT.ServerListenPort = portNumber;

            if (_networkManager.StartClient())
            {
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Client Started" });
                return 1;
            }
            else
            {
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "red", doAnimate = true, text = "\n Unable to Join" });
                return 0;
            }
        }

        public int StopClient()
        {
            _networkManager.Shutdown();
            writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Client Stopped Successfully" });
            return 1;
        }

        public string GetDefaultIpAddr()
        {
            UNetTransport uNT = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            return uNT.ConnectAddress;
        }

        public int GetDefaultPortNo()
        {
            UNetTransport uNT = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            return uNT.ConnectPort;
        }

        public bool IsMultiplayerMode()
        {
            if (_networkManager.IsClient || _networkManager.IsServer)
                return true;
            else
                return false;
        }

        private void OnClientConnected(ulong obj)
        {
            _numberOfClients++;
            writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Client Connected With Id: " + obj });
            ClientConnected_EventHandler?.Invoke(this, new ClientConnectedEventArgs(obj, _numberOfClients));
        }

        private void OnClientDisconnected(ulong obj)
        {
            _numberOfClients--;
            writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Client Disconnected With Id: " + obj });
            ClientDisconnected_EventHandler?.Invoke(this, new ClientDisconnectedEventArgs(obj));
        }
    }
}


