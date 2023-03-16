using System;
using System.Collections.Generic;
using System.IO;
using Modules.Logging.Channels.ScriptableObjects;
using Newtonsoft.Json;
using UnityEngine;

namespace Modules.Logging
{
	public class ScoreSummaryLogger:MonoBehaviour
	{
		private LogEventChannelSO _logEventChannel;
		private DateTime _startTime;

		private static List<Log> Logs { get; set; } = new ();

		 
		private void Start()
		{
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			
			if(_logEventChannel!=null)
			{
				_logEventChannel.Subscribe(OnLogReceived);
			}
			_startTime = DateTime.Now;
		}
		
		private void OnLogReceived(Log log)
		{
			switch (log.eventType)
			{
				case "Score Keeper Updated":
				{
					//	ifLogs Contains a log with the same event type, then replace it with the new one
					if(Logs.Exists(x=>x.eventType==log.eventType))
					{
						Logs.Remove(Logs.Find(x=>x.eventType==log.eventType));
					}
					Logs.Add(log);
					break;
				}
				case "Scoring Criteria":
				{
					if(Logs.Exists(x=>x.eventType==log.eventType))
					{
						Logs.Remove(Logs.Find(x=>x.eventType==log.eventType));
					}
					Logs.Add(log);
					break;
				}
				default:
				{
					break;
				}
			}
		}
		
		
		private void OnDisable()
		{
			if(_logEventChannel!=null)
			{
				_logEventChannel.Unsubscribe(OnLogReceived);
			}

			AddSettingsLog();
			WriteLogsToFile();
		}

		private void AddSettingsLog()
		{
			var log= new Log
			{
				logType = "Settings",
				eventType = "Settings Log",
				logMessages = new ()
				{
					"Number Of Active Uavs For Rerouting: " + GameManager.Instance.settingsDatabase.navigationSettings.numberOfActiveUavsForRerouting,
					"Number Of Active Uavs For Target Detection: " + GameManager.Instance.settingsDatabase.targetDetectionSettings.numberOfActiveUavsForTargetDetection,
					"Probability Of Target in Target Detection Active Uavs: " + GameManager.Instance.settingsDatabase.targetDetectionSettings.ratioOfActiveFeedsWithTarget,
					"Probability of Non Target in Target Detection Active Uavs: " + GameManager.Instance.settingsDatabase.targetDetectionSettings.ratioOfActiveFeedsWithNonTarget,
					"Probability of NFZ heading Uav (Active for Rerouting): " + GameManager.Instance.settingsDatabase.nfzSettings.RatioOfHeadToNFZ,
					"Uav Speed:" + GameManager.Instance.settingsDatabase.navigationSettings.fixedSpeed,
					"Target Maximum distance from Uav Start Position (in seconds): " + GameManager.Instance.settingsDatabase.targetDetectionSettings.targetSpawnBufferRangeInSeconds.Max,
					"Target Minimum distance from Uav Start Position (in seconds): " + GameManager.Instance.settingsDatabase.targetDetectionSettings.targetSpawnBufferRangeInSeconds.Min,
					"Maximum path duration (in seconds): " + GameManager.Instance.settingsDatabase.navigationSettings.maxPathDuration,
					"Minimum path duration (in seconds): " + GameManager.Instance.settingsDatabase.navigationSettings.minPathDuration,
					"Minimum distance from NFZ (in seconds): " + GameManager.Instance.settingsDatabase.navigationSettings.minDistanceFromNFZInDuration,
					"Target Detection Task Priority:" + GameManager.Instance.settingsDatabase.uavCameraAndTargetDetectionPanelSettings.headerText,
					"Rerouting Task Priority:" + GameManager.Instance.settingsDatabase.reroutingPanelSettings.headerText,
				}
			};
			Logs.Add(log);
		}


		private void WriteLogsToFile()
		{
			var logFolder = Application.dataPath + "/logFiles/";
			if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
			
			string json = JsonConvert.SerializeObject(Logs, Formatting.Indented);
			using StreamWriter file = File.CreateText(logFolder+ _startTime.ToString("yyyy-MM-dd HH-mm-ss-") +"SummaryLogs.json");
			file.Write(json);
		}
	}
}