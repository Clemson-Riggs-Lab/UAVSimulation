using HelperScripts;
using TMPro;
using UI.ReroutingPanel.Settings.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ReroutingPanel
{
	public class ReroutingPanelManager: MonoBehaviour
	{
		public TextMeshProUGUI headerText;
		private ReroutingPanelSettingsSO _reroutingPanelSettings;
		public GameObject leftPanel;
		private void Start()
		{

			GetReferencesFromGameManager();
			headerText.text = _reroutingPanelSettings.headerText;
			headerText.color = ColorHelper.StringToColor(_reroutingPanelSettings.headerTextColor);
			
			leftPanel.GetComponent<Image>().color = ColorHelper.StringToColor(headerText.text == "Primary Task" ? _reroutingPanelSettings.headerTextColor : "Black");
		}

		private void GetReferencesFromGameManager()
		{
			_reroutingPanelSettings = GameManager.Instance.settingsDatabase.reroutingPanelSettings;
		}
	}
}