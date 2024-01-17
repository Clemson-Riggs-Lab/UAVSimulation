using System.Linq;
using HelperScripts;
using Modules.Logging;
using Modules.Logging.Channels.ScriptableObjects;
using Modules.ScoreKeeper.Channels.ScriptableObjects;
using Modules.ScoreKeeper.Settings.ScriptableObjects;
using UnityEngine;
using static Modules.ScoreKeeper.ScoreManager.AIRelatedScoreEventType;
using static Modules.ScoreKeeper.ScoreManager.ScoreEventType;

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
			var log = new Log
{
    logType = "Score",
    eventType = "Score Keeper Updated",
    logData = new
    {
        ReroutingCounts = new
        {
            ReroutingFalsePositiveCount = score.scoreCounts[ReroutingFalsePositive],
            ReroutingFalseNegativeCount = score.scoreCounts[ReroutingFalseNegative],
            ReroutingTruePositiveCount = score.scoreCounts[ReroutingTruePositive],
            ReroutingTrueNegativeCount = score.scoreCounts[ReroutingTrueNegative],
           
            ReroutingTruePositiveWithAITruePositiveCount = score.aIRelatedScoreCounts[ReroutingTruePositiveWithAITruePositive].ToCustomString() ,
            ReroutingTruePositiveDespiteAIFalseNegativeCount = score.aIRelatedScoreCounts[ReroutingTruePositiveDespiteAIFalseNegative].ToCustomString(),
            ReroutingTrueNegativeWithAITrueNegativeCount = score.aIRelatedScoreCounts[ReroutingTrueNegativeWithAITrueNegative].ToCustomString(),
            ReroutingTrueNegativeDespiteAIFalsePositiveCount = score.aIRelatedScoreCounts[ReroutingTrueNegativeDespiteAIFalsePositive].ToCustomString(),
            ReroutingFalsePositiveDueToAIFalsePositiveCount = score.aIRelatedScoreCounts[ReroutingFalsePositiveDueToAIFalsePositive].ToCustomString(),
            ReroutingFalsePositiveDespiteAITrueNegativeCount = score.aIRelatedScoreCounts[ReroutingFalsePositiveDespiteAITrueNegative].ToCustomString(),
            ReroutingFalseNegativeDespiteAITruePositiveCount = score.aIRelatedScoreCounts[ReroutingFalseNegativeDespiteAITruePositive].ToCustomString(),
            ReroutingFalseNegativeDueToAIFalseNegativeCount = score.aIRelatedScoreCounts[ReroutingFalseNegativeDueToAIFalseNegative].ToCustomString(),
	        
        },
        TargetDetectionCounts = new
        {
            TargetDetectionFalsePositiveCount = score.scoreCounts[TargetDetectionFalsePositive],
            TargetDetectionFalseNegativeCount = score.scoreCounts[TargetDetectionFalseNegative],
            TargetDetectionTruePositiveCount = score.scoreCounts[TargetDetectionTruePositive],
            TargetDetectionTrueNegativeCount = score.scoreCounts[TargetDetectionTrueNegative],
        },
        // ReroutingScores = new
        // {
        //     ReroutingFalsePositiveScore = score.scoreValues[ReroutingFalsePositive],
        //     ReroutingFalseNegativeScore = score.scoreValues[ReroutingFalseNegative],
        //     ReroutingTruePositiveScore = score.scoreValues[ReroutingTruePositive],
        //     ReroutingTrueNegativeScore = score.scoreValues[ReroutingTrueNegative],
        //     
        //     ReroutingTruePositiveWithAITruePositiveScore = score.aIRelatedScoreValues[ReroutingTruePositiveWithAITruePositive],
        //     ReroutingTruePositiveDespiteAIFalseNegativeScore = score.aIRelatedScoreValues[ReroutingTruePositiveDespiteAIFalseNegative],
        //     ReroutingTrueNegativeWithAITrueNegativeScore = score.aIRelatedScoreValues[ReroutingTrueNegativeWithAITrueNegative],
        //     ReroutingTrueNegativeDespiteAIFalsePositiveScore = score.aIRelatedScoreValues[ReroutingTrueNegativeDespiteAIFalsePositive],
        //     ReroutingFalsePositiveDueToAIFalsePositiveScore = score.aIRelatedScoreValues[ReroutingFalsePositiveDueToAIFalsePositive],
        //     ReroutingFalsePositiveDespiteAITrueNegativeScore = score.aIRelatedScoreValues[ReroutingFalsePositiveDespiteAITrueNegative],
        //     ReroutingFalseNegativeDespiteAITruePositiveScore = score.aIRelatedScoreValues[ReroutingFalseNegativeDespiteAITruePositive],
        //     ReroutingFalseNegativeDueToAIFalseNegativeScore = score.aIRelatedScoreValues[ReroutingFalseNegativeDueToAIFalseNegative],
        //     
        // },
        // TargetDetectionScores = new
        // {
        //     TargetDetectionFalsePositiveScore = score.scoreValues[TargetDetectionFalsePositive],
        //     TargetDetectionFalseNegativeScore = score.scoreValues[TargetDetectionFalseNegative],
        //     TargetDetectionTruePositiveScore = score.scoreValues[TargetDetectionTruePositive],
        //     TargetDetectionTrueNegativeScore = score.scoreValues[TargetDetectionTrueNegative],
        // },
        ReroutingResponseTime = score.reroutingResponseTimePathIdDictionary,
        TargetDetectionResponseTime = score.targetDetectionResponseTimePathIdDictionary,
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
				logData = new
				{
					ReroutingScores = new
					{
						FalsePositive = _scoreKeepersSettings.reroutingFalsePositiveScore,
						FalseNegative = _scoreKeepersSettings.reroutingFalseNegativeScore,
						TruePositive = _scoreKeepersSettings.reroutingTruePositiveScore,
						TrueNegative = _scoreKeepersSettings.reroutingTrueNegativeScore,
						

						TruePositiveWithAITruePositive= _scoreKeepersSettings.reroutingTruePositiveWithAITruePositiveScore,
						TruePositiveDespiteAIFalseNegative= _scoreKeepersSettings.reroutingTruePositiveDespiteAIFalseNegativeScore,
						TrueNegativeWithAITrueNegative= _scoreKeepersSettings.reroutingTrueNegativeWithAITrueNegativeScore,
						TrueNegativeDespiteAIFalsePositive= _scoreKeepersSettings.reroutingTrueNegativeDespiteAIFalsePositiveScore,
						FalsePositiveDueToAIFalsePositive= _scoreKeepersSettings.reroutingFalsePositiveDueToAIFalsePositiveScore,
						FalsePositiveDespiteAITrueNegative= _scoreKeepersSettings.reroutingFalsePositiveDespiteAITrueNegativeScore,
						FalseNegativeDespiteAITruePositive= _scoreKeepersSettings.reroutingFalseNegativeDespiteAITruePositiveScore,
						FalseNegativeDueToAIFalseNegative= _scoreKeepersSettings.reroutingFalseNegativeDueToAIFalseNegativeScore,
						
						
					},
					TargetDetectionScores = new
					{
						FalsePositive = _scoreKeepersSettings.targetDetectionFalsePositiveScore,
						FalseNegative = _scoreKeepersSettings.targetDetectionFalseNegativeScore,
						TruePositive = _scoreKeepersSettings.targetDetectionTruePositiveScore,
						TrueNegative = _scoreKeepersSettings.targetDetectionTrueNegativeScore,
					}
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