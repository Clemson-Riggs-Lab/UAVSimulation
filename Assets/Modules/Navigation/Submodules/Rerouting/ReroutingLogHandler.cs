using System;
using System.Collections.Generic;
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
				logData = new
				{
					message = $"Uav {arg0.uavName} requested one click rerouting"
				}

			};
			_logEventChannel.RaiseEvent(log);
		}
		
		
		private void LogReroutingEvent(Uav uav, Path path)
		{
			var logDataDict = new Dictionary<string, object>
			{
				{"message", $"Uav {uav.uavName} rerouted"}
			};

			if (_reroutingSettings.logIfReroutingWasNeeded)
			{
				var wasHeadingToNFZ = path.reroutedPath.headingToNFZ;
				logDataDict["reroutingNeeded"] = wasHeadingToNFZ ? "Rerouting was needed" : "Rerouting was not needed";
			}

			if (_reroutingSettings.logTimeOfPathStart)
			{
				LogTimeOfPathStart(logDataDict, uav, path);
			}

			if (_reroutingSettings.logIfNewRouteIsGoodOrBad)
			{
				var isHeadingToNFZ = CheckIfHeadingToNFZ(uav, path);
				logDataDict["routeQuality"] = isHeadingToNFZ ? "New route is bad" : "New route is good";
			}

			var log = new Log
			{
				logType = "Rerouting",
				eventType = "Rerouting Event",
				logData = logDataDict
			};

			_logEventChannel.RaiseEvent(log);
		}
		
		private void LogReroutingOptionPreviewed(Uav uav, Path path)
		{
			var logDataDict = new Dictionary<string, object>
			{
				{"message", $"Rerouting option previewed for Uav {uav.uavName}"}
			};

			if (_reroutingSettings.logTimeOfPathStart)
			{
				LogTimeOfPathStart(logDataDict, uav, path);
			}

			if (_reroutingSettings.logIfPreviewedRouteIsGoodOrBad)
			{
				var isHeadingToNFZ = CheckIfHeadingToNFZ(uav, path);
				logDataDict["previewedRoute"] = isHeadingToNFZ ? "Previewed route is bad" : "Previewed route is good";
			}

			var log = new Log
			{
				logType = "Rerouting",
				eventType = "Rerouting Option Previewed",
				logData = logDataDict
			};

			_logEventChannel.RaiseEvent(log);
		}

		private void LogReroutingOptionsRequested(Uav uav)
		{
			var logDataDict = new Dictionary<string, object>
			{
				{"message", $"requested rerouting options for uav {uav.uavName}"}
			};

			if (_reroutingSettings.logTimeOfPathStart)
			{
				LogTimeOfPathStart(logDataDict, uav, uav.currentPath);
			}

			if (_reroutingSettings.logIfReroutingWasNeeded)
			{
				var rerouteIsNeeded = CheckIfHeadingToNFZ(uav, uav.currentPath);
				logDataDict["reroutingNeeded"] = rerouteIsNeeded ? "Rerouting is needed" : "Rerouting is not needed";
			}

			var log = new Log
			{
				logType = "Rerouting",
				eventType = "Rerouting Options Requested",
				logData = logDataDict
			};

			_logEventChannel.RaiseEvent(log);
		}

		
		private void LogTimeOfPathStart(Dictionary<string, object> logDataDict, Uav uav, Path path)
		{
			try
			{
				logDataDict["pathStartTime"] = $"Path start time: {path.originalStartTimeDateTime.ToString("MM/dd/yyyy HH:mm:ss.ffffff")}";
			}
			catch
			{
				logDataDict["pathStartTime"] = "Path start time could not be logged, probably because it is the first path";
			}
		}

		private bool CheckIfHeadingToNFZ( Uav uav, Path path)
		{
			// linecast from the uav to the destination and check if it intersects with any NFZ based on the layer mask
			if (Physics.Linecast(uav.transform.position, path.destinationWayPoint.transform.position, out  var hit, 1 << LayerMask.NameToLayer("NFZ")))
			{
				return true;
				
			}
			else
			{
				return false;
			}
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