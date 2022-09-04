using System;
using System.Collections.Generic;
using TargetDetection.ScriptableObjects;
using UAVs;
using UAVs.Sub_Modules.Navigation;
using UAVs.Sub_Modules.Navigation.ScriptableObjects;
using UnityEditor;
using UnityEngine;
using WayPoints;

namespace TargetDetection
{
	public class VisualObjectsManager:MonoBehaviour
	{
		private TargetDetectionSettingsSO _settings;
		private UavPathEventChannelSO uavStartedNewPathEventChannel;
		private UavVisualObjectsEventChannelSO newTargetAddedEventEventChannel;
		private List<Uav> _uavs;
		private List<WayPoint> _wayPoints;
		private Dictionary<Uav, List<GameObject>> _uavVisualObjectsDictionary;

		
		private void Start()
		{
			InitializeFromGameManager();
			SubscribeToChannels();
		}

		private void InitializeFromGameManager()
		{
			_uavs= GameManager.Instance.uavsManager.uavs;
			_wayPoints = GameManager.Instance.wayPointsManager.wayPoints;
			_settings = GameManager.Instance.settingsDatabase.targetDetectionSettings;
		}
		
		private void UpdateVisualObjects(Uav uav, Path path)
		{
			ClearVisualObjects(uav);
			
			if (!path.TargetIsPresent) return; // no targets to add
			else
			{
				var target = GenerateTargetAtWaypoint(path.DestinationWayPoint);
				AddTargetToDictionary(uav, target);
			}
			
		}

		private void AddTargetToDictionary(Uav uav, GameObject target)
		{
			if (!_uavVisualObjectsDictionary.ContainsKey(uav))
			{
				_uavVisualObjectsDictionary.Add(uav, new List<GameObject>());
			}
			_uavVisualObjectsDictionary[uav].Add(target);
		}

		private GameObject GenerateTargetAtWaypoint(WayPoint waypoint)
		{
			var target= Instantiate(GameManager.Instance.prefabsDatabase.targetBoxPrefab,  waypoint.transform);
			target.GetComponent<VisualObject>().Initialize(waypoint.Id,true,ObjectType.TargetBox,waypoint.transform.position);
			return target;
		}

		private void ClearVisualObjects(Uav uav)
		{
			foreach (var visualObject in _uavVisualObjectsDictionary[uav])
			{
				_uavVisualObjectsDictionary[uav].Remove(visualObject);
				Destroy(visualObject.gameObject);
			}
		}


		private void OnDisable()
		{
			UnsubscribeFromChannels();
			
		}
		
		private void SubscribeToChannels()
		{
			if(uavStartedNewPathEventChannel != null)
				uavStartedNewPathEventChannel.Subscribe(UpdateVisualObjects);// subscribing to get each wayPoint that is created 
		}

		

		private void UnsubscribeFromChannels()
		{
			if(uavStartedNewPathEventChannel != null)
				uavStartedNewPathEventChannel.Unsubscribe(UpdateVisualObjects);// subscribing to get each wayPoint that is created 
		}
	}
}