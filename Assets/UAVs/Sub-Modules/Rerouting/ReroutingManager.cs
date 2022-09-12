using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.Navigation;
using ScriptableObjects.UAVs.Navigation.Rerouting;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;

namespace UAVs.Sub_Modules.Rerouting
{
	public class ReroutingManager : MonoBehaviour
	{
		public GameObject reroutingButtonsContainer;
		public GameObject reroutingOptionsPanelsContainer;

		private ReroutingButtonsContainerController reroutingButtonsContainerController;

		public Dictionary<Uav, List<Path>> reroutingOptions = new();
		private UavEventChannelSO reroutingOptionsRequestedChannel;
		private ReroutingPanelsContainerController reroutingPanelsContainerController;


		private ReroutingSettingsSO reroutingSettings;

		private UavPathEventChannelSO uavArrivedAtDestinationEventChannel;

		private UavEventChannelSO uavCreatedEventChannel;
		private UavEventChannelSO uavDestroyedEventChannel;
		private UavEventChannelSO uavLostEventChannel;
		private UavPathEventChannelSO uavReroutedEventChannel;
		private UavPathEventChannelSO uavReroutePreviewEventChannel;

		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();

			if (reroutingButtonsContainer.TryGetComponent(out reroutingButtonsContainerController))
				reroutingButtonsContainerController.Initialize(this);
			else Debug.LogError("ReroutingButtonsContainerController not found");

			if (reroutingOptionsPanelsContainer.TryGetComponent(out reroutingPanelsContainerController))
				reroutingPanelsContainerController.Initialize(this);
			else Debug.LogError("ReroutingPanelsContainerController not found");
		}


		private void OnDestroy()
		{
			UnsubscribeFromChannels();
		}
		
		public void OnRerouteOptionsRequested(Uav uav)
		{
			reroutingOptionsRequestedChannel.RaiseEvent(uav);
			reroutingPanelsContainerController.AddPanel(uav);
			PopulateReroutingOptions(uav);
		}

		private void SubscribeToChannels()
		{
			if (uavCreatedEventChannel != null)
				uavCreatedEventChannel.Subscribe(CreateButton);

			if (uavDestroyedEventChannel != null)
				uavDestroyedEventChannel.Subscribe(RemoveUav);

			if (uavArrivedAtDestinationEventChannel != null)
				uavArrivedAtDestinationEventChannel.Subscribe(UpdateReroutingOptions);
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
					if (Physics.Linecast(uav.transform.position, waypoint.transform.position,
						    1 << LayerMask.NameToLayer("NFZ")))
						possiblePathsWithNFZ.Add(newPath);
					else
						possiblePathsWithNoNFZ.Add(newPath);
				}

			if (reroutingSettings.selectShortestPathsAsReroutingOptions)
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
			var numberOfBadReroutingOptionsToPresent = reroutingSettings.numberOfBadReroutingOptionsToPresent;
			//if the number of paths with NFZ is less than the number of bad rerouting options to present, then present all of them
			if (possiblePathsWithNFZ.Count < numberOfBadReroutingOptionsToPresent)
				numberOfBadReroutingOptionsToPresent = possiblePathsWithNFZ.Count;
			var numberOfGoodReroutingOptionsToPresent = reroutingSettings.numberOfReroutingOptionsToPresent - numberOfBadReroutingOptionsToPresent;

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
			uavReroutePreviewEventChannel.RaiseEvent(uav, reroutingOptions[uav][optionIndex]);
		}

		public void RerouteUav(Uav uav, int optionIndex)
		{
			uavReroutedEventChannel.RaiseEvent(uav, reroutingOptions[uav][optionIndex]);
			reroutingPanelsContainerController.RemovePanel(uav);
			reroutingOptions.Remove(uav);
		}

		private void RemoveUav(Uav uav)
		{
			reroutingButtonsContainerController.RemoveButton(uav);
			RemoveUavPanelAndOptions(uav);
		}

		public void RemoveUavPanelAndOptions(Uav uav)
		{
			reroutingPanelsContainerController.RemovePanel(uav);
			reroutingOptions.Remove(uav);
		}

		private void CreateButton(Uav uav)
		{
			reroutingButtonsContainerController.CreateButton(uav);
		}


		private void GetReferencesFromGameManager()
		{
			reroutingSettings = GameManager.Instance.settingsDatabase.reroutingSettings;
			reroutingOptionsRequestedChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.reroutingOptionsRequestedChannel;
			uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			uavLostEventChannel = GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavLostEventChannel;
			uavArrivedAtDestinationEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavArrivedAtDestinationEventChannel;
			uavReroutedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavReroutedEventChannel;
			uavReroutePreviewEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavReroutePreviewEventChannel;
		}

		private void UnsubscribeFromChannels()
		{
			if (uavCreatedEventChannel != null)
				uavCreatedEventChannel.Unsubscribe(CreateButton);

			if (uavDestroyedEventChannel != null)
				uavDestroyedEventChannel.Unsubscribe(RemoveUav);
			if (uavArrivedAtDestinationEventChannel != null)
				uavArrivedAtDestinationEventChannel.Unsubscribe(UpdateReroutingOptions);
		}
	}
}