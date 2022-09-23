using System.Collections;
using System.Collections.Generic;
using Modules.Fuel;
using Modules.FuelAndHealth.Settings.ScriptableObjects;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using static HelperScripts.Enums;
using static HelperScripts.Enums.UavCondition;

namespace Modules.FuelAndHealth
{
	public class FuelManager:MonoBehaviour
	{
		private FuelSettingsSO _fuelSettings;
		private UavEventChannelSO _uavCreatedEventChannel;
		private UavEventChannelSO _uavDestroyedEventChannel;
		private UavStringEventChannelSO _uavHealthButtonClickedEventChannel;
		private UavConditionEventChannelSO _uavConditionChangedEventChannel;
		
		Dictionary<Uav,FuelController> _uavToFuelControllerDictionary = new ();
		
		public void Initialize()
		{
			GetReferencesFromGameManager();
			SubscribeToChannels();
			AddFuelControllersToUavs();
			
			var fuelLogHandler= gameObject.GetOrAddComponent<FuelLogHandler>();
			fuelLogHandler.Initialize();
		}
		private void AddFuelControllersToUavs()
		{
			foreach (var uav in GameManager.Instance.uavsManager.uavs)
			{
				OnUavCreated(uav);
			}
		}
		private void OnUavCreated(Uav uav)
		{
			var fuelAndHealthController = uav.GetOrAddComponent<FuelController>();
			fuelAndHealthController.Initialize(uav);
			_uavToFuelControllerDictionary.Add(uav,fuelAndHealthController);
		}
		private void OnUavDestroyed(Uav uav)
		{
			if (!_uavToFuelControllerDictionary.ContainsKey(uav)) return; //does not exist
			Destroy(_uavToFuelControllerDictionary[uav].gameObject); //destroy the controller
			_uavToFuelControllerDictionary.Remove(uav); //remove from dictionary
		}
		

		public IEnumerator StartFuelControllers(float simulationStartTime)
		{
			yield return new WaitForSeconds( simulationStartTime- Time.time);
			
			foreach (var uav in _uavToFuelControllerDictionary.Keys)
			{
				_uavToFuelControllerDictionary[uav].Begin();
			}
			yield return null;
		}

		private void GetReferencesFromGameManager()
		{
			_fuelSettings = GameManager.Instance.settingsDatabase.fuelSettings;
			_uavCreatedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
			_uavDestroyedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
			_uavHealthButtonClickedEventChannel = GameManager.Instance.channelsDatabase.fuelChannels.uavHealthButtonClickedEventChannel;
			_uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
		}
		
		private void OnDestroy()
		{
			UnsubscribeFromChannels();
		}

		private void SubscribeToChannels()
		{
			if (_uavCreatedEventChannel != null)
				_uavCreatedEventChannel.Subscribe(OnUavCreated);
			
			
			if (_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Subscribe(OnUavDestroyed);
			
			if (_uavHealthButtonClickedEventChannel != null)
				_uavHealthButtonClickedEventChannel.Subscribe(OnUavHealthButtonClicked);
			
			if (_uavConditionChangedEventChannel != null)
				_uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);
			
			
		}

		private void OnUavConditionChanged(Uav uav, UavCondition condition)
		{
			if (!_uavToFuelControllerDictionary.ContainsKey(uav)) return;
			
			if (condition is Finished or Lost)
			{
				_uavToFuelControllerDictionary[uav].isConsumingFuel=false;
			}
		}

		private void OnUavHealthButtonClicked(Uav uav, string buttonText)
		{
			if (!_uavToFuelControllerDictionary.ContainsKey(uav)) return; //does not exist
			_uavToFuelControllerDictionary[uav].OnHealthButtonClicked(buttonText);
		}

		private void UnsubscribeFromChannels()
		{
			if (_uavCreatedEventChannel != null)
				_uavCreatedEventChannel.Unsubscribe(OnUavCreated);

			if (_uavDestroyedEventChannel != null)
				_uavDestroyedEventChannel.Unsubscribe(OnUavDestroyed);
			
			if (_uavHealthButtonClickedEventChannel != null)
				_uavHealthButtonClickedEventChannel.Unsubscribe(OnUavHealthButtonClicked);
		}
	}
}