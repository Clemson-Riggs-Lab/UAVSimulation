using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using Modules.Navigation.Channels.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UI.ReroutingPanel.Settings.ScriptableObjects;
using UnityEngine;

namespace Modules.Navigation.Submodules.Rerouting
{
	public class ReroutingManager : MonoBehaviour
	{
		public Dictionary<Uav, List<Path>> reroutingOptions = new();

		private UavEventChannelSO _reroutingOptionsRequestedChannel;
		private UavPathEventChannelSO _uavArrivedAtDestinationEventChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		private UavPathEventChannelSO _uavReroutedEventChannel;
		private UavPathEventChannelSO _uavReroutePreviewEventChannel;
		
		private ReroutingPanelSettingsSO _reroutingPanelSettings;
		
		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			
			var reroutingLogHandler = gameObject.GetOrAddComponent<ReroutingLogHandler>();
			reroutingLogHandler.Initialize();
		}


		private void OnDestroy()
		{
			UnsubscribeFromChannels();
		}

		private void SubscribeToChannels()
		{

			if (_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Subscribe(RemoveUavPanelAndOptions);
			if (_uavArrivedAtDestinationEventChannel != null)
				_uavArrivedAtDestinationEventChannel.Subscribe(UpdateReroutingOptions);
			if(_reroutingOptionsRequestedChannel != null)
				_reroutingOptionsRequestedChannel.Subscribe(PopulateReroutingOptions);
		}

		private void UpdateReroutingOptions(Uav uav, Path path)
		{
			if (reroutingOptions.ContainsKey(uav)) PopulateReroutingOptions(uav);
		}

		private void PopulateReroutingOptions(Uav uav)
		{
			var path = uav.currentPath;
			var possiblePathsWithNoNFZ = new List<Path>();
			var possiblePathsWithNFZ = new List<Path>();
			foreach (var waypoint in GameManager.Instance.wayPointsManager.wayPoints)
				if (waypoint != uav.currentPath.destinationWayPoint)
				{
					var newPath = new Path(path)
					{
						destinationWayPoint = waypoint
					};

					//linecast from uav to waypoint, and check if it hits an object in layer "NFZ"
					if (Physics.Linecast(uav.transform.position, waypoint.transform.position, 1 << LayerMask.NameToLayer("NFZ")))
						possiblePathsWithNFZ.Add(newPath);
					else
						possiblePathsWithNoNFZ.Add(newPath);
				}

			if (_reroutingPanelSettings.selectShortestPathsAsReroutingOptions)
			{
				//order the paths in each list by distance (between uav and waypoint) using linq
				possiblePathsWithNoNFZ = possiblePathsWithNoNFZ.OrderBy(p =>
					Vector3.Distance(uav.transform.position, p.destinationWayPoint.transform.position)).ToList();
				possiblePathsWithNFZ = possiblePathsWithNFZ.OrderBy(p =>
					Vector3.Distance(uav.transform.position, p.destinationWayPoint.transform.position)).ToList();
			}
			else //shuffle randomly
			{
				possiblePathsWithNFZ = possiblePathsWithNFZ.OrderBy(p => Guid.NewGuid()).ToList();
				possiblePathsWithNoNFZ = possiblePathsWithNoNFZ.OrderBy(p => Guid.NewGuid()).ToList();
			}

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
			//shuffle the list of rerouting options
			reroutingOptions[uav] = reroutingOptions[uav].OrderBy(x => Guid.NewGuid()).ToList();
		}

		public void PreviewPath(Uav uav, int optionIndex) 
		{
			if (optionIndex == -1) // -1 indicates that we want to cancel the preview
			{
				_uavReroutePreviewEventChannel.RaiseEvent(uav,uav.currentPath);
			}
			else //normal preview
			{
				_uavReroutePreviewEventChannel.RaiseEvent(uav,reroutingOptions[uav][optionIndex]);
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
	}
}