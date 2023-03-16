using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.ScoreKeeper.Channels.ScriptableObjects;
using Modules.ScoreKeeper.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;
using static Modules.ScoreKeeper.ScoreManager.ScoreEventType;

namespace Modules.ScoreKeeper
{
	public class ScoreManager:MonoBehaviour
	{
		
		private UavEventChannelSO _uavDestroyedEventChannel;
		private UavEventChannelSO _uavLostEventChannel;
		private UavPathEventChannelSO _uavArrivedAtDestinationEventChannel;
		private UavPathEventChannelSO _uavReroutedEventChannel;
		private UavPathEventChannelSO _uavStartedNewPathEventChannel;
		private UavPathEventChannelSO _targetDetectedEventChannel;
		private UavPathEventChannelSO _targetNotDetectedEventChannel;
		private ScoreKeepersSettingsSO _scoreKeepersSettings;
		
		private ScoreKeeperUpdatedEventChannelSO _scoreKeeperUpdatedEventChannel;
		
		private Score _score;
		
		private Dictionary<Uav,List<PathScoreData>> _pathScoreDataDictionary = new Dictionary<Uav, List<PathScoreData>>();
		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			_score = new Score();
			
			var ScoreLogHandler = gameObject.GetOrAddComponent<ScoreLogHandler>();
			ScoreLogHandler.Initialize();
		}

