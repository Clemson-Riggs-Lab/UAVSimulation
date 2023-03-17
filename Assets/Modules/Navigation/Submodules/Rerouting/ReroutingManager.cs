using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using Modules.Navigation.Channels.ScriptableObjects;
using Multiplayer;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UI.ReroutingPanel;
using UI.ReroutingPanel.Settings.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Modules.Navigation.Submodules.Rerouting
{
    public class ReroutingManager : MonoBehaviour
    {
        public static event EventHandler<RerouteOptionPreviewedEventArgs> DirectLogRerouteOptionPreviewed_EventHandler;

        public Dictionary<Uav, List<Path>> reroutingOptions = new();

        private UavEventChannelSO _reroutingOptionsRequestedChannel;
        private UavPathEventChannelSO _uavArrivedAtDestinationEventChannel;
        private UavEventChannelSO _uavDestroyedEventChannel;
        private UavPathEventChannelSO _uavReroutedEventChannel;
        private UavPathEventChannelSO _uavReroutePreviewEventChannel;

        private ReroutingPanelSettingsSO _reroutingPanelSettings;
        private UavsManager _uavsManager;

        private Guid _lastReroutOptLsOrderBase;

        public Guid LastReroutOptLsOrderBase { get => _lastReroutOptLsOrderBase; }

        private void Start()
        {
            GetReferencesFromGameManager();
            SubscribeToChannels();

            var reroutingLogHandler = gameObject.GetOrAddComponent<ReroutingLogHandler>();
            reroutingLogHandler.Initialize();

            if (AppNetPortal.Instance.IsMultiplayerMode())
            {
                GameplayNetworkCallsHandler.Instance.ReroutingUAV_NetworkEventHandler += OnReroutingUAVNetworkEventHandler;
                GameplayNetworkCallsHandler.Instance.ReroutePreview_NetworkEventHandler += OnReroutePreviewNetworkEventHandler;
                GameplayNetworkCallsHandler.Instance.ReroutePathMade_NetworkEventHandler += OnReroutePathMadeNetworkEventHandler;
            }
        }

        private void SubscribeToChannels()
        {
            if (_uavDestroyedEventChannel != null)
                _uavDestroyedEventChannel.Subscribe(RemoveUavPanelAndOptions);
            if (_uavArrivedAtDestinationEventChannel != null)
                _uavArrivedAtDestinationEventChannel.Subscribe(UpdateReroutingOptions);
            if (_reroutingOptionsRequestedChannel != null)
                _reroutingOptionsRequestedChannel.Subscribe(PopulateReroutingOptions);
        }

        private void UpdateReroutingOptions(Uav uav, Path path)
        {
            if (reroutingOptions.ContainsKey(uav)) PopulateReroutingOptions(uav);
        }

        public void PopulateReroutingOptions(Uav uav)
        {
            Tuple<List<Path>, List<Path>> tuple = GetPossibleNFZAndNoNFZ(uav);
            var possiblePathsWithNFZ = tuple.Item1;
            var possiblePathsWithNoNFZ = tuple.Item2;

            if (_reroutingPanelSettings.selectShortestPathsAsReroutingOptions)
            {
                //order the paths in each list by distance (between uav and waypoint) using linq
                possiblePathsWithNoNFZ = possiblePathsWithNoNFZ.OrderBy(p => Vector3.Distance(uav.transform.position, p.destinationWayPoint.transform.position)).ToList();
                possiblePathsWithNFZ = possiblePathsWithNFZ.OrderBy(p => Vector3.Distance(uav.transform.position, p.destinationWayPoint.transform.position)).ToList();
            }
            else //shuffle randomly
            {
                var rand = new Random();
                possiblePathsWithNFZ = possiblePathsWithNFZ.OrderBy(p => rand.Next()).ToList();
                possiblePathsWithNoNFZ = possiblePathsWithNoNFZ.OrderBy(p => rand.Next()).ToList();
            }

            AddInReroutingOptions(uav, possiblePathsWithNFZ, possiblePathsWithNoNFZ);
            
            var random = new Random();
            reroutingOptions[uav] =  reroutingOptions[uav].OrderBy(x => random.Next()).ToList();

            if (AppNetPortal.Instance.IsMultiplayerMode())
            {
                GameplayNetworkCallsHandler.Instance.ReroutePathsMadeServerRpc(AppNetPortal.Instance.LocalClientId, uav.id, _lastReroutOptLsOrderBase.ToString(),
                        reroutingOptions[uav][0].id, reroutingOptions[uav][1].id, reroutingOptions[uav][2].id);
            }
        }

        private void PopulateReroutingOptionsFromNetworkData(Uav uav, string lastReroutOptLsOrderBaseStr, int pathId_0, int pathId_1, int pathId_2)
        {
            Guid lastReroutOptLsOrderBase = new Guid(lastReroutOptLsOrderBaseStr);

            Tuple<List<Path>, List<Path>> tuple = GetPossibleNFZAndNoNFZ(uav);
            var possiblePathsWithNFZ = tuple.Item1;
            var possiblePathsWithNoNFZ = tuple.Item2;

            if (_reroutingPanelSettings.selectShortestPathsAsReroutingOptions)
            {
                List<Path> newLs = new List<Path>();

                if (possiblePathsWithNFZ.Count != 0)
                {
                    newLs.Add(possiblePathsWithNFZ.Find(x => x.id == pathId_0));
                    newLs.Add(possiblePathsWithNFZ.Find(x => x.id == pathId_1));
                    newLs.Add(possiblePathsWithNFZ.Find(x => x.id == pathId_2));

                    possiblePathsWithNFZ.InsertRange(0, newLs);
                    possiblePathsWithNFZ = possiblePathsWithNFZ.Distinct().ToList();
                }

                newLs.Clear();

                newLs.Add(possiblePathsWithNoNFZ.Find(x => x.id == pathId_0));
                newLs.Add(possiblePathsWithNoNFZ.Find(x => x.id == pathId_1));
                newLs.Add(possiblePathsWithNoNFZ.Find(x => x.id == pathId_2));
                possiblePathsWithNoNFZ.InsertRange(0, newLs);
                possiblePathsWithNoNFZ = possiblePathsWithNoNFZ.Distinct().ToList();
            }
            else //shuffle randomly
            {
                possiblePathsWithNFZ = possiblePathsWithNFZ.OrderBy(p => lastReroutOptLsOrderBase).ToList();
                possiblePathsWithNoNFZ = possiblePathsWithNoNFZ.OrderBy(p => lastReroutOptLsOrderBase).ToList();
            }

            AddInReroutingOptions(uav, possiblePathsWithNFZ, possiblePathsWithNoNFZ);

            reroutingOptions[uav] = reroutingOptions[uav].OrderBy(x => lastReroutOptLsOrderBase).ToList();
        }

        public void PreviewPath(Uav uav, int optionIndex)
        {
            if (AppNetPortal.Instance.IsMultiplayerMode())
            {
                if (reroutingOptions.ContainsKey(uav) && reroutingOptions[uav].Count != 0)
                    GameplayNetworkCallsHandler.Instance.ReroutePreviewServerRpc(AppNetPortal.Instance.IsThisHost ? CallerType.Host : CallerType.Client, AppNetPortal.Instance.LocalClientId, uav.id, optionIndex, LastReroutOptLsOrderBase.ToString(),
                        reroutingOptions[uav][0].id, reroutingOptions[uav][1].id, reroutingOptions[uav][2].id);
                else
                    GameplayNetworkCallsHandler.Instance.ReroutePreviewServerRpc(AppNetPortal.Instance.IsThisHost ? CallerType.Host : CallerType.Client, AppNetPortal.Instance.LocalClientId, uav.id, optionIndex, LastReroutOptLsOrderBase.ToString(),
                        -1, -1, -1);
            }
            else
            {
                if (optionIndex == -1) // -1 indicates that we want to cancel the preview
                {
                    _uavReroutePreviewEventChannel.RaiseEvent(uav, uav.currentPath);
                }
                else //normal preview
                {
                    _uavReroutePreviewEventChannel.RaiseEvent(uav, reroutingOptions[uav][optionIndex]);
                }
            }
        }

        public void RerouteUav(Uav uav, int optionIndex)
        {
            _uavReroutedEventChannel.RaiseEvent(uav, reroutingOptions[uav][optionIndex]);
            RemoveUavPanelAndOptions(uav);
        }

        public void RemoveUavPanelAndOptions(Uav uav)
        {
            PreviewPath(uav, -1); // make sure to cancel a preview if it is active
            reroutingOptions.Remove(uav);
        }

        private void GetReferencesFromGameManager()
        {
            _reroutingPanelSettings = GameManager.Instance.settingsDatabase.reroutingPanelSettings;
            _reroutingOptionsRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.reroutingOptionsRequestedChannel;
            _uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
            _uavArrivedAtDestinationEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavArrivedAtDestinationEventChannel;
            _uavReroutedEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutedEventChannel;
            _uavReroutePreviewEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutePreviewEventChannel;
            _uavsManager = GameManager.Instance.uavsManager;
        }

        private void UnsubscribeFromChannels()
        {

            if (_uavDestroyedEventChannel != null)
                _uavDestroyedEventChannel.Unsubscribe(RemoveUavPanelAndOptions);

            if (_uavArrivedAtDestinationEventChannel != null)
                _uavArrivedAtDestinationEventChannel.Unsubscribe(UpdateReroutingOptions);

            if (_reroutingOptionsRequestedChannel != null)
                _reroutingOptionsRequestedChannel.Unsubscribe(PopulateReroutingOptions);
        }

        private void OnReroutingUAVNetworkEventHandler(object sender, ReroutingUAVEventArgs e)
        {
            Uav uav = _uavsManager.GetUAVAgainstId(e.UavId);

            RerouteUav(uav, e.OptionIndex);
        }

        private void OnReroutePreviewNetworkEventHandler(object sender, ReroutePreviewEventArgs e)
        {
            Uav uav = _uavsManager.GetUAVAgainstId(e.UavId);

            if (AppNetPortal.Instance.LocalClientId == e.LocalClientId)
            {
                if (e.OptionNumber == -1) // -1 indicates that we want to cancel the preview
                {
                    _uavReroutePreviewEventChannel.RaiseEvent(uav, uav.currentPath);
                }
                else //normal preview
                {
                    _uavReroutePreviewEventChannel.RaiseEvent(uav, reroutingOptions[uav][e.OptionNumber]);
                }
            }
            else
            {
                DirectLogRerouteOptionPreviewed_EventHandler?.Invoke(this, new RerouteOptionPreviewedEventArgs(uav, e.OptionNumber == -1 ? uav.currentPath : reroutingOptions[uav][e.OptionNumber]));
            }
        }

        private void OnReroutePathMadeNetworkEventHandler(object sender, ReroutePathMadeEventArgs e)
        {
            Uav uav = _uavsManager.GetUAVAgainstId(e.UavId);

            PopulateReroutingOptionsFromNetworkData(uav, e.LastReroutOptLsOrderBase, e.PathId_0, e.PathId_1, e.PathId_2);
        }

        private Tuple<List<Path>, List<Path>> GetPossibleNFZAndNoNFZ(Uav uav)
        {
            var path = uav.currentPath;
            var possiblePathsWithNoNFZ = new List<Path>();
            var possiblePathsWithNFZ = new List<Path>();
            foreach (var waypoint in GameManager.Instance.wayPointsManager.wayPoints)
                if (waypoint != uav.currentPath.destinationWayPoint)
                {
                    var newPath = new Path(path)
                    {
                        id = waypoint.id,
                        destinationWayPoint = waypoint // same as original path, just with a different destination
                    };

                    //linecast from uav to waypoint, and check if it hits an object in layer "NFZ"
                    if (Physics.Linecast(uav.transform.position, waypoint.transform.position, 1 << LayerMask.NameToLayer("NFZ")))
                        possiblePathsWithNFZ.Add(newPath);
                    else
                        possiblePathsWithNoNFZ.Add(newPath);
                }

            return new Tuple<List<Path>, List<Path>>(possiblePathsWithNFZ, possiblePathsWithNoNFZ);
        }

        private void AddInReroutingOptions(Uav uav, List<Path> possiblePathsWithNFZ, List<Path> possiblePathsWithNoNFZ)
        {
            //checking how many good/bad paths to add
            var numberOfBadReroutingOptionsToPresent = _reroutingPanelSettings.numberOfBadReroutingOptionsToPresent;
            //if the number of paths with NFZ is less than the number of bad rerouting options to present, then present all of them
            if (possiblePathsWithNFZ.Count < numberOfBadReroutingOptionsToPresent)
                numberOfBadReroutingOptionsToPresent = possiblePathsWithNFZ.Count;
            var numberOfGoodReroutingOptionsToPresent = _reroutingPanelSettings.numberOfReroutingOptionsToPresent - numberOfBadReroutingOptionsToPresent;

            //Adding pathOptions to Dictionary
            reroutingOptions[uav] = new List<Path>();

            //add the paths with NFZ to the list of rerouting options
            for (var i = 0; i < numberOfBadReroutingOptionsToPresent; i++)
                reroutingOptions[uav].Add(possiblePathsWithNFZ[i]);
            //add the paths with no NFZ to the list of rerouting options
            for (var i = 0; i < numberOfGoodReroutingOptionsToPresent; i++)
                reroutingOptions[uav].Add(possiblePathsWithNoNFZ[i]);
        }
    }
}