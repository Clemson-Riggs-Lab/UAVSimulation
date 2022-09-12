using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using HelperScripts;
using ScriptableObjects.UAVs.Navigation.Rerouting;
using UAVs.Sub_Modules.Rerouting.Panel;
using UI;
using UnityEngine;

namespace UAVs.Sub_Modules.Rerouting
{
	public class ReroutingPanelsContainerController:MonoBehaviour
	{
		private ReroutingSettingsSO reroutingSettings;
		private ReroutingManager reroutingManager;
		
		private GameObject panelPrefab;
		private GameObject panelRowPrefab;
		private OrderedDictionary<Uav,ReroutingOptionsPanelController> uav_ReroutingOptionsPanelControllerDictionary = new ();
		// ordered dictionary
		


		private void Start()
		{
			GetReferencesFromGameManager();
			ClearPanels();
			SetGridDimensions();
			
		}

		private void SetGridDimensions()
		{
			var flexibleGridLayout = GetComponent<FlexibleGridLayout>();
			if (flexibleGridLayout != null)
			{
				flexibleGridLayout.fitType = FlexibleGridLayout.FitType.FixedBoth;
				flexibleGridLayout.rows = reroutingSettings.numberOfReroutingOptionsPanelsGridRows;
				flexibleGridLayout.columns = reroutingSettings.numberOfReroutingOptionsPanelsGridColumns;
			}

		}

		private void ClearPanels()
		{
			foreach(Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}

		private void GetReferencesFromGameManager()
		{
			reroutingSettings = GameManager.Instance.settingsDatabase.reroutingSettings;
			panelPrefab = GameManager.Instance.prefabsDatabase.reroutingPrefabsDatabase.reroutingOptionsPanelPrefab;
			panelRowPrefab = GameManager.Instance.prefabsDatabase.reroutingPrefabsDatabase.reroutingOptionRowPrefab;
		}

		
		
		public void AddPanel (Uav uav)
		{
			if (uav_ReroutingOptionsPanelControllerDictionary.ContainsKey(uav))
			{
				HighlightPanel(uav);
				return; // panel already exists
			}
			if(uav_ReroutingOptionsPanelControllerDictionary.Count >= reroutingSettings.maximumNumberOfReroutingOptionsPanels) //if we have more panels than the limit, we need to remove one before adding the new one.
			{ 
				reroutingManager.RemoveUavPanelAndOptions(uav_ReroutingOptionsPanelControllerDictionary.Keys.First());
			}
			var panelController = Instantiate(panelPrefab,transform).GetComponent<ReroutingOptionsPanelController>();
			panelController.Initialize(uav,this,reroutingManager);
			
			switch (reroutingSettings.newPanelPosition) //setting the position of the newly added panel 
			{
				case ReroutingSettingsSO.NewPanelPosition.PlaceAtTheBeginning:
					panelController.transform.SetAsFirstSibling();
					break;
				case ReroutingSettingsSO.NewPanelPosition.PlaceAtTheEnd:
				default:
					panelController.transform.SetAsLastSibling();
					break;
			}
			
			uav_ReroutingOptionsPanelControllerDictionary[uav]=panelController;
			HighlightPanel(uav);
		}
		
		public void RemovePanel(Uav uav)
		{
			if(uav_ReroutingOptionsPanelControllerDictionary.ContainsKey(uav))
			{
				Destroy(uav_ReroutingOptionsPanelControllerDictionary[uav].gameObject);
				uav_ReroutingOptionsPanelControllerDictionary.Remove(uav);
			}
		}
		
		public void HighlightPanel (Uav uav)
		{
			if( uav_ReroutingOptionsPanelControllerDictionary.ContainsKey(uav) )
			{
				foreach (var panel in uav_ReroutingOptionsPanelControllerDictionary.Values) //Unhighlight all panels
				{
					panel.Highlight(false);
				}
				uav_ReroutingOptionsPanelControllerDictionary[uav].Highlight(true); //then highlight the one we want
			}
		}

		public void Initialize(ReroutingManager rm)
		{
			reroutingManager = rm;
		}
	}
}