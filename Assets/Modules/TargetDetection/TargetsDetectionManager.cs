using System.Collections.Generic;
using HelperScripts;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.TargetDetection.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;

namespace Modules.TargetDetection
{
	public class TargetsDetectionManager:MonoBehaviour
	{
		private TargetDetectionSettingsSO _settings;
		private UavPathEventChannelSO _uavStartedNewPathEventChannel;
		private UavEventChannelSO _uavCreatedEventChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		private Dictionary<Uav, List<GameObject>> _uavTargetsDictionary=new();

		
		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();

			var targetDetectionLogHandler = gameObject.GetOrAddComponent<TargetDetectionLogHandler>();
			targetDetectionLogHandler.Initialize();
		}

		private void GetReferencesFromGameManager()
		{
			_settings = GameManager.Instance.settingsDatabase.targetDetectionSettings;
			_uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavStartedNewPathEventChannel;
			_uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
		}
		
		private void OnUavDestroyed(Uav uav)
		{
			_uavTargetsDictionary.Remove(uav);
		}

		private void OnUavCreated(Uav uav)
		{
			AddCameraToUav(uav);
			_uavTargetsDictionary.Add(uav, new List<GameObject>());
		}

		private void AddCameraToUav(Uav uav)
		{
			var cameraController =Instantiate(GameManager.Instance.prefabsDatabase.uavCameraPrefab, uav.uavBody.transform).GetComponent<UavCameraController>();
			cameraController.InitializeCamera(uav.gameObject.layer);
		}
		
		private void UpdateTargets(Uav uav, Path path)
		{
			ClearTargets(uav);
			
			if (path.targetIsPresent) 
			{
				var target= Instantiate(GameManager.Instance.prefabsDatabase.targetBoxPrefab,  path.destinationWayPoint.transform);
				target.GetComponent<Target>().Initialize(path.destinationWayPoint.id,ObjectType.Box,path.destinationWayPoint.transform.position,uav.gameObject.layer);
				_uavTargetsDictionary[uav].Add(target);
			}
			
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
			if(_uavStartedNewPathEventChannel != null)
				_uavStartedNewPathEventChannel.Subscribe(UpdateTargets);
			if(_uavCreatedEventChannel != null)
					_uavCreatedEventChannel.Subscribe(OnUavCreated);
			if(_uavDestroyedEventChannel != null)
					_uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
		}

		private void UnsubscribeFromChannels()
		{
			if(_uavStartedNewPathEventChannel != null)
				_uavStartedNewPathEventChannel.Unsubscribe(UpdateTargets);
			if(_uavCreatedEventChannel != null)
				_uavCreatedEventChannel.Unsubscribe(OnUavCreated);
			if(_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
		}
		
		
	}
}