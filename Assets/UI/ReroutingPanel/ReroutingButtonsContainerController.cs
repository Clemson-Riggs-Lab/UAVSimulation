using System;
using System.Collections.Generic;
using HelperScripts;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Submodules.Rerouting.Settings.ScriptableObjects;
using TMPro;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UI.ReroutingPanel.Settings.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using static HelperScripts.Enums;
using static HelperScripts.Enums.ConditionalState;
using static HelperScripts.Enums.UavCondition;
using Random = System.Random;

namespace UI.ReroutingPanel
{
	public class ReroutingButtonsContainerController:MonoBehaviour
	{
		private GameObject _buttonPrefab;
		private ReroutingPanelSettingsSO _reroutingPanelSettings;
		private ReroutingSettingsSO _reroutingSettings;
		private UavPathEventChannelSO _uavReroutedEventChannel;
		private UavEventChannelSO _uavCreatedEventChannel;
		private UavEventChannelSO	_uavDestroyedEventChannel;
		private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		private UavEventChannelSO _reroutingOptionsRequestedChannel;
		private UavEventChannelSO _oneClickReroutingRequestedChannel;
		private Dictionary<Uav, GameObject> _uavsToButtonContainersDictionary = new();
		private Random _OneClickReroutingRandomGenerator;
		private int _falsePositiveQueuCounter=0;
		
		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			ClearButtons();
			CreateButtons();

			gameObject.GetComponent<FlexibleGridLayout>().ignoreZeroScaleObjects = _reroutingPanelSettings.onlyAccountForVisibleUavsButtonsInLayout;

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
			
			if(_uavReroutedEventChannel != null)
				_uavReroutedEventChannel.Subscribe(OnUavRerouted);
		}

		private void OnUavRerouted(Uav uav, Path arg1)
		{
			if (_reroutingSettings.oneClickRerouteEnabled == false) return;
			
			if(!_uavsToButtonContainersDictionary.ContainsKey(uav)) return;
			var button = _uavsToButtonContainersDictionary[uav].transform.Find("OneClickRerouteButton").GetComponent<Button>();

			button.interactable = false;
			button.image.color= new Color(1,1,1,0);
			button.transform.localScale = Vector3.zero;
			
		}

		private void OnUavConditionChanged(Uav uav, UavCondition condition)
		{
			if(!_uavsToButtonContainersDictionary.ContainsKey(uav)) return;
			
			var disabled = condition switch
			{
				
				Hidden => _reroutingPanelSettings.disableButtonsForHiddenUavs,
				Lost => _reroutingPanelSettings.disableButtonsForLostUavs,
				EnabledForReroutingOnly => false,
				EnabledForTargetDetectionOnly =>  _reroutingPanelSettings.disableButtonsForHiddenUavs,
				EnabledForTargetDetectionAndRerouting => false,
				_ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
			};
			
			var hidden = condition switch
			{
				
				Hidden => _reroutingPanelSettings.hideButtonsForHiddenUavs,
				Lost => _reroutingPanelSettings.hideButtonsForLostUavs,
				EnabledForReroutingOnly => false,
				EnabledForTargetDetectionOnly =>  _reroutingPanelSettings.hideButtonsForHiddenUavs,
				EnabledForTargetDetectionAndRerouting => false,
				_ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
			};
			
			var button = _uavsToButtonContainersDictionary[uav];
			
			button.transform.Find("ReroutingOptionsButton").GetComponent<Button>().interactable = !disabled;


			if (hidden)
			{
				if (_reroutingPanelSettings.keepHiddenButtonsPositions)
				{
					button.transform.localScale = Vector3.zero;
					button.GetComponent<LayoutElement>().ignoreLayout = false;
				}
				else
				{
					button.transform.localScale = Vector3.zero;
					button.GetComponent<LayoutElement>().ignoreLayout = true;
				}
			}
			else
			{
				button.transform.localScale = Vector3.one;
				button.GetComponent<LayoutElement>().ignoreLayout = false;
				SetOneClickRerouteButton(uav,button.transform.Find("OneClickRerouteButton").GetComponent<Button>());
			}
		}

