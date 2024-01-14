using Modules.Logging;
using Modules.Logging.Channels.ScriptableObjects;
using ScriptableObjects.EventChannels;
using UAVs.Channels.ScriptableObjects;
using UAVs.Settings.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums;

namespace UAVs
{
	public class UavLogHandler:MonoBehaviour
	{
		private UavEventChannelSO _uavCreatedEventChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		private LogEventChannelSO _logEventChannel;
		private UavSettingsSO _uavSettings;
		
		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
		}

		private void GetReferencesFromGameManager()
		{
			_uavSettings = GameManager.Instance.settingsDatabase.uavSettings;
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			_uavCreatedEventChannel=GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			_uavDestroyedEventChannel=GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavConditionChangedEventChannel=GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
		}

		private void SubscribeToChannels()
		{
			if (_uavCreatedEventChannel != null && _uavSettings.logUavCreationEvents)
			{
				_uavCreatedEventChannel.Subscribe(OnUavCreated);
			}
			if (_uavDestroyedEventChannel != null && _uavSettings.logUavDestroyEvents)
			{
				_uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
			}
			if (_uavConditionChangedEventChannel != null && _uavSettings.logUavConditionChangedEvents)
			{
				_uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);
			}
		
		}

		private void OnUavConditionChanged(Uav uav, UavCondition condition)
		{
			var log = new Log
			{
				logType = "Uav",
				eventType = "ConditionChanged",
				logData = new { message=$"Uav {uav.uavName} condition changed to {condition}" }
			};
			_logEventChannel.RaiseEvent(log);
		}

		private void OnUavDestroyed(Uav uav)
		{
			var log = new Log
			{
				logType = "Uav",
				eventType = "Destroyed",
				logData = new { message= $"Uav {uav.uavName} destroyed" }
			};
			_logEventChannel.RaiseEvent(log);
		}

		private void OnUavCreated(Uav uav)
		{
			var log = new Log
			{
				logType = "Uav",
				eventType = "Created",
				logData = new { message= $"Uav {uav.uavName} created" }
			};
			_logEventChannel.RaiseEvent(log);
		}

		private void OnDisable()
		{
			UnsubscribeFromChannels();
		}

		private void UnsubscribeFromChannels()
		{
			if (_uavCreatedEventChannel != null)
			{
				_uavCreatedEventChannel.Unsubscribe(OnUavCreated);
			}
			if (_uavDestroyedEventChannel != null)
			{
				_uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
			}
			if (_uavConditionChangedEventChannel != null)
			{
				_uavConditionChangedEventChannel.Unsubscribe(OnUavConditionChanged);
			}
		}
	}
}