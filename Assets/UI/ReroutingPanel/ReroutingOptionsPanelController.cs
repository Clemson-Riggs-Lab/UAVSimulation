using System.Collections.Generic;
using Modules.Navigation;
using Modules.Navigation.Submodules.Rerouting;
using Multiplayer;
using TMPro;
using UAVs;
using UI.ReroutingPanel.Settings.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ReroutingPanel
{
	public class ReroutingOptionsPanelController : MonoBehaviour
	{

		public TextMeshProUGUI headerText;
		public Outline outline;
		private Uav _uav;
		private  ReroutingPanelsContainerController _containerController;
		private ReroutingManager _manager;

		public Button closeButton;

		
		private ReroutingPanelSettingsSO _reroutingPanelSettings;
		private GameObject _panelRowPrefab;
		
		private Dictionary<int, ReroutingOptionRowController> _panelRows = new ();

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
			this._uav= uav;
			_containerController = containerController;
			_manager = manager;
			
			_reroutingPanelSettings = GameManager.Instance.settingsDatabase.reroutingPanelSettings;
			_panelRowPrefab = GameManager.Instance.prefabsDatabase.reroutingPrefabsDatabase.reroutingOptionRowPrefab;
			closeButton.onClick.AddListener( OnCloseButtonClicked);
			ClearRows();
			
			
			headerText.text = "Rerouting options for " + uav.uavName;

			//create a row from prefab for each option in range
			for( int i = 0; i < _reroutingPanelSettings.numberOfReroutingOptionsToPresent; i++)
			{
				var iterator = i;
				var row = Instantiate(_panelRowPrefab, transform);
				var rowController = row.GetComponent<ReroutingOptionRowController>();
				rowController.routeLabel.text = "Option #" + (i + 1);
				rowController.cancelButton.onClick.AddListener(() => OnCancelButtonClicked(iterator));
				rowController.previewButton.onClick.AddListener(() => OnPreviewButtonClicked(iterator));
				rowController.confirmButton.onClick.AddListener(() => OnConfirmButtonClicked(iterator));
				_panelRows.Add(i, rowController);
				
			}

		}

		private void OnCloseButtonClicked()
		{
			_manager.RemoveUavPanelAndOptions(_uav);//remove panel and stop rerouting manager from calculating rerouting options for this path.
			_containerController.RemovePanel(_uav);

			if (AppNetPortal.Instance.IsMultiplayerMode())
				GameplayNetworkCallsHandler.Instance.ReroutePanelCloseServerRpc(AppNetPortal.Instance.LocalClientId, _uav.id);
		}
		
		private void OnPreviewButtonClicked(int optionNumber)
		{
			_manager.PreviewPath(_uav, optionNumber);
			_containerController.HighlightPanel(_uav);
			HighlightRow(optionNumber);
		}
		private void OnCancelButtonClicked(int optionNumber)
		{
			UnhighlightAllRows();
			this.Highlight(false);
			_manager.PreviewPath(_uav, -1);
		}
		private void OnConfirmButtonClicked(int optionIndex)
		{
            if (AppNetPortal.Instance.IsMultiplayerMode())
			{
                GameplayNetworkCallsHandler.Instance.ReroutePanelCloseServerRpc(AppNetPortal.Instance.LocalClientId, _uav.id);

                GameplayNetworkCallsHandler.Instance.ReroutingUAVOnServerRpc(AppNetPortal.Instance.IsThisHost ? CallerType.Host : CallerType.Client, _uav.id, optionIndex, _manager.LastReroutOptLsOrderBase.ToString());
			}
			else
                _manager.RerouteUav(_uav, optionIndex);

			_containerController.RemovePanel(_uav);
		}

		private void HighlightRow(int optionNumber)
		{
			UnhighlightAllRows();
			_panelRows[optionNumber].Highlight(true);	//highlight the selected row
		}

		public void UnhighlightAllRows()
		{
			foreach(var row in _panelRows.Values)			

			{
				row.Highlight(false);
			}
		}

		public void Highlight(bool b)
		{
			outline.enabled = b;
		}
	}
}