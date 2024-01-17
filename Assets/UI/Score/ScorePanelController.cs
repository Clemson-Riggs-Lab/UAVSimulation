using Modules.Navigation.Submodules.Rerouting.Settings.ScriptableObjects;
using Modules.ScoreKeeper.Channels.ScriptableObjects;
using Modules.ScoreKeeper.Settings.ScriptableObjects;
using TMPro;
using UnityEngine;
using static Modules.ScoreKeeper.ScoreManager.AIRelatedScoreEventType;
using static Modules.ScoreKeeper.ScoreManager.ScoreEventType;
using static HelperScripts.ArrayExtensions;

namespace UI.Score
{
	public class ScorePanelController:MonoBehaviour
	{
		private ScoreKeeperUpdatedEventChannelSO _scoreKeeperUpdatedEventChannel;
		private ScoreKeepersSettingsSO _scoreKeepersSettings;
		private ReroutingSettingsSO _reroutingSettings;
		[SerializeField] private GameObject container;
		[SerializeField] private TextMeshProUGUI fpReroutingScoreText;
		[SerializeField] private TextMeshProUGUI fnReroutingScoreText;
		[SerializeField] private TextMeshProUGUI tnReroutingScoreText;
		[SerializeField] private TextMeshProUGUI tpReroutingScoreText;
		
		
		[SerializeField] private TextMeshProUGUI reroutingTruePositiveWithAITruePositiveScoreText;
		[SerializeField] private TextMeshProUGUI reroutingTruePositiveDespiteAIFalseNegativeScoreText;
		[SerializeField] private TextMeshProUGUI reroutingTrueNegativeWithAITrueNegativeScoreText; 
		[SerializeField] private TextMeshProUGUI reroutingTrueNegativeDespiteAIFalsePositiveScoreText;
		[SerializeField] private TextMeshProUGUI reroutingFalsePositiveDueToAIFalsePositiveScoreText;
		[SerializeField] private TextMeshProUGUI reroutingFalsePositiveDespiteAITrueNegativeScoreText;
		[SerializeField] private TextMeshProUGUI reroutingFalseNegativeDueToAIFalseNegativeScoreText;
		[SerializeField] private TextMeshProUGUI reroutingFalseNegativeDespiteAITruePositiveScoreText;


		[SerializeField] private TextMeshProUGUI fpReroutingCountText; 
		[SerializeField] private TextMeshProUGUI fnReroutingCountText;
		[SerializeField] private TextMeshProUGUI tnReroutingCountText;
		[SerializeField] private TextMeshProUGUI tpReroutingCountText;
		
		
		[SerializeField] private TextMeshProUGUI reroutingTruePositiveWithAITruePositiveCountText;
		[SerializeField] private TextMeshProUGUI reroutingTruePositiveDespiteAIFalseNegativeCountText;
		[SerializeField] private TextMeshProUGUI reroutingTrueNegativeWithAITrueNegativeCountText; 
		[SerializeField] private TextMeshProUGUI reroutingTrueNegativeDespiteAIFalsePositiveCountText;
		[SerializeField] private TextMeshProUGUI reroutingFalsePositiveDueToAIFalsePositiveCountText;
		[SerializeField] private TextMeshProUGUI reroutingFalsePositiveDespiteAITrueNegativeCountText;
		[SerializeField] private TextMeshProUGUI reroutingFalseNegativeDueToAIFalseNegativeCountText;
		[SerializeField] private TextMeshProUGUI reroutingFalseNegativeDespiteAITruePositiveCountText;
		
		
		[SerializeField] private TextMeshProUGUI fpTargetDetectionScoreText;
		[SerializeField] private TextMeshProUGUI fnTargetDetectionScoreText;
		[SerializeField] private TextMeshProUGUI tnTargetDetectionScoreText;
		[SerializeField] private TextMeshProUGUI tpTargetDetectionScoreText;
		
		[SerializeField] private TextMeshProUGUI fpTargetDetectionCountText;
		[SerializeField] private TextMeshProUGUI fnTargetDetectionCountText;
		[SerializeField] private TextMeshProUGUI tnTargetDetectionCountText;
		[SerializeField] private TextMeshProUGUI tpTargetDetectionCountText;
		
		[SerializeField] private TextMeshProUGUI fpAIPercentageText;
		[SerializeField] private TextMeshProUGUI fnAIPercentageText;
		[SerializeField] private TextMeshProUGUI tnAIPercentageText;
		[SerializeField] private TextMeshProUGUI tpAIPercentageText;
		[SerializeField] private TextMeshProUGUI BadRRAIPercentageText;
		

		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			if(!_scoreKeepersSettings.showScorePanel)
				container.SetActive(false);
			
