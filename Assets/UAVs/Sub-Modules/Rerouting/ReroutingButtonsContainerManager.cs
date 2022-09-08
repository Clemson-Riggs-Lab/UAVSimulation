using System;
using ScriptableObjects.EventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UAVs.Sub_Modules.Rerouting
{
	public class ReroutingButtonsContainerManager:MonoBehaviour
	{
		private UavEventChannelSO uavCreatedEventChannel;
		private UavEventChannelSO uavDestroyedEventChannel;

		private UavEventChannelSO reroutingOptionsRequestedChannel;

		private GameObject buttonPrefab;

		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			ClearButtons();

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
			uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			buttonPrefab = GameManager.Instance.prefabsDatabase.rerouteButtonPrefab;
			reroutingOptionsRequestedChannel = GameManager.Instance.channelsDatabase.uavChannels.reroutingOptionsRequestedChannel;
		}

		private void CreateButton(Uav uav)
		{
			var button=Instantiate(buttonPrefab, transform);
			button.GetComponentInChildren<TextMeshProUGUI>().text = "UAV "+(uav.ID+1).ToString();
			button.name = "UAV "+uav.ID+ "Reroute Options Request Button";
			button.GetComponent<Button>().onClick.AddListener(() => { reroutingOptionsRequestedChannel.RaiseEvent(uav);} );
		}
		
		private void RemoveButton(Uav uav)
		{
			var button = transform.Find("UAV "+uav.ID+ "Reroute Options Request Button");
			if(button != null)
				Destroy(button.gameObject);
		}

		private void OnDisable()
		{
			UnsubscribeFromChannels();
		}

		
		private void SubscribeToChannels()
		{
			if (uavCreatedEventChannel != null)
				uavCreatedEventChannel.Subscribe(CreateButton);
			if(uavDestroyedEventChannel != null)
				uavDestroyedEventChannel.Subscribe(RemoveButton);
		}
		private void UnsubscribeFromChannels()
		{
			if (uavCreatedEventChannel != null)
				uavCreatedEventChannel.Unsubscribe(CreateButton);
			if(uavDestroyedEventChannel != null)
				uavDestroyedEventChannel.Unsubscribe(RemoveButton);
		}
	}

}
