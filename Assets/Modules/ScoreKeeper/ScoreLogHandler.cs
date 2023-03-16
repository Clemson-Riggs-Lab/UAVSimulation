using Modules.Logging;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.ScoreKeeper.Channels.ScriptableObjects;
using Modules.ScoreKeeper.Settings.ScriptableObjects;
using UnityEngine;

namespace Modules.ScoreKeeper
{
	public class ScoreLogHandler :MonoBehaviour
	{	
		private ScoreKeepersSettingsSO _scoreKeepersSettings;
		private LogEventChannelSO _logEventChannel;
		private ScoreKeeperUpdatedEventChannelSO _scoreKeeperUpdatedEventChannel;
		private ScoreManager _scoreManager;
		
		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			
			if(_scoreKeepersSettings.logScoringCriteria)
				LogScoringCriteria();
		}

		

		private void GetReferencesFromGameManager()
		{
			_scoreKeepersSettings = GameManager.Instance.settingsDatabase.scoreKeepersSettings;
			_logEventChannel = GameManager.Instance.channelsDatabase.logEventChannel;
			_scoreKeeperUpdatedEventChannel = GameManager.Instance.channelsDatabase.scoreKeeperUpdatedEventChannel;
			_scoreManager = GameManager.Instance.scoreManager;
		}


		private void SubscribeToChannels()
		{
			
			if(_scoreKeepersSettings.logScoreResults && _scoreKeeperUpdatedEventChannel != null)
				_scoreKeeperUpdatedEventChannel.Subscribe(LogScoreKeeperUpdated);
		}

		private void LogScoreKeeperUpdated(Score score)
		{
			//Copilot seed = 33
			
			var log = new Log
			{
				logType = "Score",
				eventType = "Score Keeper Updated",
				logMessages = new ()
				{
					"ReroutingFalsePositiveCount: " + score.scoreCounts[ScoreManager.ScoreEventType.ReroutingFalsePositive],
					"ReroutingFalseNegativeCount: " + score.scoreCounts[ScoreManager.ScoreEventType.ReroutingFalseNegative],
					"ReroutingTruePositiveCount: " + score.scoreCounts[ScoreManager.ScoreEventType.ReroutingTruePositive],
					"ReroutingTrueNegativeCount: " + score.scoreCounts[ScoreManager.ScoreEventType.ReroutingTrueNegative],
					
					"TargetDetectionFalsePositiveCount: " + score.scoreCounts[ScoreManager.ScoreEventType.TargetDetectionFalsePositive],
					"TargetDetectionFalseNegativeCount: " + score.scoreCounts[ScoreManager.ScoreEventType.TargetDetectionFalseNegative],
					"TargetDetectionTruePositiveCount: " + score.scoreCounts[ScoreManager.ScoreEventType.TargetDetectionTruePositive],
					"TargetDetectionTrueNegativeCount: " + score.scoreCounts[ScoreManager.ScoreEventType.TargetDetectionTrueNegative],
					
					"ReroutingFalsePositiveScore: " + score.scoreValues[ScoreManager.ScoreEventType.ReroutingFalsePositive],
					"ReroutingFalseNegativeScore: " + score.scoreValues[ScoreManager.ScoreEventType.ReroutingFalseNegative],
					"ReroutingTruePositiveScore: " + score.scoreValues[ScoreManager.ScoreEventType.ReroutingTruePositive],
					"ReroutingTrueNegativeScore: " + score.scoreValues[ScoreManager.ScoreEventType.ReroutingTrueNegative],
					
					"TargetDetectionFalsePositiveScore: " + score.scoreValues[ScoreManager.ScoreEventType.TargetDetectionFalsePositive],
					"TargetDetectionFalseNegativeScore: " + score.scoreValues[ScoreManager.ScoreEventType.TargetDetectionFalseNegative],
					"TargetDetectionTruePositiveScore: " + score.scoreValues[ScoreManager.ScoreEventType.TargetDetectionTruePositive],
					"TargetDetectionTrueNegativeScore: " + score.scoreValues[ScoreManager.ScoreEventType.TargetDetectionTrueNegative],
				}
			};
			_logEventChannel.RaiseEvent(log);
		}
	
		private void LogScoringCriteria()
		{
			var log = new Log
			{
				logType = "Score",
				eventType = "Scoring Criteria",
				logMessages = new ()
				{
					"ReroutingFalsePositiveScore: " + _scoreKeepersSettings.reroutingFalsePositiveScore,
					"ReroutingFalseNegativeScore: " + _scoreKeepersSettings.reroutingFalseNegativeScore,
					"ReroutingTruePositiveScore: " + _scoreKeepersSettings.reroutingTruePositiveScore,
					"ReroutingTrueNegativeScore: " + _scoreKeepersSettings.reroutingTrueNegativeScore,
					
					"TargetDetectionFalsePositiveScore: " + _scoreKeepersSettings.targetDetectionFalsePositiveScore,
					"TargetDetectionFalseNegativeScore: " + _scoreKeepersSettings.targetDetectionFalseNegativeScore,
					"TargetDetectionTruePositiveScore: " + _scoreKeepersSettings.targetDetectionTruePositiveScore,
					"TargetDetectionTrueNegativeScore: " + _scoreKeepersSettings.targetDetectionTrueNegativeScore,
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
			
			if(_scoreKeepersSettings.logScoreResults && _scoreKeeperUpdatedEventChannel != null)
				_scoreKeeperUpdatedEventChannel.Unsubscribe(LogScoreKeeperUpdated);
		}
		
		
	}
}