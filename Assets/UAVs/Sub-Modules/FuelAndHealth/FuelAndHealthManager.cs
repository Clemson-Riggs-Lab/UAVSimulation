using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.FuelAndHealth;
using UAVs.Sub_Modules.Fuel;
using Unity.VisualScripting;
using UnityEngine;

namespace UAVs.Sub_Modules.FuelAndHealth
{
	public class FuelAndHealthManager:MonoBehaviour
	{
		private FuelAndHealthSettingsSO _fuelAndHealthSettings;
		private UavEventChannelSO uavCreatedEventChannel;
		private UavEventChannelSO uavDestroyedEventChannel;
		
		Dictionary<Uav,FuelAndHealthController> _uavToFuelAndHealthControllerDictionary = new Dictionary<Uav, FuelAndHealthController>();
		
		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			ClearPanels();
			CreatePanelsForUavs();
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
		
		private void OnUavDestroyed(Uav uav)
		{
			
			if (_uavToFuelAndHealthControllerDictionary.ContainsKey(uav))
			{
				Destroy(_uavToFuelAndHealthControllerDictionary[uav].gameObject);
				_uavToFuelAndHealthControllerDictionary.Remove(uav);
			}

			//remove panel
			var panel = transform.Find("UAV " +uav.uavName + " Fuel And Health Panel");
			if(panel != null)
				Destroy(panel.gameObject);
		}

		private void OnUavCreated(Uav uav)
		{
			var fuelAndHealthController = uav.AddComponent<FuelAndHealthController>();
			fuelAndHealthController.Initialize(uav);
			AddFuelAndHealthPanel(fuelAndHealthController,uav);
			_uavToFuelAndHealthControllerDictionary.Add(uav,fuelAndHealthController);
		}

		public IEnumerator StartFuelAndHealthControllers(float simulationStartTime)
		{
			yield return new WaitForSeconds( simulationStartTime- Time.time);
			
			foreach (var uav in _uavToFuelAndHealthControllerDictionary.Keys)
			{
				_uavToFuelAndHealthControllerDictionary[uav].Begin();
			}

			yield return null;
		}

		private void GetReferencesFromGameManager()
		{
			_fuelAndHealthSettings = GameManager.Instance.settingsDatabase.uavSettingsDatabase.fuelAndHealthSettings;
			uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
		}

		private void AddFuelAndHealthPanel(FuelAndHealthController fuelAndHealthController, Uav uav)
		{
			var panel = Instantiate(GameManager.Instance.prefabsDatabase.fuelAndHealthPanelPrefab, transform);
			panel.name="UAV " + uav.uavName + " Fuel And Health Panel";
			var panelController= panel.GetComponent<StatusPanelController>();
			panelController.Initialize(fuelAndHealthController);
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