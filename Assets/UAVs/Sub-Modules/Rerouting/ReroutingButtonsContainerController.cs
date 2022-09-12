using System;
using ScriptableObjects.EventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UAVs.Sub_Modules.Rerouting
{
	public class ReroutingButtonsContainerController:MonoBehaviour
	{
		private  ReroutingManager reroutingManager;
		private GameObject buttonPrefab;

		private void Start()
		{
			GetReferencesFromGameManager();
			ClearButtons();

		}
		public void Initialize(ReroutingManager rm)
		{
			this.reroutingManager = rm;
		}
		
		private void ClearButtons()
		{
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}

		private void GetReferencesFromGameManager()
		{
			buttonPrefab = GameManager.Instance.prefabsDatabase.reroutingPrefabsDatabase.rerouteButtonPrefab;
		}

		public void CreateButton(Uav uav)
		{
			var button=Instantiate(buttonPrefab, transform);
			button.GetComponentInChildren<TextMeshProUGUI>().text = uav.uavName;
			button.name = "UAV "+uav.uavName+ "Reroute Options Request Button";
			button.GetComponent<Button>().onClick.AddListener(() => { reroutingManager.OnRerouteOptionsRequested(uav);} );
		}
		
		public void RemoveButton(Uav uav)
		{
			var button = transform.Find("UAV "+uav.uavName+ "Reroute Options Request Button");
			if(button != null)
				Destroy(button.gameObject);
		}
		
	
		
	}

}
