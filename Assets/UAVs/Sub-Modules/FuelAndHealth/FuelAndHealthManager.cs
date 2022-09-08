using System;
using System.Collections.Generic;
using DefaultNamespace;
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
		private Dictionary<Uav , FuelAndHealthController> _fuelAndHealthControllersDictionary = new ();
		private Dictionary<Uav , StatusPanelController> _fuelAndHealthPanelControllersDictionary = new ();
		private FuelAndHealthSettingsSO.FuelStatusAndHealthBarPositioning _fuelStatusAndHealthBarPositioning = FuelStatusAndHealthBarVisibleInSeparatePanel;
		void Start()
		{
			_uavsManager= GameManager.Instance.uavsManager;
			_fuelAndHealthSettings = GameManager.Instance.settingsDatabase.uavSettings.fuelAndHealthSettings;
			_fuelStatusAndHealthBarPositioning = _fuelAndHealthSettings.fuelStatusAndHealthBarPositioning;
		}

		public void Initialize()
		{
			var uavs= _uavsManager.uavs;
			foreach (var uav in uavs)
			{
				var fuelAndHealthController = uav.AddComponent<FuelAndHealthController>() as FuelAndHealthController;
				_fuelAndHealthControllersDictionary.Add(uav, fuelAndHealthController);
				fuelAndHealthController.Initialize();
				AddFuelAndHealthPanel(fuelAndHealthController,uav);
				fuelAndHealthController.Begin();
			}
		}
		
		private void AddFuelAndHealthPanel(FuelAndHealthController fuelAndHealthController, Uav uav)
		{
			switch (_fuelStatusAndHealthBarPositioning)
			{
				case FuelStatusAndHealthBarVisibleInSeparatePanel:
				{
					var panel = Instantiate(GameManager.Instance.prefabsDatabase.fuelAndHealthPanelPrefab,
						CanvasManager.Instance.fuelAndHealthPanel.transform);
					var panelController= panel.GetComponent<StatusPanelController>();
					panelController.Initialize(fuelAndHealthController);
					_fuelAndHealthPanelControllersDictionary.Add(uav, panelController);
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

	}
}