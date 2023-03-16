using HelperScripts;
using TMPro;
using UI.ReroutingPanel.Settings.ScriptableObjects;
using UnityEngine;

namespace UI.ReroutingPanel
{
	public class ReroutingPanelManager: MonoBehaviour
	{
		public TextMeshProUGUI headerText;
		private ReroutingPanelSettingsSO _reroutingPanelSettings;

		private void Start()
		{

			GetReferencesFromGameManager();
			headerText.text = _reroutingPanelSettings.headerText;
			headerText.color = ColorHelper.StringToColor(_reroutingPanelSettings.headerTextColor);
		}

		private void GetReferencesFromGameManager()
		{
			_reroutingPanelSettings = GameManager.Instance.settingsDatabase.reroutingPanelSettings;
		}
	}
}