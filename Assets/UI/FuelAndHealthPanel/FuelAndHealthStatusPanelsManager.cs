using System;
using System.Collections.Generic;
using Modules.FuelAndHealth.Channels.ScriptableObjects;
using Modules.FuelAndHealth.Settings.ScriptableObjects;
using Multiplayer;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UnityEngine;
using static HelperScripts.Enums;
using static Modules.FuelAndHealth.Settings.ScriptableObjects.FuelSettingsSO;

namespace UI.FuelAndHealthPanel
{
	public class FuelAndHealthStatusPanelsManager:MonoBehaviour
	{
		private FuelSettingsSO _fuelSettings;
		private UavEventChannelSO _uavCreatedEventChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		
		private UavFuelConditionEventChannelSO _uavFuelConditionChangedEventChannel;
		private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		private UavFloatEventChannelSO _uavFuelLevelChangedEventChannel;
		private UavStringEventChannelSO _uavHealthButtonClickedEventChannel;

		private UavsManager _uavsManager;

		Dictionary<Uav,FuelAndHealthStatusPanelController> _uavToPanelController = new ();

		public void Start()
		{
			GetReferencesFromGameManager();
			CreatePanelsForUavs();
			SubscribeToChannels();
			ClearPanels();

            if (AppNetPortal.Instance.IsMultiplayerMode())	  
				GameplayNetworkCallsHandler.Instance.FixLeak_NetworkEventHandler += OnFixLeakNetworkEventHandler;           
        }

		public void OnDestroy()
		{
			UnsubscribeFromChannels();
        }

		private void CreatePanelsForUavs()
		{
			foreach (var uav in GameManager.Instance.uavsManager.uavs)
			{
				OnUavCreated(uav);
			}
		}

		private void ClearPanels()
		{
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}

		private void GetReferencesFromGameManager()
		{
			_fuelSettings = GameManager.Instance.settingsDatabase.fuelSettings;
			_uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;

			_uavFuelConditionChangedEventChannel = GameManager.Instance.channelsDatabase.fuelChannels.uavFuelConditionChangedEventChannel;
			_uavFuelLevelChangedEventChannel = GameManager.Instance.channelsDatabase.fuelChannels.uavFuelLevelChangedEventChannel;
			_uavHealthButtonClickedEventChannel = GameManager.Instance.channelsDatabase.fuelChannels.uavHealthButtonClickedEventChannel;

			_uavsManager = GameManager.Instance.uavsManager;
		}

		private void OnUavDestroyed(Uav uav)
		{
			if (_uavToPanelController.ContainsKey(uav))
			{
				Destroy(_uavToPanelController[uav].gameObject);
				_uavToPanelController.Remove(uav);
			}
		}

		private void OnUavCreated(Uav uav)
		{
			var panelController = AddFuelAndHealthPanel(uav);
			_uavToPanelController.Add(uav,panelController);
		}
		
		private FuelAndHealthStatusPanelController AddFuelAndHealthPanel( Uav uav)
		{
			var panel = Instantiate(GameManager.Instance.prefabsDatabase.fuelAndHealthPanelPrefab, transform);
			panel.name="UAV " + uav.uavName + " Fuel And Health Panel";
			var panelController= panel.GetComponent<FuelAndHealthStatusPanelController>();
			panelController.Initialize(uav,this);
			return panelController;
		}
		
		private void OnUavConditionChanged(Uav uav, UavCondition uavCondition)
		{
			if (_uavToPanelController.ContainsKey(uav)) 
				_uavToPanelController[uav].OnUavConditionChanged(uavCondition);
		}

		private void OnUavFuelLevelChanged(Uav uav, float fuelLevel)
		{
			if (_uavToPanelController.ContainsKey(uav)) 
				_uavToPanelController[uav].OnFuelLevelChanged(fuelLevel);
		}

		private void OnUavFuelConditionChanged(Uav uav, FuelCondition fuelCondition)
		{
			if (_uavToPanelController.ContainsKey(uav))
				_uavToPanelController[uav].OnFuelConditionChanged(fuelCondition);
		}
		public void OnHealthButtonClicked(Uav uav, string text)
		{
			if (_uavHealthButtonClickedEventChannel != null) 
				_uavHealthButtonClickedEventChannel.RaiseEvent(uav, text);
		}

		private void SubscribeToChannels()
		{
			if (_uavCreatedEventChannel != null) 
				_uavCreatedEventChannel.Subscribe(OnUavCreated);

			if (_uavDestroyedEventChannel != null) 
				_uavDestroyedEventChannel.Subscribe(OnUavDestroyed);

			if (_uavFuelConditionChangedEventChannel != null)
				_uavFuelConditionChangedEventChannel.Subscribe(OnUavFuelConditionChanged);

			if (_uavFuelLevelChangedEventChannel != null)
				_uavFuelLevelChangedEventChannel.Subscribe(OnUavFuelLevelChanged);

			if (_uavConditionChangedEventChannel != null)
				_uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);

		}

		private void UnsubscribeFromChannels()
		{
			if (_uavCreatedEventChannel != null) 
				_uavCreatedEventChannel.Unsubscribe(OnUavCreated);

			if (_uavDestroyedEventChannel != null) 
				_uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);

			if (_uavFuelConditionChangedEventChannel != null)
				_uavFuelConditionChangedEventChannel.Unsubscribe(OnUavFuelConditionChanged);

			if (_uavFuelLevelChangedEventChannel != null)
				_uavFuelLevelChangedEventChannel.Unsubscribe(OnUavFuelLevelChanged);

			if (_uavConditionChangedEventChannel != null)
				_uavConditionChangedEventChannel.Unsubscribe(OnUavConditionChanged);

		}
        private void OnFixLeakNetworkEventHandler(object sender, FixLeakEventArgs e)
        {
            Uav uav = _uavsManager.GetUAVAgainstId(e.UavId);

			OnHealthButtonClicked(uav, e.ButtonText);
        }
    }
}