			ClearText();
		}

		private void ClearText()
		{
			fpReroutingScoreText.text = "0";
			fnReroutingScoreText.text = "0";
			tnReroutingScoreText.text = "0";
			tpReroutingScoreText.text = "0";
			
			reroutingTruePositiveWithAITruePositiveScoreText.text = "0";
			reroutingTruePositiveDespiteAIFalseNegativeScoreText.text = "0";
			reroutingTrueNegativeWithAITrueNegativeScoreText.text = "0";
			reroutingTrueNegativeDespiteAIFalsePositiveScoreText.text = "0";
			reroutingFalsePositiveDueToAIFalsePositiveScoreText.text = "0";
			reroutingFalsePositiveDespiteAITrueNegativeScoreText.text = "0";
			reroutingFalseNegativeDueToAIFalseNegativeScoreText.text = "0";
			reroutingFalseNegativeDespiteAITruePositiveScoreText.text = "0";
			
			fpReroutingCountText.text = "0";
			fnReroutingCountText.text = "0";
			tnReroutingCountText.text = "0";
			tpReroutingCountText.text = "0";
			
			reroutingTruePositiveWithAITruePositiveCountText.text = "0";
			reroutingTruePositiveDespiteAIFalseNegativeCountText.text = "0";
			reroutingTrueNegativeWithAITrueNegativeCountText.text = "0";
			reroutingTrueNegativeDespiteAIFalsePositiveCountText.text = "0";
			reroutingFalsePositiveDueToAIFalsePositiveCountText.text = "0";
			reroutingFalsePositiveDespiteAITrueNegativeCountText.text = "0";
			reroutingFalseNegativeDueToAIFalseNegativeCountText.text = "0";
			reroutingFalseNegativeDespiteAITruePositiveCountText.text = "0";
			
			fpTargetDetectionScoreText.text = "0";
			fnTargetDetectionScoreText.text = "0";
			tnTargetDetectionScoreText.text = "0";
			tpTargetDetectionScoreText.text = "0";
			
			fpTargetDetectionCountText.text = "0";
			fnTargetDetectionCountText.text = "0";
			tnTargetDetectionCountText.text = "0";
			tpTargetDetectionCountText.text = "0";
			
			fpAIPercentageText.text = _reroutingSettings.oneClickRerouteFalsePositiveProbability*100 + "%";
			fnAIPercentageText.text = _reroutingSettings.oneClickRerouteFalseNegativeProbability*100 + "%";
			tnAIPercentageText.text = 100-_reroutingSettings.oneClickRerouteFalsePositiveProbability*100 + "%";
			tpAIPercentageText.text = 100-_reroutingSettings.oneClickRerouteFalseNegativeProbability*100 + "%";
			BadRRAIPercentageText.text = _reroutingSettings.probabilityOfUnsuccessfulOneClickReroute*100 + "%";
		}

		private void SubscribeToChannels()
		{
			if (_scoreKeeperUpdatedEventChannel != null)
				_scoreKeeperUpdatedEventChannel.Subscribe(OnScoreKeeperUpdated);
		}

