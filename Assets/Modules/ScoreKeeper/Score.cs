using System.Collections.Generic;
using Modules.ScoreKeeper.Settings;
using static Modules.ScoreKeeper.ScoreManager.ScoreEventType;

namespace Modules.ScoreKeeper
{
	public class Score
	{
		public Dictionary<ScoreManager.ScoreEventType, int> scoreCounts = new Dictionary<ScoreManager.ScoreEventType, int>
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
		
		public Dictionary<ScoreManager.ScoreEventType, float> scoreValues = new Dictionary<ScoreManager.ScoreEventType, float>
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

		
		

	}
}