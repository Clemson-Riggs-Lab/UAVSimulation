using System.Collections.Generic;
using ScriptableObjects.EventChannels;
using ScriptableObjects.TargetDetection;
using ScriptableObjects.UAVs.Navigation;
using UAVs;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;
using WayPoints;

namespace TargetDetection
{
	public class TargetsManager:MonoBehaviour
	{
		public static TargetsManager Instance { get; private set; }
		private TargetDetectionSettingsSO _settings;
		private UavPathEventChannelSO uavStartedNewPathEventChannel;
		private UavEventChannelSO uavCreatedEventChannel;
		private UavEventChannelSO uavDestroyedEventChannel;
		private UavTargetsPairEventChannelSO uavTargetsAddedEventChannel;
		private Dictionary<Uav, List<GameObject>> _uavTargetsDictionary=new();

		
		private void Start()
		{
			if (Instance != null && Instance != this) 
			{ 
				Destroy(this); 
			} 
			else 
			{ 
				Instance = this; 
			} 
			
			InitializeFromGameManager();
			SubscribeToChannels();
		}

		private void InitializeFromGameManager()
		{
			_settings = GameManager.Instance.settingsDatabase.targetDetectionSettings;
			uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavStartedNewPathEventChannel;
			uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
		}
		
		private void UpdateTargets(Uav uav, Path path)
		{
			ClearTargets(uav);
			
			if (path.TargetIsPresent) 
			{
				var target = GenerateTargetAtWaypoint(path.DestinationWayPoint);
				target.layer = uav.gameObject.layer;
				_uavTargetsDictionary[uav].Add(target);
			}
			
		}
		private GameObject GenerateTargetAtWaypoint(WayPoint waypoint)
		{
			var target= Instantiate(GameManager.Instance.prefabsDatabase.targetBoxPrefab,  waypoint.transform);
			target.GetComponent<Target>().Initialize(waypoint.Id,ObjectType.Box,waypoint.transform.position);
			return target;
		}

		private void ClearTargets(Uav uav)
		{
			if (_uavTargetsDictionary.ContainsKey(uav))
			{ 
				foreach (var target in _uavTargetsDictionary[uav])
				{
					Destroy(target.gameObject);
				}
				_uavTargetsDictionary[uav].Clear();
				
				
			}
		}


		private void OnDisable()
		{
			UnsubscribeFromChannels();
		}
		
		private void SubscribeToChannels()
		{
			if(uavStartedNewPathEventChannel != null)
				uavStartedNewPathEventChannel.Subscribe(UpdateTargets);
			if(uavCreatedEventChannel != null)
					uavCreatedEventChannel.Subscribe(OnUavCreated);
			if(uavDestroyedEventChannel != null)
					uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
		}

		private void UnsubscribeFromChannels()
		{
			if(uavStartedNewPathEventChannel != null)
				uavStartedNewPathEventChannel.Unsubscribe(UpdateTargets);
			if(uavCreatedEventChannel != null)
				uavCreatedEventChannel.Unsubscribe(OnUavCreated);
			if(uavDestroyedEventChannel != null)
				uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
		}
		
		private void OnUavDestroyed(Uav uav)
		{
			_uavTargetsDictionary.Remove(uav);
		}

		private void OnUavCreated(Uav uav)
		{
			_uavTargetsDictionary.Add(uav, new List<GameObject>());
		}
	}
}