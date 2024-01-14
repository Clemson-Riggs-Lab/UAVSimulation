using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Submodules.Rerouting.Settings.ScriptableObjects;
using Modules.ScoreKeeper;
using Modules.ScoreKeeper.Channels.ScriptableObjects;
using Newtonsoft.Json;
using ScriptableObjects.EventChannels;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums.ConditionalState;
using Path = Modules.Navigation.Path;

namespace Modules.Logging
{
	public class ScoreSummaryLogger:MonoBehaviour
	{
		private LogEventChannelSO _logEventChannel;
		private ReroutingSettingsSO _reroutingSettings;
		private UavEventChannelSO _oneClickReroutingRequestedChannel;
		private UavPathEventChannelSO _targetDetectedEventChannel;
		private UavEventChannelSO _reroutingOptionsRequestedChannel;
		private VoidEventChannelSO _simulationEndedEventChannel;

		private DateTime _startTime;
		private ScoreKeeperUpdatedEventChannelSO _scoreKeeperUpdatedEventChannel;

		private static List<Log> Logs { get; set; } = new ();
		
		
		private Score _score;
		private int _totalOneClickReroute;
		private int _totalFakeFlareCountedAsTarget;
		private int _totalPreviewRequestedDueToOneClickRerouteButtonOn;
		private Dictionary<int,float> _oneClickRerouteTimesDictionary=new ();
		private Dictionary<int,float> _fakeFlareCountedAsTargetTimesDictionary=new ();
		private Dictionary<int,float> _previewRequestedDueToOneClickRerouteButtonOnTimesDictionary=new();
		
		private bool _simulationCompleted = false;

		
		private void Start()
		{
			GetReferencesFromGameManager ();
			SubscribeToChannels();
		
			
			_startTime = DateTime.Now;
		}

		private void GetReferencesFromGameManager()
		{
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			_scoreKeeperUpdatedEventChannel = GameManager.Instance.channelsDatabase.scoreKeeperUpdatedEventChannel;
			_oneClickReroutingRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.oneClickReroutingRequestedChannel;
			_targetDetectedEventChannel = GameManager.Instance.channelsDatabase.targetDetectionChannels.targetDetectedEventChannel;
			_reroutingOptionsRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.reroutingOptionsRequestedChannel;
			_reroutingSettings = GameManager.Instance.settingsDatabase.reroutingSettings;
			_simulationEndedEventChannel = GameManager.Instance.channelsDatabase.simulationEndedEventChannel;
		}

		private void SubscribeToChannels()
		{
				
			if(_logEventChannel!=null)
				_logEventChannel.Subscribe(OnLogReceived);
			
			if(_scoreKeeperUpdatedEventChannel!=null)
				_scoreKeeperUpdatedEventChannel.Subscribe(OnScoreKeeperUpdated);
			
			if(_reroutingSettings.logOneClickReroutingRequested && _oneClickReroutingRequestedChannel != null)
				_oneClickReroutingRequestedChannel.Subscribe(LogOneClickReroutingRequested);
			
			if(_targetDetectedEventChannel != null)
				_targetDetectedEventChannel.Subscribe(OnTargetDetected);
			
			if(_reroutingSettings.logReroutingOptionsRequested && _reroutingOptionsRequestedChannel != null)
				_reroutingOptionsRequestedChannel.Subscribe(LogReroutingOptionsRequested);
			
			if(_simulationEndedEventChannel!=null)
				_simulationEndedEventChannel.Subscribe(OnSimulationEnded);
		}

		private void OnSimulationEnded()
		{ 
			_simulationCompleted = true;
		}

		private void LogReroutingOptionsRequested(Uav uav)
		{
			if (uav.currentPath.OneClickRerouteButtonCondition is TP or FP)
			{
				_totalPreviewRequestedDueToOneClickRerouteButtonOn++;
				_previewRequestedDueToOneClickRerouteButtonOnTimesDictionary[uav.currentPath.id]=Time.time;
			}
		}

