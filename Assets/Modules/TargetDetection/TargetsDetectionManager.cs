using System.Collections.Generic;
using HelperScripts;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Settings.ScriptableObjects;
using Modules.TargetDetection.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums.UavCondition;

namespace Modules.TargetDetection
{
	public class TargetsDetectionManager:MonoBehaviour
	{
		private TargetDetectionSettingsSO _settings;
		private NavigationSettingsSO _navigationSettings;
		private UavPathEventChannelSO _uavStartedNewPathEventChannel;
		private UavEventChannelSO _uavCreatedEventChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		private Dictionary<Uav, List<GameObject>> _uavTargetsDictionary=new();
		
		private System.Random _targetPositioningRandomGenerator = new System.Random();
		private System.Random _targetDistanceRandomGenerator = new System.Random();

		
		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			
			_targetPositioningRandomGenerator = new System.Random(_settings.targetPositioningRandomGeneratorSeed);
			_targetDistanceRandomGenerator = new System.Random(_settings.targetDistanceRandomGeneratorSeed);
			
			var targetDetectionLogHandler = gameObject.GetOrAddComponent<TargetDetectionLogHandler>();
			targetDetectionLogHandler.Initialize();
		}

		private void GetReferencesFromGameManager()
		{
			_settings = GameManager.Instance.settingsDatabase.targetDetectionSettings;
			_navigationSettings = GameManager.Instance.settingsDatabase.navigationSettings;
			_uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavStartedNewPathEventChannel;
			_uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
		}
		
		private void OnUavDestroyed(Uav uav)
		{
			ClearTargets(uav);
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
			
			if (path.targetIsPresent )
				GenerateTarget(uav, path, true);
			
			if (path.nonTargetIsPresent)
				GenerateTarget(uav,path, false);
		}

		private void GenerateTarget(Uav uav, Path path, bool isTarget)
		{
			var timeToReachTarget = _settings.targetDetectionMaxResponseTime;
			var targetDistanceFromCurrentPosition = (float) timeToReachTarget * _navigationSettings.fixedSpeed;

			// Get the vector between the UAV and the destination waypoint
			var uavToWaypoint = path.destinationWayPoint.transform.position - uav.transform.position;
			// Project the random deviation vector onto the uavToWaypoint vector
			var randomDeviation = Vector3.ProjectOnPlane(
				new Vector3(
					(float) (_targetPositioningRandomGenerator.NextDouble() *
					         _settings.targetRandomDeviationFromCenter.Max),
					0,
					(float) (_targetPositioningRandomGenerator.NextDouble() *
					         _settings.targetRandomDeviationFromCenter.Max)),
				uavToWaypoint.normalized);

			var targetPosition = uav.transform.position + uavToWaypoint.normalized * targetDistanceFromCurrentPosition +
			                     randomDeviation;

			var target = Instantiate(GameManager.Instance.prefabsDatabase.targetBoxPrefab, targetPosition, Quaternion.identity);
			target.GetComponent<Target>().Initialize(path.destinationWayPoint.id, ObjectType.Box, targetPosition,
				uav.gameObject.layer, isTarget);

			_uavTargetsDictionary[uav].Add(target);
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
			if(_uavConditionChangedEventChannel != null)
					_uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);
		}

		private void UnsubscribeFromChannels()
		{
			if(_uavStartedNewPathEventChannel != null)
				_uavStartedNewPathEventChannel.Unsubscribe(UpdateTargets);
			if(_uavCreatedEventChannel != null)
				_uavCreatedEventChannel.Unsubscribe(OnUavCreated);
			if(_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
			if(_uavConditionChangedEventChannel != null)
				_uavConditionChangedEventChannel.Unsubscribe(OnUavConditionChanged);
		}

		private void OnUavConditionChanged(Uav uav, Enums.UavCondition condition)
		{
			if (condition is Lost or EnabledForReroutingOnly or Hidden)
			{
				ClearTargets(uav);
			}
		}
	}
}