		private void SubscribeToChannels()
		{
			if(_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
            
			if(_uavLostEventChannel != null)
				_uavLostEventChannel.Subscribe(OnUavLost);

			if (_uavArrivedAtDestinationEventChannel != null)
				_uavArrivedAtDestinationEventChannel.Subscribe(OnUavArrivedAtDestination);

			if (_uavReroutedEventChannel != null)
				_uavReroutedEventChannel.Subscribe(OnUavRerouted);
            
			if(_uavStartedNewPathEventChannel != null)
				_uavStartedNewPathEventChannel.Subscribe(OnUavStartedNewPath);
			
			if(_targetDetectedEventChannel != null)
				_targetDetectedEventChannel.Subscribe(OnTargetDetected);
			
			if(_targetNotDetectedEventChannel != null)
				_targetNotDetectedEventChannel.Subscribe(OnTargetNotDetected);
		}

		private void OnUavLost(Uav uav)
		{
			
			if(uav.currentPath == null || !uav.currentPath.uavIsVisuallyEnabledForRerouting) return;
			var  lastPathScoreData = GetLastPathScoreData(uav, uav.currentPath);
			if( uav.currentPath.headingToNFZ)
				lastPathScoreData.pathScoreEvents.Add( new PathScoreEvent(){ path = uav.currentPath,scoreEventType= ReroutingFalseNegative, time = Time.time});
			lastPathScoreData.finalizedScore = true;
			UpdateScore(lastPathScoreData);
		}

		private void OnUavArrivedAtDestination(Uav uav, Path path)
		{
			var lastPathScoreData = GetLastPathScoreData(uav, path);
			if(path.uavIsVisuallyEnabledForRerouting)
			{
				if (!lastPathScoreData.pathScoreEvents.Any(pse => pse.scoreEventType == ReroutingTruePositive) &&
			     !lastPathScoreData.pathScoreEvents.Any(pse => pse.scoreEventType == ReroutingFalsePositive))
				lastPathScoreData.pathScoreEvents.Add( new PathScoreEvent(){ path = path,scoreEventType= ReroutingTrueNegative, time = Time.time});
				
			}

			if (path.uavIsVisuallyEnabledForTargetDetection)
			{
				if (!lastPathScoreData.pathScoreEvents.Any(pse => pse.scoreEventType == TargetDetectionTruePositive) &&
				    !lastPathScoreData.pathScoreEvents.Any(pse => pse.scoreEventType == TargetDetectionFalsePositive))
					lastPathScoreData.pathScoreEvents.Add(new PathScoreEvent()
					{
						path = path,
						scoreEventType = path.targetIsPresent
							? TargetDetectionFalseNegative
							: TargetDetectionTrueNegative,
						time = Time.time
					});
			}
			lastPathScoreData.finalizedScore = true;
			UpdateScore(lastPathScoreData);
		}

		private void OnUavStartedNewPath(Uav uav, Path path)
		{
			if(!_pathScoreDataDictionary.ContainsKey(uav))
				_pathScoreDataDictionary.Add(uav, new List<PathScoreData>());
			
			_pathScoreDataDictionary[uav].Add(new PathScoreData(path));
		}

		private void OnUavRerouted(Uav uav, Path path)
		{
			if (!path.uavIsVisuallyEnabledForRerouting)
			{
				Debug.LogWarning("UAV rerouted but rerouting is not enabled for this path");
				return;
			}
			var lastPathScoreData = GetLastPathScoreData(uav, path);

			lastPathScoreData.pathScoreEvents.Add( new PathScoreEvent(){
				path = path,scoreEventType= path.reroutedPath.headingToNFZ
					? ReroutingTruePositive
					: ReroutingFalsePositive, time = Time.time});
			if(_scoreKeepersSettings.awardAndPenalizeForIntermediaryRerouting)
				UpdateScore(lastPathScoreData);
		}

		private void OnTargetDetected(Uav uav, Path path)
		{
			var lastPathScoreData = GetLastPathScoreData(uav, path);

			lastPathScoreData.pathScoreEvents.Add(new PathScoreEvent(){
				path = path,scoreEventType= path.targetIsPresent
					? TargetDetectionTruePositive
					: TargetDetectionFalsePositive, time = Time.time});
			
			if(_scoreKeepersSettings.awardAndPenalizeForIntermediaryTargetDetection)
				UpdateScore(lastPathScoreData);
		}

		private void OnTargetNotDetected(Uav uav, Path path)
		{
			
			var lastPathScoreData = GetLastPathScoreData(uav, path);
			
				lastPathScoreData.pathScoreEvents.Add(new PathScoreEvent {
					path = path,scoreEventType= path.targetIsPresent
					? TargetDetectionFalseNegative
					: TargetDetectionTrueNegative, time = Time.time});

				if(_scoreKeepersSettings.awardAndPenalizeForIntermediaryTargetDetection)
					UpdateScore(lastPathScoreData);
		}

		private PathScoreData GetLastPathScoreData(Uav uav, Path path)
		{
			_pathScoreDataDictionary.TryGetValue(uav, out var pathScoreDataList);
			var lastPathScoreData = pathScoreDataList?.LastOrDefault();

			if (lastPathScoreData?.path.id == path.id) return lastPathScoreData;
			if (lastPathScoreData != null)
			{
				lastPathScoreData.finalizedScore = true;
				UpdateScore(lastPathScoreData);
				Debug.Log("Finalized score for path: " + lastPathScoreData.path.id + " for uav: " + uav.id +
				          ", This should not happen, please report this bug");
			}

			PathScoreData newpath= new PathScoreData(path);
			_pathScoreDataDictionary[uav].Add(newpath);
			return newpath;
		}

		private void OnUavDestroyed(Uav uav)
		{
			if (_pathScoreDataDictionary.ContainsKey(uav))
			{
				//update the score for last path
				var pathScoreDataList = _pathScoreDataDictionary[uav];
				var lastPathScoreData = pathScoreDataList[pathScoreDataList.Count - 1];
				UpdateScore(lastPathScoreData);
			}
		}

		private void UpdateScore(PathScoreData lastPathScoreData)
		{
			if (!lastPathScoreData.finalizedScore)
			{
				//get the last score event from the list
				var lastScoreEvent =lastPathScoreData.pathScoreEvents.LastOrDefault();
				if(lastScoreEvent is null) return;
				UpdateScoreCounts(lastScoreEvent.scoreEventType);
				lastScoreEvent.accountedForInScore = true;
				
			}
			else
			{
				foreach (var pathScoreEvent in lastPathScoreData.pathScoreEvents.Where(pathScoreEvent => !pathScoreEvent.accountedForInScore))
				{
					UpdateScoreCounts(pathScoreEvent.scoreEventType);
					pathScoreEvent.accountedForInScore = true;
				}
			}
			RecalculateScores();
		}

		private void UpdateScoreCounts(ScoreEventType lastScoreEvent)
		{
			if (_score.scoreCounts.ContainsKey(lastScoreEvent))
			{
				_score.scoreCounts[lastScoreEvent]++;
			}
		}

		public void RecalculateScores()
		{
			foreach (var (scoreEventType, scoreEventCount) in _score.scoreCounts)
			{
				float scoreValue = 0f;
				switch (scoreEventType)
				{
					case ReroutingFalsePositive:
						scoreValue = _scoreKeepersSettings.reroutingFalsePositiveScore;
						break;
					case ReroutingFalseNegative:
						scoreValue = _scoreKeepersSettings.reroutingFalseNegativeScore;
						break;
					case ReroutingTruePositive:
						scoreValue = _scoreKeepersSettings.reroutingTruePositiveScore;
						break;
					case ReroutingTrueNegative:
						scoreValue = _scoreKeepersSettings.reroutingTrueNegativeScore;
						break;
					case TargetDetectionFalsePositive:
						scoreValue = _scoreKeepersSettings.targetDetectionFalsePositiveScore;
						break;
					case TargetDetectionFalseNegative:
						scoreValue = _scoreKeepersSettings.targetDetectionFalseNegativeScore;
						break;
					case TargetDetectionTruePositive:
						scoreValue = _scoreKeepersSettings.targetDetectionTruePositiveScore;
						break;
					case TargetDetectionTrueNegative:
						scoreValue = _scoreKeepersSettings.targetDetectionTrueNegativeScore;
						break;
					default:
						Debug.LogError("Unknown ScoreEventType: " + scoreEventType);
						break;
				}
				_score.scoreValues[scoreEventType] = scoreValue * scoreEventCount;
			}
			// publish updated score event
			_scoreKeeperUpdatedEventChannel.RaiseEvent(_score);
		}

		private void GetReferencesFromGameManager()
		{
			_uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavLostEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavLostEventChannel;
			_uavStartedNewPathEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavStartedNewPathEventChannel;
			_uavArrivedAtDestinationEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavArrivedAtDestinationEventChannel;
			_uavReroutedEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutedEventChannel;
			_targetDetectedEventChannel = GameManager.Instance.channelsDatabase.targetDetectionChannels.targetDetectedEventChannel;
			_targetNotDetectedEventChannel = GameManager.Instance.channelsDatabase.targetDetectionChannels.targetNotDetectedEventChannel;
			_scoreKeepersSettings = GameManager.Instance.settingsDatabase.scoreKeepersSettings;
			_scoreKeeperUpdatedEventChannel = GameManager.Instance.channelsDatabase.scoreKeeperUpdatedEventChannel;
		}


		public class PathScoreData
		{
			public List<PathScoreEvent> pathScoreEvents = new List<PathScoreEvent>();
			public Path path;
			public bool finalizedScore=false;
			
			public PathScoreData(Path path)
			{
				this.path = path;
			}
		}
		
		public class PathScoreEvent
		{
			public ScoreEventType scoreEventType;
			public float time;
			public Path path;
			public bool accountedForInScore=false;
		}
		public enum ScoreEventType
		{
			ReroutingFalsePositive,
			ReroutingFalseNegative,
			ReroutingTruePositive,
			ReroutingTrueNegative,
			TargetDetectionFalsePositive,
			TargetDetectionFalseNegative,
			TargetDetectionTruePositive,
			TargetDetectionTrueNegative
		}

		
		private void OnDisable()
		{
			UnsubscribeFromChannels();
		}

		private void UnsubscribeFromChannels()
		{
			if( _uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
			
			if( _uavLostEventChannel != null)
				_uavLostEventChannel.Unsubscribe(OnUavLost);
			
			if( _uavStartedNewPathEventChannel != null)
				_uavStartedNewPathEventChannel.Unsubscribe(OnUavStartedNewPath);
			
			if( _uavArrivedAtDestinationEventChannel != null)
				_uavArrivedAtDestinationEventChannel.Unsubscribe(OnUavArrivedAtDestination);
			
			if( _uavReroutedEventChannel != null)
				_uavReroutedEventChannel.Unsubscribe(OnUavRerouted);
			
			if( _targetDetectedEventChannel != null)
				_targetDetectedEventChannel.Unsubscribe(OnTargetDetected);
			
			if( _targetNotDetectedEventChannel != null)
				_targetNotDetectedEventChannel.Unsubscribe(OnTargetNotDetected);

		}
	}
}