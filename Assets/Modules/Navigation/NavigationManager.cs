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
		
		private int _numberOfTargetsInQueue=0;
		private int _numberOfNonTargetsInQueue=0;
		private int _numberOfNFZsInQueue=0;
		
		[NonSerialized] public Dictionary<Uav,Navigator> uavsNavigatorsDictionary = new ();
		private List<WayPoint> _occupiedWaypointsList = new ();

		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			_shufflingPathsRandomNumberGenerator = new Random(_navigationSettings.shufflingPathsRandomGeneratorSeed);
			_targetAndNFZProbabilityRandomNumberGenerator = new Random(_navigationSettings.targetAndNFZProbabilityRandomGeneratorSeed);
			_uavSelectionShuffleRandomNumberGenerator = new Random(_navigationSettings.uavSelectionShuffleRandomNumberGeneratorSeed);
			GenerateNavigators();

		}

		
		private void OnUavDestroyed(Uav uav)
		{
			if (uav.currentPath != null&& uav.currentPath.destinationWayPoint != null)
			{
				_occupiedWaypointsList.Remove(uav.currentPath.destinationWayPoint);
			}

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
			
			if (uav.currentPath != null&& uav.currentPath.destinationWayPoint != null)
			{
				_occupiedWaypointsList.Remove(uav.currentPath.destinationWayPoint);
			}
			
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
			
			if(uav.currentPath!=null && uav.currentPath.destinationWayPoint!=null)
				_occupiedWaypointsList.Remove(uav.currentPath.destinationWayPoint);
			
			_occupiedWaypointsList.Add(path.destinationWayPoint);
			
			//check if uavsNavigatorsDictionary dictionary contains the uav
			if (uavsNavigatorsDictionary.ContainsKey(uav)) 
				uavsNavigatorsDictionary[uav].Reroute(path); //if it does, reroute the uav
		}
