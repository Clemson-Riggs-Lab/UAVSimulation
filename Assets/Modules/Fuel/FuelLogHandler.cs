using System;
using System.Collections.Generic;
using Modules.FuelAndHealth.Channels.ScriptableObjects;
using Modules.FuelAndHealth.Settings.ScriptableObjects;
using Modules.Logging;
using Modules.Logging.Channels.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;
using static Modules.FuelAndHealth.Settings.ScriptableObjects.FuelSettingsSO;

namespace Modules.Fuel
{
	public class FuelLogHandler:MonoBehaviour
	{
		private UavEventChannelSO _uavFuelLeakFixedEventChannel;
		private UavFuelConditionEventChannelSO _uavFuelConditionChangedEventChannel;
		private FuelSettingsSO _fuelSettings;
		private LogEventChannelSO _logEventChannel;

		
		public void Initialize()
		{
			 GetReferencesFromGameManager();
			 SubscribeToChannels();
		}

		private void GetReferencesFromGameManager()
		{
			_uavFuelLeakFixedEventChannel = GameManager.Instance.channelsDatabase.fuelChannels.uavFuelLeakFixedEventChannel;
			_uavFuelConditionChangedEventChannel = GameManager.Instance.channelsDatabase.fuelChannels.uavFuelConditionChangedEventChannel;
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			_fuelSettings = GameManager.Instance.settingsDatabase.fuelSettings;
			
		}
		
		private void SubscribeToChannels()
		{
			if (_uavFuelLeakFixedEventChannel != null&& _fuelSettings.logFuelLeakFixEvents)
				_uavFuelLeakFixedEventChannel.Subscribe(OnUavFuelLeakFixed);

			if (_uavFuelConditionChangedEventChannel != null&& (_fuelSettings.logFuelEmptyEvents || _fuelSettings.logFuelLeakEvents || _fuelSettings.logFatalFuelLeakEvents))
				_uavFuelConditionChangedEventChannel.Subscribe(OnUavFuelConditionChanged);
		}
		

		private void OnUavFuelLeakFixed(Uav uav)
		{
			var log = new Log
			{
				logType = "Fuel",
				eventType = "Fuel Leak Fixed",
				logData = new { message= $"UAV {uav.uavName} fuel leak fixed" }
			};
			_logEventChannel.RaiseEvent(log);
		}
		

		private void OnUavFuelConditionChanged(Uav uav, FuelCondition condition)
		{
			var logDataDict = new Dictionary<string, object>();

			var log = new Log
			{
				logType = "Fuel",
				eventType = "Fuel Condition Changed",
				logData = logDataDict
			};

			switch (condition)
			{
				case FuelCondition.Empty when _fuelSettings.logFuelEmptyEvents:
					logDataDict.Add("message", $"UAV {uav.uavName} fuel became empty and it stopped navigating");
					break;
				case FuelCondition.Leaking when _fuelSettings.logFuelLeakEvents:
					logDataDict.Add("message", $"UAV {uav.uavName} fuel leak event started");
					break;
				case FuelCondition.FatalLeak when _fuelSettings.logFatalFuelLeakEvents:
					logDataDict.Add("message", $"UAV {uav.uavName} fuel leak became fatal ");
					break;
				case FuelCondition.Normal:
				default:
					return;
			}

			if (logDataDict.Count > 0)
			{
				_logEventChannel.RaiseEvent(log);
			}
		}


		private void OnDisable()
		{
			UnsubscribeFromChannels();
		}

		private void UnsubscribeFromChannels()
		{
			if (_uavFuelLeakFixedEventChannel != null)
				_uavFuelLeakFixedEventChannel.Unsubscribe(OnUavFuelLeakFixed);

			if (_uavFuelConditionChangedEventChannel != null)
				_uavFuelConditionChangedEventChannel.Unsubscribe(OnUavFuelConditionChanged);
			
		}
	}
}