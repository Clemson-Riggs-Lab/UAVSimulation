using System;
using Modules.Logging;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.NoFlyZone.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;

namespace Modules.NoFlyZone.Settings
{
	public class NFZLogHandler:MonoBehaviour
	{
		private UavEventChannelSO _uavLostEventChannel;
		private LogEventChannelSO _logEventChannel;
		private NFZSettingsSO _nfzSettings;


		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
		}

	

		private void GetReferencesFromGameManager()
		{
			_nfzSettings = GameManager.Instance.settingsDatabase.nfzSettings;
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			_uavLostEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavLostEventChannel;
			
		}

		private void SubscribeToChannels()
		{
			if (_nfzSettings.logNFZCollisions && _uavLostEventChannel != null)
			{
				_uavLostEventChannel.Subscribe(LogUavLost);
			}
		}
		
		private void LogUavLost(Uav arg0)
		{
			var log = new Log
			{
				logType = "NFZ",
				eventType = "NFZ Collision",
				logData = new
				{
					message = $"UAV {arg0.name} is lost due to NFZ collision"
				}
			};

			_logEventChannel.RaiseEvent(log);
		}


		private void OnDisable()
		{
			UnsubscribeFromChannels();
		}

		private void UnsubscribeFromChannels()
		{
			if (_uavLostEventChannel != null)
			{
				_uavLostEventChannel.Unsubscribe(LogUavLost);
			}
		}
	}
}