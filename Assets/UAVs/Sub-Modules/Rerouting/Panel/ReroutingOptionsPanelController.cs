using System.Collections.Generic;
using ScriptableObjects.UAVs.Navigation.Rerouting;
using TMPro;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace UAVs.Sub_Modules.Rerouting.Panel
{
	public class ReroutingOptionsPanelController : MonoBehaviour
	{

		public TextMeshProUGUI headerText;
		public Outline outline;
		private Uav uav;
		private  ReroutingPanelsContainerController _containerController;
		private ReroutingManager _manager;

		public Button closeButton;

		
		private ReroutingSettingsSO reroutingSettings;
		private GameObject panelRowPrefab;
		
		private Dictionary<int, ReroutingOptionRowController> panelRows = new ();

		private void ClearRows()
		{
			//find children with name containing "Row"
			foreach (Transform child in transform)
			{
				if (child.name.Contains("Row"))
				{
					Destroy(child.gameObject);
				}
			}
		}

		public void Initialize(Uav uav, ReroutingPanelsContainerController containerController, ReroutingManager manager)
		{
			this.uav= uav;
			_containerController = containerController;
			_manager = manager;
			
			reroutingSettings = GameManager.Instance.settingsDatabase.reroutingSettings;
			panelRowPrefab = GameManager.Instance.prefabsDatabase.reroutingPrefabsDatabase.reroutingOptionRowPrefab;
			closeButton.onClick.AddListener( OnCloseButtonClicked);
			ClearRows();
			
			
			headerText.text = "Rerouting options for " + uav.uavName;

			//create a row from prefab for each option in range
			for( int i = 0; i < reroutingSettings.numberOfReroutingOptionsToPresent; i++)
			{
				var iterator = i;
				var row = Instantiate(panelRowPrefab, transform);
				var rowController = row.GetComponent<ReroutingOptionRowController>();
				
				rowController.cancelButton.onClick.AddListener(() => OnCancelButtonClicked(iterator));
				rowController.previewButton.onClick.AddListener(() => OnPreviewButtonClicked(iterator));
				rowController.confirmButton.onClick.AddListener(() => OnConfirmButtonClicked(iterator));
				panelRows.Add(i, rowController);
				
			}

		}

		private void OnCloseButtonClicked()
		{
			_manager.RemoveUavPanelAndOptions(uav);//remove panel and stop rerouting manager from calculating rerouting options for this path.
		}
		
		private void OnPreviewButtonClicked(int optionNumber)
		{
			_manager.PreviewPath(uav, optionNumber);
			HighlightRow(optionNumber);
			_containerController.HighlightPanel(uav);
		}
		private void OnCancelButtonClicked(int optionNumber)
		{
			UnhighlightAllRows();
			this.Highlight(false);
		}
		private void OnConfirmButtonClicked(int optionNumber)
		{
			_manager.RerouteUav(uav,optionNumber);
			
		}

		private void HighlightRow(int optionNumber)
		{
			UnhighlightAllRows();
			panelRows[optionNumber].Highlight(true);	//highlight the selected row
		}

		private void UnhighlightAllRows()
		{
			foreach(var row in panelRows)			

			{
				row.Value.Highlight(false);
			}
		}
		

	

		public void Highlight(bool b)
		{
			outline.enabled = b;
		}
	}
}