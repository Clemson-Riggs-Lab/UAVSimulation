using System;
using Modules.Logging;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.TargetDetection.Settings.ScriptableObjects;
using UAVs;
using UnityEngine;
using UnityEngine.Events;

namespace Modules.TargetDetection
{
	public class TargetDetectionLogHandler:MonoBehaviour
	{
		private UavPathEventChannelSO targetDetectedEventChannel;
		private UavPathEventChannelSO targetNotDetectedEventChannel;
		private LogEventChannelSO _logEventChannel;
		private TargetDetectionSettingsSO _targetDetectionSettings;
		
		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
		}

		

		private void GetReferencesFromGameManager()
		{
			targetDetectedEventChannel = GameManager.Instance.channelsDatabase.targetDetectionChannels.targetDetectedEventChannel;
			targetNotDetectedEventChannel = GameManager.Instance.channelsDatabase.targetDetectionChannels.targetNotDetectedEventChannel;
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			_targetDetectionSettings = GameManager.Instance.settingsDatabase.targetDetectionSettings;
		}
		
		private void SubscribeToChannels()
		{
			if(!_targetDetectionSettings.logTargetDetection) return;
			
			if(targetDetectedEventChannel != null)
				targetDetectedEventChannel.Subscribe(OnTargetDetected);
			
			if(targetNotDetectedEventChannel != null)
				targetNotDetectedEventChannel.Subscribe(OnTargetNotDetected);
		}

		private void OnTargetDetected(Uav uav, Path path)
		{
			AddTargetDetectionLog(uav, path, true);
		}

		private void OnTargetNotDetected(Uav uav, Path path)
		{
			AddTargetDetectionLog(uav, path, false);
		}

		private void AddTargetDetectionLog(Uav uav, Path path, bool isDetected)
		{
			
			var log = new Log();
			log.logType = "TargetDetection";
			log.eventType = isDetected ? "TargetDetected" : "TargetNotDetected";
			log.logMessages = new() { isDetected ? "Target detected" : "Target not detected" + " by " + uav.name + " on path " + path.id };
			
			if(_targetDetectionSettings.logTargetDetectionCorrectness)
				log.logMessages.Add($"Responded correctly: {isDetected == path.targetIsPresent}");
			
			if(_targetDetectionSettings.logTimeSinceStartOfPathWhenTargetDetectionOccured)
				log.logMessages.Add($"Time since start of path: {DateTime.Now- path.startTime}");
			
			_logEventChannel.RaiseEvent(log);
		}

		private void OnDisable()
		{
			UnsubscribeFromChannels();
		}

		private void UnsubscribeFromChannels()
		{
			if(targetDetectedEventChannel != null)
				targetDetectedEventChannel.Unsubscribe(OnTargetDetected);
			
			if(targetNotDetectedEventChannel != null)
				targetNotDetectedEventChannel.Unsubscribe(OnTargetNotDetected);
		}
	}
	
}