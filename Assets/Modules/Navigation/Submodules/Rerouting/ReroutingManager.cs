using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Settings.ScriptableObjects;
using Modules.Navigation.Submodules.Rerouting.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UI.ReroutingPanel.Settings.ScriptableObjects;
using UnityEngine;
using Random = System.Random;

namespace Modules.Navigation.Submodules.Rerouting
{
	public class ReroutingManager : MonoBehaviour
	{
		public Dictionary<Uav, List<Path>> reroutingOptions = new();

		private UavEventChannelSO _reroutingOptionsRequestedChannel;
		private UavEventChannelSO _oneClickReroutingRequestedChannel;
		private UavPathEventChannelSO _uavArrivedAtDestinationEventChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		private UavPathEventChannelSO _uavReroutedEventChannel;
		private UavPathEventChannelSO _uavReroutePreviewEventChannel;
		private ReroutingSettingsSO _reroutingSettings;
		private ReroutingPanelSettingsSO _reroutingPanelSettings;
		private NavigationSettingsSO _navigationSettings;
		private Random _shuffleReroutingOptionsRandomGenerator= new Random();
		
		private Random oneClickRerouteRandomGenerator= new Random();

		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			
			_shuffleReroutingOptionsRandomGenerator = new Random(_reroutingPanelSettings.shuffleReroutingOptionsRandomGeneratorSeed);
			oneClickRerouteRandomGenerator = new Random(GameManager.Instance.settingsDatabase.reroutingSettings.oneClickRerouteRandomSeed);

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
			if (_oneClickReroutingRequestedChannel != null)
				_oneClickReroutingRequestedChannel.Subscribe(OneClickReroute);
		}

		private void PopulateReroutingOptions(Uav uav)
		{
			PopulateReroutingOptions(uav,false);
		}

		private void OneClickReroute(Uav uav)
		{
			PopulateReroutingOptions(uav,true);
			
		}

		private void UpdateReroutingOptions(Uav uav, Path path)
		{
			if (reroutingOptions.ContainsKey(uav)) PopulateReroutingOptions(uav);
		}

		private void PopulateReroutingOptions(Uav uav,bool oneClickReroute=false)
		{
			var path = uav.currentPath;
			var possiblePathsWithNoNFZ = new List<Path>();
			 var possiblePathsWithNFZ = new List<Path>();
			foreach (var waypoint in GameManager.Instance.wayPointsManager.wayPoints)
				if (waypoint != uav.currentPath.destinationWayPoint)
				{
					var newPath = new Path(path)
					{
						destinationWayPoint = waypoint, // same as original path, just with a different destination
						headingToNFZ =  Physics.Linecast(uav.transform.position, waypoint.transform.position, 1 << LayerMask.NameToLayer("NFZ"))
					};

					//linecast from uav to waypoint, and check if it hits an object in layer "NFZ"
					if (newPath.headingToNFZ)
						possiblePathsWithNFZ.Add(newPath);
					else
						possiblePathsWithNoNFZ.Add(newPath);
				}
			

			
			var minDuration = _navigationSettings.minPathDuration;
			var maxDuration = _navigationSettings.maxPathDuration;
			var speed=_navigationSettings.fixedSpeed;
			var minDistance = minDuration * speed;
			var maxDistance = maxDuration * speed;
			
			//update below so that we reject paths that are shorter than minimum duration or longer than maximum duration, also shuffle
			possiblePathsWithNoNFZ = possiblePathsWithNoNFZ
				.OrderBy(p => Vector3.Distance(uav.transform.position, p.destinationWayPoint.transform.position))
				.Where(p => {
					var distance = Vector3.Distance(uav.transform.position, p.destinationWayPoint.transform.position);
					return distance >= minDistance && distance <= maxDistance;
				}).OrderBy(p => Guid.NewGuid())
				.ToList();

			possiblePathsWithNFZ = possiblePathsWithNFZ
				.OrderBy(p => Vector3.Distance(uav.transform.position, p.destinationWayPoint.transform.position))
				.Where(p => {
					var distance = Vector3.Distance(uav.transform.position, p.destinationWayPoint.transform.position);
					return distance >= minDistance && distance <= maxDistance;
				}).OrderBy(p => Guid.NewGuid())
				.ToList();
			
			// add default option if there are no paths with no NFZ
			if (possiblePathsWithNoNFZ.Count == 0)
				possiblePathsWithNoNFZ.Add(path);
			if (possiblePathsWithNFZ.Count == 0)
				possiblePathsWithNFZ.Add(path);
			
			//if one click reroute is enabled, then we reroute to the first path in the list
			if (oneClickReroute)
			{
				var random = oneClickRerouteRandomGenerator.NextDouble();
				if (random <=  _reroutingSettings.probabilityOfUnsuccessfulOneClickReroute)
				{
					_uavReroutedEventChannel.RaiseEvent(uav, possiblePathsWithNFZ[0]);
					return;
				}
				else
				{
					_uavReroutedEventChannel.RaiseEvent(uav, possiblePathsWithNoNFZ[0]);
					return;
				}

			}
			//////////////end of one click reroute //////////////////////
			
			
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
			reroutingOptions[uav] = reroutingOptions[uav].OrderBy(x => _shuffleReroutingOptionsRandomGenerator.Next()).ToList();
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
			_navigationSettings = GameManager.Instance.settingsDatabase.navigationSettings;
			_reroutingOptionsRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.reroutingOptionsRequestedChannel;
			_oneClickReroutingRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.oneClickReroutingRequestedChannel;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavArrivedAtDestinationEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavArrivedAtDestinationEventChannel;
			_uavReroutedEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutedEventChannel;
			_uavReroutePreviewEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutePreviewEventChannel;
			_reroutingSettings = GameManager.Instance.settingsDatabase.reroutingSettings;
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