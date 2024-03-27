using System;
using System.Linq;
using HelperScripts;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.Navigation.Submodules.Rerouting;
using Multiplayer;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UI.ReroutingPanel.Settings.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums;
using static HelperScripts.Enums.UavCondition;

namespace UI.ReroutingPanel
{
	public class ReroutingPanelsContainerController:MonoBehaviour
	{
		private ReroutingPanelSettingsSO _reroutingPanelSettings;
		private ReroutingManager _reroutingManager;
		private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		private UavEventChannelSO _reroutingOptionsRequestedChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		private UavPathEventChannelSO _uavArrivedAtDestinationEventChannel;
		private GameObject _panelPrefab;
		private OrderedDictionary<Uav,ReroutingOptionsPanelController> _uavReroutingOptionsPanelControllerDictionary = new ();
		


		private void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			ClearPanels();
			SetGridDimensions();
			
		}

		private void SubscribeToChannels()
		{
			if (_uavConditionChangedEventChannel != null)
				_uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);
			
			if (_reroutingOptionsRequestedChannel != null)
				_reroutingOptionsRequestedChannel.Subscribe(AddPanel);
			
			if (_uavArrivedAtDestinationEventChannel != null)
				_uavArrivedAtDestinationEventChannel.Subscribe(OnUavStartedNewPath);
			
			if (_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Subscribe(RemovePanel);
				
		}

		private void OnUavStartedNewPath(Uav uav, Path path)
		{
			if (_uavReroutingOptionsPanelControllerDictionary.ContainsKey(uav))
			{
				_uavReroutingOptionsPanelControllerDictionary[uav].UnhighlightAllRows();
			}
		}

		private void OnUavConditionChanged(Uav uav, UavCondition condition)
		{
			if(!_uavReroutingOptionsPanelControllerDictionary.ContainsKey(uav)) return; // no rerouting panel open for this uav
			
			var closePanel = condition switch
			{
				Enabled => false,
				Hidden => _reroutingPanelSettings.closePanelsForHiddenUavs,
				Finished => _reroutingPanelSettings.closePanelsForFinishedUavs,
				Lost => _reroutingPanelSettings.closePanelsForLostUavs,
				_ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
			};
			
			if(closePanel)
				RemovePanel(uav);

		}

		private void SetGridDimensions()
		{
			var flexibleGridLayout = GetComponent<FlexibleGridLayout>();
			if (flexibleGridLayout != null)
			{
				flexibleGridLayout.fitType = FlexibleGridLayout.FitType.FixedBoth;
				flexibleGridLayout.rows = _reroutingPanelSettings.numberOfReroutingOptionsPanelsGridRows;
				flexibleGridLayout.columns = _reroutingPanelSettings.numberOfReroutingOptionsPanelsGridColumns;
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
			_reroutingManager = GameManager.Instance.reroutingManager;
			_reroutingPanelSettings = GameManager.Instance.settingsDatabase.reroutingPanelSettings;
			_panelPrefab = GameManager.Instance.prefabsDatabase.reroutingPrefabsDatabase.reroutingOptionsPanelPrefab;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
			_reroutingOptionsRequestedChannel = GameManager.Instance.channelsDatabase.navigationChannels.reroutingOptionsRequestedChannel;
			_uavArrivedAtDestinationEventChannel = GameManager.Instance.channelsDatabase.navigationChannels.uavArrivedAtDestinationEventChannel;
		}
		
		public void AddPanel (Uav uav)
		{
			if (_uavReroutingOptionsPanelControllerDictionary.ContainsKey(uav))
            {
                HighlightPanel(uav);
				return; // panel already exists
			}
			if(_uavReroutingOptionsPanelControllerDictionary.Count >= _reroutingPanelSettings.maximumNumberOfReroutingOptionsPanels) //if we have more panels than the limit, we need to remove one before adding the new one.
			{
                var panelToRemove = _uavReroutingOptionsPanelControllerDictionary.First();

                if (AppNetPortal.Instance.IsMultiplayerMode())
                    GameplayNetworkCallsHandler.Instance.ReroutePanelCloseServerRpc(AppNetPortal.Instance.LocalClientId, panelToRemove.Key.id);

                RemovePanel(panelToRemove.Key);

			}
			var panelController = Instantiate(_panelPrefab,transform).GetComponent<ReroutingOptionsPanelController>();
			panelController.Initialize(uav,this, GameManager.Instance.uavsManager);
			panelController.transform.localScale = Vector3.zero;
			switch (_reroutingPanelSettings.newPanelPosition) //setting the position of the newly added panel 
			{
				case ReroutingPanelSettingsSO.NewPanelPosition.PlaceAtTheBeginning:
					panelController.transform.SetAsFirstSibling();
					break;
				case ReroutingPanelSettingsSO.NewPanelPosition.PlaceAtTheEnd:
				default:
					panelController.transform.SetAsLastSibling();
					break;
			}
			panelController.transform.localScale = Vector3.one;
			_uavReroutingOptionsPanelControllerDictionary[uav]=panelController;

            HighlightPanel(uav);
		}
		
		public void DisablePanel(Uav uav)
		{
            if (_uavReroutingOptionsPanelControllerDictionary.ContainsKey(uav))
            {
                _uavReroutingOptionsPanelControllerDictionary[uav].gameObject.SetActive(false);
            }
        }
		public void RemovePanel(Uav uav)
		{
			if(_uavReroutingOptionsPanelControllerDictionary.ContainsKey(uav))
			{
				_reroutingManager.RemoveUavPanelAndOptions(uav);
				Destroy(_uavReroutingOptionsPanelControllerDictionary[uav].gameObject);
				_uavReroutingOptionsPanelControllerDictionary.Remove(uav);
			}
		}
		
		public void HighlightPanel (Uav uav)
		{
			if( _uavReroutingOptionsPanelControllerDictionary.ContainsKey(uav) )
			{
				foreach (var panel in _uavReroutingOptionsPanelControllerDictionary.Values) //Unhighlight all panels
				{
					panel.Highlight(false);
					panel.UnhighlightAllRows();
				}
				_uavReroutingOptionsPanelControllerDictionary[uav].Highlight(true); //then highlight the one we want
			}
		}


		private void OnDestroy()
		{
			UnsubscribeFromChannels();
		}

		private void UnsubscribeFromChannels()
		{
			if (_reroutingOptionsRequestedChannel != null)
				_reroutingOptionsRequestedChannel.Unsubscribe(AddPanel);
			
			if (_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Unsubscribe(RemovePanel);
			
			if (_uavConditionChangedEventChannel != null)
				_uavConditionChangedEventChannel.Unsubscribe(OnUavConditionChanged);
			
			if (_uavArrivedAtDestinationEventChannel != null)
				_uavArrivedAtDestinationEventChannel.Unsubscribe(OnUavStartedNewPath);
		}
	}
}