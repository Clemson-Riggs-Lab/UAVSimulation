using System;
using System.Collections.Generic;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.FuelAndHealth;
using UAVs.Sub_Modules.Fuel;
using Unity.VisualScripting;
using UnityEngine;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO.FuelStatusAndHealthBarPositioning;

namespace UAVs.Sub_Modules.FuelAndHealth
{
	public class FuelAndHealthManager:MonoBehaviour
	{
		private UavsManager _uavsManager;
		private FuelAndHealthSettingsSO _fuelAndHealthSettings;
		private FuelAndHealthSettingsSO.FuelStatusAndHealthBarPositioning _fuelStatusAndHealthBarPositioning = FuelStatusAndHealthBarVisibleInSeparatePanel;

		private UavEventChannelSO uavCreatedEventChannel;
		private UavEventChannelSO uavDestroyedEventChannel;
		
		void Start()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			ClearPanels();
			_fuelStatusAndHealthBarPositioning = _fuelAndHealthSettings.fuelStatusAndHealthBarPositioning;
		}

		private void ClearPanels()
		{
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}


		private void OnUavDestroyed(Uav uav)
		{

			//remove panel
			var panel = transform.Find("UAV " +uav.uavName + " Fuel And Health Panel");
			if(panel != null)
				Destroy(panel.gameObject);
		}

		private void OnUavCreated(Uav uav)
		{
			var fuelAndHealthController = uav.AddComponent<FuelAndHealthController>() as FuelAndHealthController;
			fuelAndHealthController.Initialize();
			AddFuelAndHealthPanel(fuelAndHealthController,uav);
			fuelAndHealthController.Begin();
		}

		private void GetReferencesFromGameManager()
		{
			_uavsManager= GameManager.Instance.uavsManager;
			_fuelAndHealthSettings = GameManager.Instance.settingsDatabase.uavSettings.fuelAndHealthSettings;
			uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
		}

		private void AddFuelAndHealthPanel(FuelAndHealthController fuelAndHealthController, Uav uav)
		{
			switch (_fuelStatusAndHealthBarPositioning)
			{
				case FuelStatusAndHealthBarVisibleInSeparatePanel:
				{
					var panel = Instantiate(GameManager.Instance.prefabsDatabase.fuelAndHealthPanelPrefab, transform);
					panel.name="UAV " + uav.uavName + " Fuel And Health Panel";
					var panelController= panel.GetComponent<StatusPanelController>();
					panelController.Initialize(fuelAndHealthController);
					break;
				}
				
				case FuelStatusOnlyVisibleInSeparatePanel:
				case HealthBarOnlyVisibleInSeparatePanel:
				case FuelStatusAndHealthBarVisibleInCameraWindow:
				case FuelStatusOnlyVisibleInCameraWindow:
				case HealthBarOnlyVisibleInCameraWindow:
				default:
					throw  new NotImplementedException();//TODO
			}
		}


		private void OnDestroy()
		{
			UnsubscribeFromChannels();
		}

		private void SubscribeToChannels()
		{
			if (uavCreatedEventChannel != null)
			{
				uavCreatedEventChannel.Subscribe(OnUavCreated);
			}
			
			if (uavDestroyedEventChannel != null)
			{
				uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
			}
			
		}
		
		private void UnsubscribeFromChannels()
		{
			if (uavCreatedEventChannel != null)
			{
				uavCreatedEventChannel.Unsubscribe(OnUavCreated);
			}
			
			if (uavDestroyedEventChannel != null)
			{
				uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
			}
		}
	}
}