		private void SetOneClickRerouteButton(Uav uav, Button button)
		{
			
			
			if (_reroutingSettings.oneClickRerouteEnabled == false)
			{
				uav.currentPath.OneClickRerouteButtonCondition = OFF;
				return;
			}
			
			
			button.transform.localScale = Vector3.one;
			button.interactable = false;
			//set button color to transparent
			button.image.color= new Color(1,1,1,1);
			if(uav.currentPath.isReroutePath)
			{
				uav.currentPath.OneClickRerouteButtonCondition = uav.currentPath.headingToNFZ ? FN : TN; // if it is a reroute path, we do not want to show the button, so if it is heading to NFZ, so we set the button state to false negative, otherwise we set it to true negative
				return;
			}
			
			if (!uav.currentPath.headingToNFZ)
			{
				var falsePositive= _OneClickReroutingRandomGenerator.NextDouble() < _reroutingSettings.oneClickRerouteFalsePositiveProbability;
				
				if( falsePositive)
					_falsePositiveQueuCounter++;
				
				if (_falsePositiveQueuCounter > 0)
				{ 
					var maxDistance = Vector3.Distance(uav.transform.position, uav.currentPath.destinationWayPoint.transform.position)*1.3f; // we want to give a false positive that is realistic, i.e., the uav destination is not too far away from the NFZ
					if (Physics.Raycast(uav.transform.position, uav.currentPath.destinationWayPoint.transform.position, out var hit, maxDistance, 1 << LayerMask.NameToLayer("NFZ")))
					{
						button.interactable = true;
						button.image.color = new Color(1, 0, 0, 1);
						uav.currentPath.OneClickRerouteButtonCondition = FP;
						_falsePositiveQueuCounter--;
						Debug.Log("queue remaining" + _falsePositiveQueuCounter);
						return;
					}
				}
				
				uav.currentPath.OneClickRerouteButtonCondition = TN;
			}
			else
			{
				var falseNegative= _OneClickReroutingRandomGenerator.NextDouble() < _reroutingSettings.oneClickRerouteFalseNegativeProbability;
				if (falseNegative)
				{
					uav.currentPath.OneClickRerouteButtonCondition = FN;
				}
				else
				{
					button.interactable = true;
					button.image.color= new Color(1,0,0,1);
					uav.currentPath.OneClickRerouteButtonCondition = TP;
				}
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
			_reroutingSettings = GameManager.Instance.settingsDatabase.reroutingSettings;
			_uavReroutedEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavReroutedEventChannel;
			_uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
			_reroutingOptionsRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.reroutingOptionsRequestedChannel;
			_oneClickReroutingRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.oneClickReroutingRequestedChannel;
			_OneClickReroutingRandomGenerator = new Random( _reroutingSettings.oneClickRerouteFpFnRandomSeed);
		}

		public void CreateButton(Uav uav)
		{
			var button=Instantiate(_buttonPrefab, transform);
			button.transform.Find("ReroutingOptionsButton").GetComponentInChildren<TextMeshProUGUI>().text = uav.uavName;
			button.name = "UAV "+uav.uavName+ "Reroute Options Request Button";

			button.transform.Find("ReroutingOptionsButton").GetComponent<Button>().onClick.AddListener(() => { _reroutingOptionsRequestedChannel.RaiseEvent(uav);} );
			button.transform.Find("OneClickRerouteButton").GetComponent<Button>().onClick.AddListener(() => { OneClickReroute(uav,button);} );

			button.transform.Find("OneClickRerouteButton").gameObject.SetActive(_reroutingSettings.oneClickRerouteEnabled );
			if(_reroutingPanelSettings.colorButtonsLikeUav)
				button.transform.Find("ReroutingOptionsButton").GetComponent<Image>().color = uav.uavColor;

			_uavsToButtonContainersDictionary[uav]= button;

		}

		private void OneClickReroute(Uav uav, GameObject button)
		{
			_oneClickReroutingRequestedChannel.RaiseEvent(uav);

		}

		public void RemoveButton(Uav uav)
		{
			if (_uavsToButtonContainersDictionary.ContainsKey(uav))
			{
				Destroy(_uavsToButtonContainersDictionary[uav].gameObject);
				_uavsToButtonContainersDictionary.Remove(uav);
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
			
			if (_uavReroutedEventChannel != null)
				_uavReroutedEventChannel.Unsubscribe(OnUavRerouted);
		}
	}

}
