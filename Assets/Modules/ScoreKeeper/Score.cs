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
		
		public Dictionary<AIRelatedScoreEventType, List<int>> aIRelatedScoreCounts = new Dictionary<AIRelatedScoreEventType, List<int>>
		{
			{ReroutingTruePositiveWithAITruePositive, new List<int>() },
			{ReroutingTruePositiveDespiteAIFalseNegative, new List<int>() },
			{ReroutingTrueNegativeWithAITrueNegative, new List<int>() },
			{ReroutingTrueNegativeDespiteAIFalsePositive, new List<int>() },
			{ReroutingFalsePositiveDueToAIFalsePositive, new List<int>() },
			{ReroutingFalsePositiveDespiteAITrueNegative, new List<int>() },
			{ReroutingFalseNegativeDespiteAITruePositive, new List<int>() },
			{ReroutingFalseNegativeDueToAIFalseNegative, new List<int>() },
			
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