using System.Collections.Generic;
using Modules.ScoreKeeper.Settings;
using static Modules.ScoreKeeper.ScoreManager;
using static Modules.ScoreKeeper.ScoreManager.AIRelatedScoreEventType;
using static Modules.ScoreKeeper.ScoreManager.ScoreEventType;

namespace Modules.ScoreKeeper
{
	public class Score
	{
		public Dictionary<ScoreEventType, int> scoreCounts = new Dictionary<ScoreEventType, int>
		{
			{ ReroutingFalsePositive, 0 },
			{ ReroutingFalseNegative, 0 },
			{ ReroutingTruePositive, 0 },
			{ ReroutingTrueNegative, 0 },

			{ TargetDetectionFalsePositive, 0 },
			{ TargetDetectionFalseNegative, 0 },
			{ TargetDetectionTruePositive, 0 },
			{ TargetDetectionTrueNegative, 0 }
		};
		
		public Dictionary<ScoreEventType, float> scoreValues = new Dictionary<ScoreEventType, float>
		{
			{ ReroutingFalsePositive, 0 },
			{ ReroutingFalseNegative, 0 },
			{ ReroutingTruePositive, 0 },
			{ ReroutingTrueNegative, 0 },

			{ TargetDetectionFalsePositive, 0 },
			{ TargetDetectionFalseNegative, 0 },
			{ TargetDetectionTruePositive, 0 },
			{ TargetDetectionTrueNegative, 0 }
		};
		
		public Dictionary<AIRelatedScoreEventType, int> aIRelatedScoreCounts = new Dictionary<AIRelatedScoreEventType, int>
		{
			{ReroutingTruePositiveWithAITruePositive, 0 },
			{ReroutingTruePositiveDespiteAIFalseNegative, 0 },
			{ReroutingTrueNegativeWithAITrueNegative, 0 },
			{ReroutingTrueNegativeDespiteAIFalsePositive, 0 },
			{ReroutingFalsePositiveDueToAIFalsePositive, 0 },
			{ReroutingFalsePositiveDespiteAITrueNegative, 0 },
			{ReroutingFalseNegativeDespiteAITruePositive, 0 },
			{ReroutingFalseNegativeDueToAIFalseNegative, 0 },
			
		};
		public Dictionary<AIRelatedScoreEventType, float> aIRelatedScoreValues = new Dictionary<AIRelatedScoreEventType, float>
		{
			{ReroutingTruePositiveWithAITruePositive, 0 },
			{ReroutingTruePositiveDespiteAIFalseNegative, 0 },
			{ReroutingTrueNegativeWithAITrueNegative, 0 },
			{ReroutingTrueNegativeDespiteAIFalsePositive, 0 },
			{ReroutingFalsePositiveDueToAIFalsePositive, 0 },
			{ReroutingFalsePositiveDespiteAITrueNegative, 0 },
			{ReroutingFalseNegativeDespiteAITruePositive, 0 },
			{ReroutingFalseNegativeDueToAIFalseNegative, 0 },
		};
		
		
		public Dictionary<int,float> reroutingResponseTimePathIdDictionary=new ();
		public Dictionary<int,float> targetDetectionResponseTimePathIdDictionary=new ();

		
		

	}
}