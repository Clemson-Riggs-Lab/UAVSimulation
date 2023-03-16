using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IOHandlers;
using HelperScripts;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Settings.ScriptableObjects;
using Modules.NoFlyZone.Settings.ScriptableObjects;
using Modules.TargetDetection.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;
using WayPoints;
using static HelperScripts.Enums;
using static HelperScripts.Enums.InputRecordsSource;
using static HelperScripts.Enums.UavCondition;
using Random = System.Random;

namespace Modules.Navigation
{
	public class NavigationManager: MonoBehaviour
	{
		private UavsManager _uavsManager;
		private NavigationSettingsSO _navigationSettings;
		private NFZSettingsSO _nfzSettings;
		private TargetDetectionSettingsSO _targetDetectionSettings;
		private UavPathEventChannelSO _uavReroutedEventChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		
		private List<WayPoint> _waypoints = new List<WayPoint>();
		
		private  Random _shufflingPathsRandomNumberGenerator = new Random();
		private  Random _targetAndNFZProbabilityRandomNumberGenerator = new Random();
		private Random _uavSelectionShuffleRandomNumberGenerator=new Random();

		private  List<Uav> _enabledUavsForTargetDetection = new List<Uav>();
		private  List<Uav> _enabledUavsForRerouting = new List<Uav>();
		
		[NonSerialized] public Dictionary<Uav,Navigator> uavsNavigatorsDictionary = new ();
		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			_shufflingPathsRandomNumberGenerator = new Random(_navigationSettings.shufflingPathsRandomGeneratorSeed);
			_targetAndNFZProbabilityRandomNumberGenerator = new Random(_navigationSettings.targetAndNFZProbabilityRandomGeneratorSeed);
			_uavSelectionShuffleRandomNumberGenerator = new Random(_navigationSettings.uavSelectionShuffleRandomNumberGeneratorSeed);
			GenerateNavigators();
			SetEnabledUavs();

		}

		private void SetEnabledUavs()
		{
			var uavs = _uavsManager.uavs;
			
			//shuffle the uavs list
			uavs = uavs.OrderBy(x => _uavSelectionShuffleRandomNumberGenerator.Next()).ToList();
			
			for (var i = 0; i < uavs.Count; i++)
			{
				if (i < _targetDetectionSettings.numberOfActiveUavsForTargetDetection)
				{
					_enabledUavsForTargetDetection.Add(uavs[i]);
				}
				if (i < _navigationSettings.numberOfActiveUavsForRerouting)
				{
					_enabledUavsForRerouting.Add(uavs[i]);
				}
				
			}
		}


		private void OnUavDestroyed(Uav uav)
		{
			
				if(_enabledUavsForTargetDetection.Contains(uav))
					_enabledUavsForTargetDetection.Remove(uav);
				
				if(_enabledUavsForRerouting.Contains(uav))
					_enabledUavsForRerouting.Remove(uav);
				
				//destroy navigator and remove it from the dictionary
				if (uavsNavigatorsDictionary.ContainsKey(uav))
				{
					Destroy(uavsNavigatorsDictionary[uav]);
					uavsNavigatorsDictionary.Remove(uav);
				}
		}

		private void OnUavConditionChanged(Uav uav, UavCondition uavCondition)
		{
			//destroy navigator and remove it from the dictionary if the uav is lost 
			if (uavCondition is not Lost ) return;
			if (uavsNavigatorsDictionary.ContainsKey(uav))
			{
				Destroy(uavsNavigatorsDictionary[uav]);
				uavsNavigatorsDictionary.Remove(uav);
				
				if(_enabledUavsForTargetDetection.Contains(uav))
					_enabledUavsForTargetDetection.Remove(uav);
				
				if(_enabledUavsForRerouting.Contains(uav))
					_enabledUavsForRerouting.Remove(uav);
			}
		}

		private void RerouteUav(Uav uav, Path path)
		{
			//check if uavsNavigatorsDictionary dictionary contains the uav
			if (uavsNavigatorsDictionary.ContainsKey(uav)) 
				uavsNavigatorsDictionary[uav].Reroute(path); //if it does, reroute the uav
		}

