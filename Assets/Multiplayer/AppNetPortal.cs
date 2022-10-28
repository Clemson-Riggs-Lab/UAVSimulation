using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

        [Space(10)]
        [SerializeField] ConsoleMessageEventChannelSO writeMessageToConsoleChannel;

        public ulong LocalClientId { get => _networkManager.LocalClientId; }
        public int ConnectedClientCount { get => _networkManager.ConnectedClientsList.Count; }
        public bool IsServer { get => _networkManager.IsServer; }

        public bool IsThisHost { get => _networkManager.IsHost; }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            _networkManager.OnClientConnectedCallback += OnClientConnected;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
            _networkManager.OnTransportFailure += OnTransportFailure;
        }

        public int StartHost(string ipAddress, int portNumber)
        {
            UNetTransport uNT = (UNetTransport)_networkManager.NetworkConfig.NetworkTransport;
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
            UNetTransport uNT = (UNetTransport)_networkManager.NetworkConfig.NetworkTransport;
            uNT.ConnectAddress = ipAddress;
            uNT.ConnectPort = portNumber;

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

        public bool IsMultiplayerMode()
        {
            if (_networkManager.IsClient || _networkManager.IsServer)
                return true;
            else
                return false;
        }
        
        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return null;
        }

        public int GetDefaultPortNo()
        {
            UNetTransport uNT = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            return uNT.ConnectPort;
        }

        private void OnClientConnected(ulong obj)
        {
            writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Client Connected With Id: " + obj });

            if (_networkManager.IsServer && _networkManager.ConnectedClientsList.Count > 2)
            {
                _networkManager.DisconnectClient(obj);
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Max Client Limit Reached. Connection Refused: " + obj });
            }
            else
                ClientConnected_EventHandler?.Invoke(this, new ClientConnectedEventArgs(obj));

            if (_networkManager.IsServer)
            {
                UNetTransport uNT = (UNetTransport)_networkManager.NetworkConfig.NetworkTransport;
                writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Kindly Enter IP Address: " + uNT.ConnectAddress + " & Port: " + uNT.ConnectPort  + " on other computer"});
            }
        }

        private void OnClientDisconnected(ulong obj)
        {
            writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Client Disconnected With Id: " + obj });

            if (_networkManager.IsClient && !_networkManager.IsServer)
                ClientDisconnected_EventHandler?.Invoke(this, new ClientDisconnectedEventArgs(obj));
            else if (_networkManager.IsServer && _networkManager.ConnectedClientsList.ToList().Exists(x => x.ClientId == obj))
                ClientDisconnected_EventHandler?.Invoke(this, new ClientDisconnectedEventArgs(obj));
        }

        private void OnTransportFailure()
        {
            writeMessageToConsoleChannel.RaiseEvent("", new() { color = "green", doAnimate = true, text = "\n Transport Failure Happens "});
        }
    }
}