		private void LogOneClickReroutingRequested(Uav uav)
		{
			_totalOneClickReroute++;
			_oneClickRerouteTimesDictionary[uav.currentPath.id]=Time.time;;
		}
		
		private void OnTargetDetected(Uav uav, Path path)
		{
			if (path.nonTargetIsPresent)
			{
				_totalFakeFlareCountedAsTarget++;
				_fakeFlareCountedAsTargetTimesDictionary[uav.currentPath.id]=Time.time;;
			}
		}
		private void OnScoreKeeperUpdated(Score score)
		{
			_score = score;
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
			UnsubscribeFromChannels();
			AddSettingsLog();
			AddResponseTimeLog();
			AddOneClickRerouteFrequencyLog();
			AddFakeFlareCountedAsTargetLog();
			AddPreviewRequestedDueToOneClickRerouteButtonOnLog();
			AddSimulationEndedLog();
			WriteLogsToFile();
		}

		private void AddSimulationEndedLog()
		{
			var log = new Log
			{
				logType = "Simulation Ended",
				eventType = "Simulation Ended",
				logData = new
				{
					simulationCompleted = _simulationCompleted
				}
			};
			Logs.Add(log);
		}

		private void AddPreviewRequestedDueToOneClickRerouteButtonOnLog()
		{ 
			var log = new Log
			{
				logType = "Preview Requested Due To One Click Reroute Button On",
				eventType = "Preview Requested Due To One Click Reroute Button On",
				logData = new
				{

					previewRequestedDueToOneClickRerouteButtonOnFrequency = _totalPreviewRequestedDueToOneClickRerouteButtonOn,
					previewRequestedDueToOneClickRerouteButtonOnTimes = _previewRequestedDueToOneClickRerouteButtonOnTimesDictionary
				}
			};

		}

		private void AddFakeFlareCountedAsTargetLog()
		{ 
			var log = new Log
			{
				logType = "Fake Flare Counted As Target",
				eventType = "Fake Flare Counted As Target",
				logData = new
				{
					fakeFlareCountedAsTargetFrequency = _totalFakeFlareCountedAsTarget,
					fakeFlareCountedAsTargetTimes = _fakeFlareCountedAsTargetTimesDictionary
				}
			};
			Logs.Add(log);
		}

		private void UnsubscribeFromChannels()
		{
			if (_logEventChannel != null)
				_logEventChannel.Unsubscribe(OnLogReceived);
			
			if (_scoreKeeperUpdatedEventChannel != null)
				_scoreKeeperUpdatedEventChannel.Unsubscribe(OnScoreKeeperUpdated);

			if (_oneClickReroutingRequestedChannel != null)
				_oneClickReroutingRequestedChannel.Unsubscribe(LogOneClickReroutingRequested);
			
			if (_targetDetectedEventChannel != null)
				_targetDetectedEventChannel.Unsubscribe(OnTargetDetected);
			
			if (_reroutingOptionsRequestedChannel != null)
				_reroutingOptionsRequestedChannel.Unsubscribe(LogReroutingOptionsRequested);
			
			if (_simulationEndedEventChannel != null)
				_simulationEndedEventChannel.Unsubscribe(OnSimulationEnded);
		}

		private void AddOneClickRerouteFrequencyLog()
		{
			var log = new Log
			{
				logType = "One Click Reroute Frequency",
				eventType = "One Click Reroute Frequency",
				logData = new
				{
					oneClickRerouteFrequency = _totalOneClickReroute,
					oneClickRerouteTimes = _oneClickRerouteTimesDictionary
					
				}
			};
			Logs.Add(log);
		}

		private void AddResponseTimeLog()
		{
			var log = new Log
			{
				logType = "Response Time",
				eventType = "Response Time",
				logData = new
				{
					reroutingResponseTime = _score.reroutingResponseTimePathIdDictionary.Values.Count > 0
						? (double?)_score.reroutingResponseTimePathIdDictionary.Values.Average()
						: null,
					targetDetectionResponseTime = _score.targetDetectionResponseTimePathIdDictionary.Values.Count > 0
						? (double?)_score.targetDetectionResponseTimePathIdDictionary.Values.Average()
						: null,
				}
			};
			Logs.Add(log);
		}
		