		private void OnScoreKeeperUpdated(Modules.ScoreKeeper.Score score)
		{
			fpReroutingScoreText.text = score.scoreValues[ReroutingFalsePositive].ToString();
			fnReroutingScoreText.text = score.scoreValues[ReroutingFalseNegative].ToString();
			tnReroutingScoreText.text = score.scoreValues[ReroutingTrueNegative].ToString();
			tpReroutingScoreText.text = score.scoreValues[ReroutingTruePositive].ToString();
			
			reroutingTrueNegativeWithAITrueNegativeScoreText.text = score.aIRelatedScoreValues[ReroutingTrueNegativeWithAITrueNegative].ToString();
			reroutingTrueNegativeDespiteAIFalsePositiveScoreText.text = score.aIRelatedScoreValues[ReroutingTrueNegativeDespiteAIFalsePositive].ToString();
			reroutingTruePositiveWithAITruePositiveScoreText.text = score.aIRelatedScoreValues[ReroutingTruePositiveWithAITruePositive].ToString();
			reroutingTruePositiveDespiteAIFalseNegativeScoreText.text = score.aIRelatedScoreValues[ReroutingTruePositiveDespiteAIFalseNegative].ToString();
			reroutingFalsePositiveDueToAIFalsePositiveScoreText.text = score.aIRelatedScoreValues[ReroutingFalsePositiveDueToAIFalsePositive].ToString();
			reroutingFalsePositiveDespiteAITrueNegativeScoreText.text = score.aIRelatedScoreValues[ReroutingFalsePositiveDespiteAITrueNegative].ToString();
			reroutingFalseNegativeDueToAIFalseNegativeScoreText.text = score.aIRelatedScoreValues[ReroutingFalseNegativeDueToAIFalseNegative].ToString();
			reroutingFalseNegativeDespiteAITruePositiveScoreText.text = score.aIRelatedScoreValues[ReroutingFalseNegativeDespiteAITruePositive].ToString();
			

			fpReroutingCountText.text = score.scoreCounts[ReroutingFalsePositive].ToString();
			fnReroutingCountText.text = score.scoreCounts[ReroutingFalseNegative].ToString();
			tnReroutingCountText.text = score.scoreCounts[ReroutingTrueNegative].ToString();
			tpReroutingCountText.text = score.scoreCounts[ReroutingTruePositive].ToString();
			
			reroutingTrueNegativeWithAITrueNegativeCountText.text = score.aIRelatedScoreCounts[ReroutingTrueNegativeWithAITrueNegative].ToCustomString();
			reroutingTrueNegativeDespiteAIFalsePositiveCountText.text = score.aIRelatedScoreCounts[ReroutingTrueNegativeDespiteAIFalsePositive].ToCustomString();
			reroutingTruePositiveWithAITruePositiveCountText.text = score.aIRelatedScoreCounts[ReroutingTruePositiveWithAITruePositive].ToCustomString();
			reroutingTruePositiveDespiteAIFalseNegativeCountText.text = score.aIRelatedScoreCounts[ReroutingTruePositiveDespiteAIFalseNegative].ToCustomString();
			reroutingFalsePositiveDueToAIFalsePositiveCountText.text = score.aIRelatedScoreCounts[ReroutingFalsePositiveDueToAIFalsePositive].ToCustomString();
			reroutingFalsePositiveDespiteAITrueNegativeCountText.text = score.aIRelatedScoreCounts[ReroutingFalsePositiveDespiteAITrueNegative].ToCustomString();
			reroutingFalseNegativeDueToAIFalseNegativeCountText.text = score.aIRelatedScoreCounts[ReroutingFalseNegativeDueToAIFalseNegative].ToCustomString();
			reroutingFalseNegativeDespiteAITruePositiveCountText.text = score.aIRelatedScoreCounts[ReroutingFalseNegativeDespiteAITruePositive].ToCustomString();
			
			fpTargetDetectionScoreText.text  = score.scoreValues[TargetDetectionFalsePositive].ToString();
			fnTargetDetectionScoreText.text  = score.scoreValues[TargetDetectionFalseNegative].ToString();
			tnTargetDetectionScoreText.text  = score.scoreValues[TargetDetectionTrueNegative].ToString();
			tpTargetDetectionScoreText.text  = score.scoreValues[TargetDetectionTruePositive].ToString();
			
			fpTargetDetectionCountText.text = score.scoreCounts[TargetDetectionFalsePositive].ToString();
			fnTargetDetectionCountText.text = score.scoreCounts[TargetDetectionFalseNegative].ToString();
			tnTargetDetectionCountText.text = score.scoreCounts[TargetDetectionTrueNegative].ToString();
			tpTargetDetectionCountText.text = score.scoreCounts[TargetDetectionTruePositive].ToString();
			
			fpAIPercentageText.text = _reroutingSettings.oneClickRerouteFalsePositiveProbability*100 + "%";
			fnAIPercentageText.text = _reroutingSettings.oneClickRerouteFalseNegativeProbability*100 + "%";
			tnAIPercentageText.text = 100-_reroutingSettings.oneClickRerouteFalsePositiveProbability*100 + "%";
			tpAIPercentageText.text = 100-_reroutingSettings.oneClickRerouteFalseNegativeProbability*100 + "%";
			BadRRAIPercentageText.text = _reroutingSettings.probabilityOfUnsuccessfulOneClickReroute*100 + "%";
			

		}

		private void GetReferencesFromGameManager()
		{
			_scoreKeepersSettings = GameManager.Instance.settingsDatabase.scoreKeepersSettings;
			_scoreKeeperUpdatedEventChannel = GameManager.Instance.channelsDatabase.scoreKeeperUpdatedEventChannel;
			_reroutingSettings = GameManager.Instance.settingsDatabase.reroutingSettings;
		}
	}
}