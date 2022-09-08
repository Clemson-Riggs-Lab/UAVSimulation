using System;
using System.Collections.Generic;
using IOHandlers.Records;
using ScriptableObjects.UAVs.Navigation;
using UAVs.Navigation;
using UnityEngine;
using WayPoints;

namespace UAVs.Sub_Modules.Navigation
{
	public class NavigationManager: MonoBehaviour
	{

		[NonSerialized] public List<Navigator> navigators = new List<Navigator>();
		
		 private PathsGenerator _pathsGenerator ;
		 private WayPointsManager _wayPointsManager;
		 private UavsManager _uavsManager;
		 private NavigationSettingsSO _navigationSettings;
		 private List<UavPathsRecord>  _uavPathsRecord;
		 
		 private UavPathEventChannelSO _UavReroutedEventChannel;
		
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

		private void SubscribeToChannels()
		{
			if (_UavReroutedEventChannel != null)
			{
				_UavReroutedEventChannel.Subscribe(RerouteUav);
			}
		}

		private void RerouteUav(Uav uav, Path path)
		{
			//find the navigator of the uav
			Navigator navigator = navigators.Find(n => n.uav == uav);
			if (navigator != null)
			{
				navigator.Reroute(path);
			}
		}

		private void GetReferencesFromGameManager()
		{
			_navigationSettings= GameManager.Instance.navigationSettings;
			_wayPointsManager= GameManager.Instance.wayPointsManager;
			_uavsManager= GameManager.Instance.uavsManager;
			_UavReroutedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavReroutedEventChannel;
		}

		public void GeneratePaths()
		{
			switch(_navigationSettings.navigationType)
			{
				case NavigationSettingsSO.NavigationType.Sequential:
				{
					var numberOfSteps = _wayPointsManager.wayPoints.Count * _navigationSettings.numberOfLoops;
					navigators= _pathsGenerator.GenerateSequentialNavigationPaths( numberOfSteps );
					break;
				}
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
				case NavigationSettingsSO.NavigationType.Random:
				{
					//throw new NotImplementedException();
					break;
				}
				default:
				{
					throw new NotImplementedException();
				}
			}
		
		}

		public void NavigateAll()
		{
			foreach (var navigator in navigators)
			{
				navigator.StartNavigation();
			}
		}
		
	}
}