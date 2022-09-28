using System;
using System.Collections.Generic;
using TMPro;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UI.ReroutingPanel.Settings.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using static HelperScripts.Enums;
using static HelperScripts.Enums.UavCondition;

namespace UI.ReroutingPanel
{
	public class ReroutingButtonsContainerController:MonoBehaviour
	{
		private GameObject _buttonPrefab;
		private ReroutingPanelSettingsSO _reroutingPanelSettings;
		private UavEventChannelSO _uavCreatedEventChannel;
		private UavEventChannelSO	_uavDestroyedEventChannel;
		private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		private UavEventChannelSO _reroutingOptionsRequestedChannel;
		private Dictionary<Uav, Button> _uavsToButtonsDictionary = new();

		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			ClearButtons();
			CreateButtons();

		}

		private void CreateButtons()
		{
			foreach (var uav in GameManager.Instance.uavsManager.uavs)
			{
				CreateButton(uav);
			}
		}

		private void SubscribeToChannels()
		{
			if (_uavConditionChangedEventChannel != null)
				_uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);
			
			if (_uavCreatedEventChannel != null)
				_uavCreatedEventChannel.Subscribe(CreateButton);

			if (_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Subscribe(RemoveButton);
		}

		private void OnUavConditionChanged(Uav uav, UavCondition condition)
		{
			if(!_uavsToButtonsDictionary.ContainsKey(uav)) return;
			
			var disabled = condition switch
			{
				Enabled => false,
				Hidden => _reroutingPanelSettings.disableButtonsForHiddenUavs,
				Finished => _reroutingPanelSettings.disableButtonsForFinishedUavs,
				Lost => _reroutingPanelSettings.disableButtonsForLostUavs,
				_ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
			};
			
			var hidden = condition switch
			{
				Enabled => false,
				Hidden => _reroutingPanelSettings.hideButtonsForHiddenUavs,
				Finished => _reroutingPanelSettings.hideButtonsForFinishedUavs,
				Lost => _reroutingPanelSettings.hideButtonsForLostUavs,
				_ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
			};
			
			var button = _uavsToButtonsDictionary[uav];
			
			button.interactable = !disabled;


			if (hidden)
			{
				button.transform.localScale = Vector3.zero;
				button.GetComponent<LayoutElement>().ignoreLayout = true;
			}
			else
			{
				button.transform.localScale = Vector3.one;
				button.GetComponent<LayoutElement>().ignoreLayout = false;
			}
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
			_buttonPrefab = GameManager.Instance.prefabsDatabase.reroutingPrefabsDatabase.rerouteButtonPrefab;
			_reroutingPanelSettings= GameManager.Instance.settingsDatabase.reroutingPanelSettings;
			_uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
			_reroutingOptionsRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.reroutingOptionsRequestedChannel;
		}

		public void CreateButton(Uav uav)
		{
			var button=Instantiate(_buttonPrefab, transform);
			button.GetComponentInChildren<TextMeshProUGUI>().text = uav.uavName;
			button.name = "UAV "+uav.uavName+ "Reroute Options Request Button";
			button.GetComponent<Button>().onClick.AddListener(() => { _reroutingOptionsRequestedChannel.RaiseEvent(uav);} );
			_uavsToButtonsDictionary[uav]= button.GetComponent<Button>();
		}
		
		public void RemoveButton(Uav uav)
		{
			if (_uavsToButtonsDictionary.ContainsKey(uav))
			{
				Destroy(_uavsToButtonsDictionary[uav].gameObject);
				_uavsToButtonsDictionary.Remove(uav);
			}

		}


		private void OnDisable()
		{
			UnsubscribeFromChannels();
		}

		private void UnsubscribeFromChannels()
		{
			if (_uavConditionChangedEventChannel != null)
				_uavConditionChangedEventChannel.Unsubscribe(OnUavConditionChanged);
			
			if (_uavCreatedEventChannel != null)
				_uavCreatedEventChannel.Unsubscribe(CreateButton);

			if (_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Unsubscribe(RemoveButton);
		}
	}

}