		private void AddSettingsLog()
		{
		    var log = new Log
		    {
		        logType = "Settings",
		        eventType = "Settings Log",
		        logData = new
		        {
			       runSettings= new
			        {
				        participantNumber = GameManager.Instance.participantNumber,
				        trialNumber= GameManager.Instance.trialNumber,
				        starttime= _startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				        endTime= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				        settingsFile= System.IO.Path.GetFileName(GameManager.Instance.configFilesSettings.settingsFileFullFilePath)
				        
			        },
		            navigationSettings = new
		            {
		                numberOfActiveUavsForRerouting = GameManager.Instance.settingsDatabase.navigationSettings.numberOfActiveUavsForRerouting,
		                fixedSpeed = GameManager.Instance.settingsDatabase.navigationSettings.fixedSpeed,
		                reroutingMaxResponseTime = GameManager.Instance.settingsDatabase.navigationSettings.reroutingMaxResponseTime,
		                maxPathDuration = GameManager.Instance.settingsDatabase.navigationSettings.maxPathDuration,
		                minPathDuration = GameManager.Instance.settingsDatabase.navigationSettings.minPathDuration,
		                minDistanceFromNFZInDuration = GameManager.Instance.settingsDatabase.navigationSettings.minDistanceFromNFZInDuration,
		                maxDistanceFromNFZInDuration = GameManager.Instance.settingsDatabase.navigationSettings.maxDistanceFromNFZInDuration,
		            },
		            targetDetectionSettings = new
		            {
		                numberOfActiveUavsForTargetDetection = GameManager.Instance.settingsDatabase.targetDetectionSettings.numberOfActiveUavsForTargetDetection,
		                ratioOfActiveFeedsWithTarget = GameManager.Instance.settingsDatabase.targetDetectionSettings.ratioOfActiveFeedsWithTarget,
		                ratioOfActiveFeedsWithNonTarget = GameManager.Instance.settingsDatabase.targetDetectionSettings.ratioOfActiveFeedsWithNonTarget,
		                targetDetectionMaxResponseTime = GameManager.Instance.settingsDatabase.targetDetectionSettings.targetDetectionMaxResponseTime,
		            },
		            nfzSettings = new
		            {
		                RatioOfHeadToNFZ = GameManager.Instance.settingsDatabase.nfzSettings.RatioOfHeadToNFZ,
		            },
		            panelSettings = new
		            {
		                targetDetectionTaskPriority = GameManager.Instance.settingsDatabase.uavCameraAndTargetDetectionPanelSettings.headerText,
		                reroutingTaskPriority = GameManager.Instance.settingsDatabase.reroutingPanelSettings.headerText,
		            },
		            reroutingSettings = new
		            {
			            oneClickRerouting = GameManager.Instance.settingsDatabase.reroutingSettings.oneClickRerouteEnabled,
			            oneClickReroutingFalsePositive = GameManager.Instance.settingsDatabase.reroutingSettings.oneClickRerouteFalsePositiveProbability,
			            oneClickReroutingFalseNegative = GameManager.Instance.settingsDatabase.reroutingSettings.oneClickRerouteFalseNegativeProbability,
			            probabilityOfUnsuccessfulOneClickReroute= GameManager.Instance.settingsDatabase.reroutingSettings.probabilityOfUnsuccessfulOneClickReroute,

		            }
		            
		        }
		    };
		    Logs.Add(log);
		}


		private void WriteLogsToFile()
		{
			var logFolder = Application.dataPath + "/logFiles/";
			if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
			
			string json = JsonConvert.SerializeObject(Logs, Formatting.Indented);
			var participantNumber= GameManager.Instance.participantNumber;
			var trialNumber = GameManager.Instance.trialNumber;
			
			using StreamWriter file = File.CreateText(logFolder+ "p"+ participantNumber + "_t" + trialNumber + "_" + _startTime.ToString("yyyy-MM-dd HH-mm-ss_") +"SummaryLogs.json");
			file.Write(json);
		}
	}
}