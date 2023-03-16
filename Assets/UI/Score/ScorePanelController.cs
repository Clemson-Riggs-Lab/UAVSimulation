using Modules.Navigation.Submodules.Rerouting.Settings.ScriptableObjects;
using Modules.ScoreKeeper.Channels.ScriptableObjects;
using Modules.ScoreKeeper.Settings.ScriptableObjects;
using TMPro;
using UnityEngine;
using static Modules.ScoreKeeper.ScoreManager.ScoreEventType;

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
		
		[SerializeField] private TextMeshProUGUI fpReroutingCountText; 
		[SerializeField] private TextMeshProUGUI fnReroutingCountText;
		[SerializeField] private TextMeshProUGUI tnReroutingCountText;
		[SerializeField] private TextMeshProUGUI tpReroutingCountText;
		
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
			
			fpReroutingCountText.text = "0";
			fnReroutingCountText.text = "0";
			tnReroutingCountText.text = "0";
			tpReroutingCountText.text = "0";
			
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
			
			fpReroutingCountText.text = score.scoreCounts[ReroutingFalsePositive].ToString();
			fnReroutingCountText.text = score.scoreCounts[ReroutingFalseNegative].ToString();
			tnReroutingCountText.text = score.scoreCounts[ReroutingTrueNegative].ToString();
			tpReroutingCountText.text = score.scoreCounts[ReroutingTruePositive].ToString();
			
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