/// <summary>
///  
/// </summary>
/// Regarding the target, non-target and NFZ queues:
/// if one condition is active, we dont want the other conditions to be active
/// but at the same time we want to maintain the ratio of the conditions as provided in the input file.
/// to get over this issue, we have queues where if more than one condition is active, we put the other condition in the queue
/// non targets are the least priority, while NFZs and targets are of equal priority.
/// <param name="uav"></param>
/// <returns></returns>
		public Path GetRandomPathDynamically(Uav uav)
		{
			UpdateEnabledUavsLists();
			UpdateUavVisualSettings(uav, out var visuallyEnabledForTargetDetection, out var visuallyEnabledForRerouting, out var currentPathDestination);
			bool hasTarget= false;
			bool hasNonTarget= false;
			bool pathHasNFZ= false;
			
			if (visuallyEnabledForTargetDetection)
			{
				if(_targetAndNFZProbabilityRandomNumberGenerator.NextDouble() < _targetDetectionSettings.ratioOfActiveFeedsWithTarget)
					_numberOfTargetsInQueue++;
				if(_targetAndNFZProbabilityRandomNumberGenerator.NextDouble() < _targetDetectionSettings.ratioOfActiveFeedsWithNonTarget)
					_numberOfNonTargetsInQueue++;
							
				if (_numberOfTargetsInQueue > 0)
				{
					hasTarget = true;
					_numberOfTargetsInQueue--;
				}
				else if (_numberOfNonTargetsInQueue > 0)//if there are no targets in the queue, we check for non targets, non targets are the least priority
				{
					hasNonTarget = true;
					_numberOfNonTargetsInQueue--;
				}
			
			}

			if (visuallyEnabledForRerouting)
			{
				if( _targetAndNFZProbabilityRandomNumberGenerator.NextDouble() < _nfzSettings.RatioOfHeadToNFZ)
					_numberOfNFZsInQueue++;

				if (_numberOfNFZsInQueue > 0)
				{
					pathHasNFZ = true;
					_numberOfNFZsInQueue--;
				}
			}
			
			if (pathHasNFZ && hasTarget)//we dont want both conditions to be active at the same time (designed this way, although it can be changed)
			{
				if (_numberOfNFZsInQueue >= _numberOfTargetsInQueue) //checking which queue is bigger, so that we dont keep on getting one queue big only.
				{
					hasTarget = false;
					_numberOfTargetsInQueue++;
				}
				else
				{
					pathHasNFZ = false;
					_numberOfNFZsInQueue++;
				}
			}
			else if (pathHasNFZ && hasNonTarget)
			{
				hasNonTarget = false;
				_numberOfNonTargetsInQueue++;
			}
			
			
			var validWaypoints = new List<WayPoint>(_waypoints);
			_occupiedWaypointsList.Remove(currentPathDestination);
			validWaypoints.Remove(currentPathDestination);
			validWaypoints.RemoveAll(item=> _occupiedWaypointsList.Contains(item));
			

			validWaypoints = validWaypoints.Where(waypoint =>
			{
				if (!(_navigationSettings.maxPathDuration > 0) && !(_navigationSettings.minPathDuration > 0))
					return true; //if the duration range is not set, return true
				var distance = Vector3.Distance(currentPathDestination.transform.position,
					waypoint.transform.position);
				var duration = distance / _navigationSettings.fixedSpeed;
				return duration < _navigationSettings.maxPathDuration && duration > _navigationSettings.minPathDuration; //if the duration is  within range, return true //if the duration is not within range, return false
			}).ToList();
			
			
			validWaypoints.Shuffle(_shufflingPathsRandomNumberGenerator);
			var fallbackWaypointCandidate = validWaypoints.FirstOrDefault();

			validWaypoints = validWaypoints.Where(waypoint =>
			{
				var nfzHit = Physics.Linecast(uav.uavBody.transform.position, waypoint.transform.position, out var hit, 1 << LayerMask.NameToLayer("NFZ"));
				
				if (nfzHit != pathHasNFZ) return false; //if we do not have a nfz and we need to hit one or vice versa, return false
				
				if(!pathHasNFZ) return true; //if we do not have a nfz, return true as there is no need to check the distance
				
				var distanceToNfz = Vector3.Distance(uav.uavBody.transform.position, hit.point);
				var minDistanceFromNFZ = _navigationSettings.minDistanceFromNFZInDuration * _navigationSettings.fixedSpeed;
				var maxDistanceFromNFZ = _navigationSettings.maxDistanceFromNFZInDuration* _navigationSettings.fixedSpeed;
				var isWithinRange = distanceToNfz >= minDistanceFromNFZ && distanceToNfz <= maxDistanceFromNFZ;

				// Return true if the distance is within the range, otherwise return false
				return isWithinRange;

			}).ToList();

			if (validWaypoints.Count == 0) //if there are no valid waypoints, use the fallback waypoint
			{
				//adjust the queue because we are using the fallback waypoint which *might* not have the same conditions as the required path
				 if (pathHasNFZ)
					 _numberOfNFZsInQueue++;
				
				validWaypoints.Add(fallbackWaypointCandidate);
				pathHasNFZ =  Physics.Linecast(uav.transform.position, fallbackWaypointCandidate.transform.position, 1 << LayerMask.NameToLayer("NFZ"));
				
				//adjust the queue again to reflect the actual conditions of the path
				if (pathHasNFZ)
					_numberOfNFZsInQueue--;
				
				
			}
			
			var newDestinationWaypointCandidate = validWaypoints.FirstOrDefault();
			_occupiedWaypointsList.Add(newDestinationWaypointCandidate);
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
				if(!_navigationSettings.distinctUavsForReroutingAndTargetDetection) 
					uavs.InsertRange(0, enabledUavsForTargetDetection);
				
				uavs= SortByClosestToDestination(uavs); //sort the list by the closest uav to the destination so that we get a uav that is qucikly enabled
				// take the first uavsToEnable from the shuffled and sorted list of uavs
				_enabledUavsForRerouting.AddRange(uavs.Take(Math.Min(uavsToEnable, uavs.Count)));
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
	
				// find the enabled uavs for rerouting and put them first in the list or remove them if we want distinct uavs for rerouting and target detection
				var enabledUavsForRerouting = _enabledUavsForRerouting.Intersect(uavs).ToList();
				uavs.RemoveAll(uav => _enabledUavsForRerouting.Contains(uav));
				if(!_navigationSettings.distinctUavsForReroutingAndTargetDetection) 
					uavs.InsertRange(0, enabledUavsForRerouting);
				// take the first uavsToEnable from the shuffled and sorted list of uavs
				uavs= SortByClosestToDestination(uavs);
				_enabledUavsForTargetDetection.AddRange(uavs.Take(Math.Min(uavsToEnable, uavs.Count)));
			}
			
			else if (_targetDetectionSettings.numberOfActiveUavsForTargetDetection < _enabledUavsForTargetDetection.Count)
			{
				var uavsToDisable = _enabledUavsForTargetDetection.Count - _targetDetectionSettings.numberOfActiveUavsForTargetDetection;
				_enabledUavsForTargetDetection.RemoveRange(0, uavsToDisable);
			}
			
		}

		private List<Uav> SortByClosestToDestination(List<Uav> uavs)
		{
			//sort them by which uav is closest to uav.currentPath.destinationWayPoint
			var sortedUavs = uavs.OrderBy(uav =>
			{
				if (uav == null || uav.transform == null || uav.currentPath == null || uav.currentPath.destinationWayPoint == null || uav.currentPath.destinationWayPoint.transform == null)
				{
					return float.MaxValue; // Assign the maximum float value if any object is null
				}
				var distance = Vector3.Distance(uav.transform.position, uav.currentPath.destinationWayPoint.transform.position);
				return distance;
			}).ToList();
			return sortedUavs;
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