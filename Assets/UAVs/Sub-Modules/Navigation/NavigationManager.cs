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
		
		private void Start()
		{
			_pathsGenerator = gameObject.AddComponent<PathsGenerator>();
			
			_navigationSettings= GameManager.Instance.navigationSettings;
			_wayPointsManager= GameManager.Instance.wayPointsManager;
			_uavsManager= GameManager.Instance.uavsManager;
			
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