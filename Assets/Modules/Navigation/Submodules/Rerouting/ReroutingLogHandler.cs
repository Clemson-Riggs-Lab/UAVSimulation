using System;
using Modules.Logging;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Submodules.Rerouting.Settings.ScriptableObjects;
using Multiplayer;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UI.ReroutingPanel;
using UnityEngine;

namespace Modules.Navigation.Submodules.Rerouting
{
	public class ReroutingLogHandler:MonoBehaviour
	{
		private ReroutingSettingsSO _reroutingSettings;
		private LogEventChannelSO _logEventChannel;
		private UavEventChannelSO _reroutingOptionsRequestedChannel;
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
		}
		
		private void SubscribeToChannels()
		{
			if(_reroutingSettings.logReroutingOptionsRequested && _reroutingOptionsRequestedChannel != null)
				_reroutingOptionsRequestedChannel.Subscribe(LogReroutingOptionsRequested);
			
			if(_reroutingSettings.logReroutingOptionPreview && _uavReroutePreviewEventChannel != null)
				_uavReroutePreviewEventChannel.Subscribe(LogReroutingOptionPreviewed);

			if(_reroutingSettings.logReroutingEvents && _uavReroutedEventChannel != null)
				_uavReroutedEventChannel.Subscribe(LogReroutingEvent);

			ReroutingButtonsContainerController.DirectLogRerouteRequested_EventHandler += OnDirectLogRerouteRequestedEventHandler;
            ReroutingManager.DirectLogRerouteOptionPreviewed_EventHandler += DirectLogRerouteOptionPreviewedEventHandler;
		}

		private void OnDirectLogRerouteRequestedEventHandler(object sender, Uav e)
		{
			LogReroutingOptionsRequested(e);
		}

        private void DirectLogRerouteOptionPreviewedEventHandler(object sender, RerouteOptionPreviewedEventArgs e)
        {
			LogReroutingOptionPreviewed(e.UavObj, e.PathObj);
        }

        private void LogReroutingEvent(Uav uav, Path path)
		{
			var log = new Log();
			log.logType = "Rerouting";
			log.eventType = "Rerouting Event";
			log.logMessages = new() { $"Uav {uav.uavName}  rerouted"};

			if (AppNetPortal.Instance.IsMultiplayerMode())
				log.logGenerator = GameplayNetworkCallsHandler.Instance.GetCallerType(CallType.ReroutingUAV).ToString();
			else
				log.logGenerator = CallerType.None.ToString();

			if (_reroutingSettings.logIfReroutingWasNeeded)
			{
				try
				{
					var originalPath = path.previousPath.nextPath;// a hacky way to check if the uav was heading to a NFZ since the next path of the previous path remains as the original path
					var wasHeadingToNFZ=CheckIfHeadingToNFZ(uav,originalPath);
					log.logMessages.Add(wasHeadingToNFZ ? "Rerouting was needed" : "Rerouting was not needed");
				}
				catch 
				{
					log.logMessages.Add("Rerouting could not be determined, since the original path was not found");
				}
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
			var log = new Log();
			log.logType = "Rerouting";
			log.eventType = "Rerouting Option Previewed";
			log.logMessages = new() { $"Rerouting option previewed for Uav {uav.uavName} "};

            if (AppNetPortal.Instance.IsMultiplayerMode())
                log.logGenerator = GameplayNetworkCallsHandler.Instance.GetCallerType(CallType.RerouteOptionPreviewed).ToString();
            else
                log.logGenerator = CallerType.None.ToString();

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

            if (AppNetPortal.Instance.IsMultiplayerMode())
                log.logGenerator = GameplayNetworkCallsHandler.Instance.GetCallerType(CallType.RerouteOptionRequested).ToString();
            else
                log.logGenerator = CallerType.None.ToString();

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