		public Path GetRandomPathDynamically(Uav uav)
		{
			UpdateEnabledUavsLists();
			UpdateUavVisualSettings(uav, out var visuallyEnabledForTargetDetection, out var visuallyEnabledForRerouting, out var currentPathDestination);
			
			var hasTarget = visuallyEnabledForTargetDetection && _targetAndNFZProbabilityRandomNumberGenerator.NextDouble() < _targetDetectionSettings.ratioOfActiveFeedsWithTarget;
			var hasNonTarget = visuallyEnabledForTargetDetection && _targetAndNFZProbabilityRandomNumberGenerator.NextDouble() < _targetDetectionSettings.ratioOfActiveFeedsWithNonTarget;
			var pathHasNFZ = visuallyEnabledForRerouting && _targetAndNFZProbabilityRandomNumberGenerator.NextDouble() < _nfzSettings.RatioOfHeadToNFZ;


			var validWaypoints = new List<WayPoint>(_waypoints);
			

			validWaypoints.Remove(currentPathDestination);

		
			var fallbackWaypointCandidate = validWaypoints.FirstOrDefault();

			validWaypoints = validWaypoints.Where(waypoint =>
			{
				if (_navigationSettings.maxPathDuration > 0)
				{
					var distance = Vector3.Distance(currentPathDestination.transform.position,
						waypoint.transform.position);
					var duration = distance / _navigationSettings.fixedSpeed;
					if (duration > _navigationSettings.maxPathDuration ||
					    duration < _navigationSettings.minPathDuration)
					{
						return false;
					}
				}


				var nfzHit = Physics.Linecast(uav.uavBody.transform.position, waypoint.transform.position, out var hit, 1 << LayerMask.NameToLayer("NFZ"));
				if (nfzHit == pathHasNFZ)
				{
					// if distance between uav and hit point is less than the minimum indicated in the settings , return false
					return !(Vector3.Distance(uav.uavBody.transform.position, hit.point) <
					         (_navigationSettings.minDistanceFromNFZInDuration * _navigationSettings.fixedSpeed));
					//if the route satisfies the headingToNFZ condition(i.e.,uav should head to NFZ and the path contains a NFZ) , return true, otherwise return false
				}
				else
				{
					return false;
				}
			}).ToList();

			if (validWaypoints.Count == 0)
			{
				Debug.LogWarning($"No waypoints found for UAV {uav.name} because there are no waypoints that satisfy the conditions");
				validWaypoints.Add(fallbackWaypointCandidate);
				pathHasNFZ =  Physics.Linecast(uav.transform.position, fallbackWaypointCandidate.transform.position, 1 << LayerMask.NameToLayer("NFZ"));
			}
			validWaypoints.Shuffle(_shufflingPathsRandomNumberGenerator);
			var newDestinationWaypointCandidate = validWaypoints.FirstOrDefault();
			return new Path(uav, newDestinationWaypointCandidate, visuallyEnabledForTargetDetection, visuallyEnabledForRerouting  ,hasTarget, hasNonTarget, pathHasNFZ);
		}

		private void UpdateEnabledUavsLists()
		{
			if(_navigationSettings.numberOfActiveUavsForRerouting > _enabledUavsForRerouting.Count)
			{
				var uavsToEnable = _navigationSettings.numberOfActiveUavsForRerouting - _enabledUavsForRerouting.Count;
				var uavs = uavsNavigatorsDictionary.Keys.ToList();
				//remove uavs that are already enabled
				uavs.RemoveAll(uav => _enabledUavsForRerouting.Contains(uav));
    
				// shuffle the remaining uavs
				uavs.Shuffle(_uavSelectionShuffleRandomNumberGenerator);
    
				// find the enabled uavs for target detection and put them first in the list
				var enabledUavsForTargetDetection = _enabledUavsForTargetDetection.Intersect(uavs).ToList();
				uavs.RemoveAll(uav => _enabledUavsForTargetDetection.Contains(uav));
				uavs.InsertRange(0, enabledUavsForTargetDetection);

				// take the first uavsToEnable from the shuffled and sorted list of uavs
				_enabledUavsForRerouting.AddRange(uavs.Take(uavsToEnable));
			}
			
			else if (_navigationSettings.numberOfActiveUavsForRerouting < _enabledUavsForRerouting.Count)
			{
				var uavsToDisable = _enabledUavsForRerouting.Count - _navigationSettings.numberOfActiveUavsForRerouting;
				_enabledUavsForRerouting.RemoveRange(0, uavsToDisable);
			}

			if (_targetDetectionSettings.numberOfActiveUavsForTargetDetection > _enabledUavsForTargetDetection.Count)
			{
				var uavsToEnable = _targetDetectionSettings.numberOfActiveUavsForTargetDetection - _enabledUavsForTargetDetection.Count;
				var uavs = uavsNavigatorsDictionary.Keys.ToList();
				//remove uavs that are already enabled
				uavs.RemoveAll(uav => _enabledUavsForTargetDetection.Contains(uav));
	
				// shuffle the remaining uavs
				uavs.Shuffle(_uavSelectionShuffleRandomNumberGenerator);
	
				// find the enabled uavs for rerouting and put them first in the list
				var enabledUavsForRerouting = _enabledUavsForRerouting.Intersect(uavs).ToList();
				uavs.RemoveAll(uav => _enabledUavsForRerouting.Contains(uav));
				uavs.InsertRange(0, enabledUavsForRerouting);

				// take the first uavsToEnable from the shuffled and sorted list of uavs
				_enabledUavsForTargetDetection.AddRange(uavs.Take(uavsToEnable));
			}
			
			else if (_targetDetectionSettings.numberOfActiveUavsForTargetDetection < _enabledUavsForTargetDetection.Count)
			{
				var uavsToDisable = _enabledUavsForTargetDetection.Count - _targetDetectionSettings.numberOfActiveUavsForTargetDetection;
				_enabledUavsForTargetDetection.RemoveRange(0, uavsToDisable);
			}
			
		}

