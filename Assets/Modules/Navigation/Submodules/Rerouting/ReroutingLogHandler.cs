using System;
using Modules.Logging;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Submodules.Rerouting.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;

namespace Modules.Navigation.Submodules.Rerouting
{
	public class ReroutingLogHandler:MonoBehaviour
	{
		private ReroutingSettingsSO _reroutingSettings;
		private LogEventChannelSO _logEventChannel;
		private UavEventChannelSO _reroutingOptionsRequestedChannel;
		private UavEventChannelSO _oneClickReroutingRequestedChannel;
		private UavPathEventChannelSO _uavReroutePreviewEventChannel;
		private UavPathEventChannelSO _uavReroutedEventChannel;
		
		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
		}

		private void GetReferencesFromGameManager()
		{
			_reroutingSettings = GameManager.Instance.settingsDatabase.reroutingSettings;
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			_uavReroutedEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutedEventChannel;
			_uavReroutePreviewEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutePreviewEventChannel;
			_reroutingOptionsRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.reroutingOptionsRequestedChannel;
			_oneClickReroutingRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.oneClickReroutingRequestedChannel;
		}
		
		private void SubscribeToChannels()
		{
			if(_reroutingSettings.logReroutingOptionsRequested && _reroutingOptionsRequestedChannel != null)
				_reroutingOptionsRequestedChannel.Subscribe(LogReroutingOptionsRequested);
			
			if(_reroutingSettings.logReroutingOptionPreview && _uavReroutePreviewEventChannel != null)
				_uavReroutePreviewEventChannel.Subscribe(LogReroutingOptionPreviewed);

			if(_reroutingSettings.logReroutingEvents && _uavReroutedEventChannel != null)
				_uavReroutedEventChannel.Subscribe(LogReroutingEvent);
			
			if(_reroutingSettings.logOneClickReroutingRequested && _oneClickReroutingRequestedChannel != null)
				_oneClickReroutingRequestedChannel.Subscribe(LogOneClickReroutingRequested);
		}

		private void LogOneClickReroutingRequested(Uav arg0)
		{
			var log = new Log
			{
				logType = "Rerouting",
				eventType = "One Click Rerouting Requested",
				logMessages = new() { $"Uav {arg0.uavName} requested one click rerouting"}
			};
			_logEventChannel.RaiseEvent(log);
		}

		private void LogReroutingEvent(Uav uav, Path path)
		{
			var log = new Log
			{
				logType = "Rerouting",
				eventType = "Rerouting Event",
				logMessages = new() { $"Uav {uav.uavName}  rerouted"}
			};
			if (_reroutingSettings.logIfReroutingWasNeeded)
			{
				var wasHeadingToNFZ = path.headingToNFZ;
				log.logMessages.Add(wasHeadingToNFZ ? "Rerouting was needed" : "Rerouting was not needed");
			}

			if (_reroutingSettings.logTimeOfPathStart)
			{
				LogTimeOfPathStart(log, uav, path);
			}

			if (_reroutingSettings.logIfNewRouteIsGoodOrBad)
			{
				var isHeadingToNFZ=CheckIfHeadingToNFZ(uav,path);
				log.logMessages.Add(isHeadingToNFZ ? "New route is bad" : "New route is good");
			}
				
			_logEventChannel.RaiseEvent(log);	
			
		}
		

		private void LogReroutingOptionPreviewed(Uav uav, Path path)
		{
			var log = new Log
			{
				logType = "Rerouting",
				eventType = "Rerouting Option Previewed",
				logMessages = new() { $"Rerouting option previewed for Uav {uav.uavName} "}
			};
			if (_reroutingSettings.logTimeOfPathStart)
			{
				LogTimeOfPathStart(log, uav, path);
			}
			
			if(_reroutingSettings.logIfPreviewedRouteIsGoodOrBad)
			{
				var isHeadingToNFZ=CheckIfHeadingToNFZ(uav,path);
				log.logMessages.Add(isHeadingToNFZ ? "Previewed route is bad" : "Previewed route is good");
			}
			
			_logEventChannel.RaiseEvent(log);
		}

		private void LogReroutingOptionsRequested(Uav uav)
		{
			var log = new Log();
			log.logType = "Rerouting";
			log.eventType = "Rerouting Options Requested";
			log.logMessages = new() { $" requested rerouting options for uav {uav.uavName}"};

			if (_reroutingSettings.logTimeOfPathStart)
			{
				LogTimeOfPathStart(log, uav, uav.currentPath);
			}

			if (_reroutingSettings.logIfReroutingWasNeeded)
			{
				var rerouteIsNeeded =CheckIfHeadingToNFZ(uav,uav.currentPath);
				log.logMessages.Add(rerouteIsNeeded ? "Rerouting is needed" : "Rerouting is not needed");
			}
			
			_logEventChannel.RaiseEvent(log);
		}
		
		
		private void LogTimeOfPathStart(Log log, Uav uav, Path path)
		{
			try
			{
				log.logMessages.Add($"Path start time: {path.startTime.ToString("MM/dd/yyyy HH:mm:ss.ffffff")}");
			}
			catch 
			{
				log.logMessages.Add($"Path start time could not be logged, probably because it is the first path");
			}
		}

		private bool CheckIfHeadingToNFZ( Uav uav, Path path)
		{
			// linecast from the uav to the destination and check if it intersects with any NFZ based on the layer mask
			var hit = Physics2D.Linecast(uav.transform.position, path.destinationWayPoint.transform.position, 1 << LayerMask.NameToLayer("NFZ"));
			return  hit.collider != null;
		}


		private void OnDisable()
		{
			UnsubscribeFromChannels();
		}

		private void UnsubscribeFromChannels()
		{
			if( _reroutingOptionsRequestedChannel != null)
				_reroutingOptionsRequestedChannel.Unsubscribe(LogReroutingOptionsRequested);
			
			if( _uavReroutePreviewEventChannel != null)
				_uavReroutePreviewEventChannel.Unsubscribe(LogReroutingOptionPreviewed);
			
			if( _uavReroutedEventChannel != null)
				_uavReroutedEventChannel.Unsubscribe(LogReroutingEvent);
		}
	}
}