using System;
using Modules.Logging;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.NoFlyZone.Settings.ScriptableObjects;
using Multiplayer;
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
			var log = new Log();
			log.logType = "NFZ";
			log.eventType = " NFZ Collision";
			log.logMessages = new() { $"UAV {arg0.name}is lost due to NFZ collision" };

			log.logGenerator = CallerType.None.ToString();

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