		private void UpdateUavVisualSettings(Uav uav, out bool enabledForTargetDetection, out bool enabledForRerouting, out WayPoint currentDestination)
		{
			enabledForTargetDetection = false;
			enabledForRerouting = false;
			currentDestination = null;
			
			currentDestination = uav.currentPath == null ? uav.startingWaypoint : uav.currentPath.destinationWayPoint;
			
			
			if(_enabledUavsForTargetDetection.Contains(uav))
				enabledForTargetDetection = true;
			
			if(_enabledUavsForRerouting.Contains(uav))
				enabledForRerouting = true;
		}


		public List<Navigator> GenerateNavigators(List<Uav> uavs= null)
		{
			if (uavs == null)
			{
				uavs = _uavsManager.uavs;
			}
		
			var navigators = new List<Navigator>();
			foreach (var uav in uavs)
			{
				if (!uav.gameObject.TryGetComponent(out Navigator navigator))
					navigator = uav.gameObject.AddComponent<Navigator>();
				
				uavsNavigatorsDictionary[uav] = navigator;
				navigators.Add(navigator);
			}
			return navigators;
		}

		public IEnumerator NavigateAll(float simulationStartTime)
		{
			yield return new WaitForSeconds(simulationStartTime-Time.time);
			foreach (var navigator in uavsNavigatorsDictionary.Values)
			{
				navigator.StartNavigation();
			}
		}
		
		private void GetReferencesFromGameManager()
		{
			_uavsManager = GameManager.Instance.uavsManager;
			_navigationSettings= GameManager.Instance.settingsDatabase.navigationSettings;
			_targetDetectionSettings = GameManager.Instance.settingsDatabase.targetDetectionSettings;
			_nfzSettings = GameManager.Instance.settingsDatabase.nfzSettings;
			_uavReroutedEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutedEventChannel;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
			_waypoints = GameManager.Instance.wayPointsManager.wayPoints;
		}
		private void OnDestroy() => UnsubscribeFromChannels();

		private void SubscribeToChannels()
		{
			if (_uavReroutedEventChannel != null) _uavReroutedEventChannel.Subscribe(RerouteUav);
			if (_uavDestroyedEventChannel != null) _uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
			if (_uavConditionChangedEventChannel != null) _uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);
		}
		private void UnsubscribeFromChannels()
		{
			if (_uavReroutedEventChannel != null) _uavReroutedEventChannel.Unsubscribe(RerouteUav);
			if (_uavDestroyedEventChannel != null) _uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
			if (_uavConditionChangedEventChannel != null) _uavConditionChangedEventChannel.Unsubscribe(OnUavConditionChanged);

		}


		public IEnumerator InitializeUavs(List<Uav> uavsToGenerate)
		{
			var navigators= GenerateNavigators(uavsToGenerate);
			
			yield return new WaitForSeconds(0.1f);
			foreach (var navigator in navigators)
			{
				navigator.StartNavigation();
			}
		}
	}
}