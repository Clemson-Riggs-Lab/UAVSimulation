using System;
using System.Collections.Generic;
using IOHandlers.Records;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.Navigation;
using UAVs.Navigation;
using UnityEngine;
using WayPoints;

namespace UAVs.Sub_Modules.Navigation
{
	public class NavigationManager: MonoBehaviour
	{

		[NonSerialized] public Dictionary<Uav,Navigator> navigators = new ();
		
		 private PathsGenerator _pathsGenerator ;
		 private WayPointsManager _wayPointsManager;
		 private UavsManager _uavsManager;
		 private NavigationSettingsSO _navigationSettings;
		 private List<UavPathsRecord>  _uavPathsRecord;
		 
		 private UavPathEventChannelSO uavReroutedEventChannel;
		 private UavEventChannelSO uavShotDownChannel;
		 private UavEventChannelSO uavDestroyedEventChannel;
		 private UavEventChannelSO uavLostEventChannel;
		
		private void Start()
		{
			GetReferencesFromGameManager();
			_pathsGenerator = gameObject.AddComponent<PathsGenerator>();
			SubscribeToChannels();
		}

		// Below is testing code to check if rerouting works
		// public bool gotpath = false;
		// public bool rerouted = false;
		// Path testPath;
		// private void Update()
		// {
		// 	if (Time.time > 5 && !gotpath)
		// 	{
		// 		gotpath = true;
		// 		testPath = navigators[0].uav.currentPath;
		// 	}
		//
		// 	if (Time.time > 15 && !rerouted)
		// 	{
		// 		rerouted = true;
		// 		_UavReroutedEventChannel.RaiseEvent(navigators[0].uav, testPath);
		//
		// 	}
		// }

	

		private void OnUavDestroyed(Uav uav)
		{
			//destroy navigator and remove it from the dictionary
			if (navigators.ContainsKey(uav))
			{
				Destroy(navigators[uav]);
				navigators.Remove(uav);
			}
		}

		private void OnUavLost(Uav uav)
		{
			//destroy navigator and remove it from the dictionary
			if (navigators.ContainsKey(uav))
			{
				Destroy(navigators[uav]);
				navigators.Remove(uav);
			}
		}

		private void RerouteUav(Uav uav, Path path)
		{
			//check if navigators dictionary contains the uav
			if (navigators.ContainsKey(uav))
			{
				//if it does, reroute the uav
				navigators[uav].Reroute(path);
			}
		}

		

		public void GeneratePaths()
		{
			switch(_navigationSettings.navigationType)
			{
				
				case NavigationSettingsSO.NavigationType.BasedOnDefaultInputFile:
				{
					_uavPathsRecord= GameManager.Instance.jsonSerializerTest.rootObject.UavPathsRecords;
					navigators= _pathsGenerator.GeneratePaths(_uavPathsRecord);
					break;
				}
				case NavigationSettingsSO.NavigationType.BasedOnInputFile:
				{
					//throw new NotImplementedException();
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException();
				}
			}
		
		}

		public void NavigateAll()
		{
			foreach (var navigator in navigators.Values)
			{
				navigator.StartNavigation();
			}
		}
		
		private void GetReferencesFromGameManager()
		{
			_navigationSettings= GameManager.Instance.settingsDatabase.uavSettings.navigationSettings;
			_wayPointsManager= GameManager.Instance.wayPointsManager;
			_uavsManager= GameManager.Instance.uavsManager;
			uavReroutedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavReroutedEventChannel;
			uavShotDownChannel = GameManager.Instance.channelsDatabase.uavChannels.uavShotDownChannel;
			uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			uavLostEventChannel = GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavLostEventChannel;
		}
		private void OnDestroy()
		{
			UnsubscribeFromChannels();
		}
		private void SubscribeToChannels()
		{
			if (uavReroutedEventChannel != null)
			{
				uavReroutedEventChannel.Subscribe(RerouteUav);
			}
			if (uavShotDownChannel != null)
			{
				uavShotDownChannel.Subscribe(OnUavLost);
			}
			if (uavDestroyedEventChannel != null)
			{
				uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
			}
			if (uavLostEventChannel != null)
			{
				uavLostEventChannel.Subscribe(OnUavLost);
			}
		}
		private void UnsubscribeFromChannels()
		{
			if (uavReroutedEventChannel != null)
			{
				uavReroutedEventChannel.Unsubscribe(RerouteUav);
			}
			if (uavShotDownChannel != null)
			{
				uavShotDownChannel.Unsubscribe(OnUavLost);
			}
			if (uavDestroyedEventChannel != null)
			{
				uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
			}
			if(uavLostEventChannel!=null)
			{
				uavLostEventChannel.Unsubscribe(OnUavLost);
			}
		}
	}
}