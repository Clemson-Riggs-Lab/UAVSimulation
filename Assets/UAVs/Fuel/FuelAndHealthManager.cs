using System;
using System.Collections.Generic;
using DefaultNamespace;
using UAVs.Fuel.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using static UAVs.Fuel.ScriptableObjects.FuelAndHealthSettingsSO.FuelStatusAndHealthBarPositioning;

namespace UAVs.Fuel
{
	public class FuelAndHealthManager:MonoBehaviour
	{
		private UavsManager _uavsManager;
		private FuelAndHealthSettingsSO _fuelAndHealthSettings;
		Dictionary<int , FuelAndHealthController> _fuelAndHealthControllersDictionary = new ();
		Dictionary<int , StatusPanelController> _fuelAndHealthPanelControllersDictionary = new ();
		private FuelAndHealthSettingsSO.FuelStatusAndHealthBarPositioning _fuelStatusAndHealthBarPositioning = FuelStatusAndHealthBarVisibleInSeparatePanel;
		void Start()
		{
			_uavsManager= GameManager.Instance.uavsManager;
			_fuelAndHealthSettings = SettingsManager.Instance.fuelAndHealthSettings;
			_fuelStatusAndHealthBarPositioning = _fuelAndHealthSettings.fuelStatusAndHealthBarPositioning;
		}

		public void Initialize()
		{
			var uavs= _uavsManager.Uavs;
			foreach (var uav in uavs)
			{
				var fuelAndHealthController = uav.AddComponent<FuelAndHealthController>() as FuelAndHealthController;
				_fuelAndHealthControllersDictionary.Add(uav.ID, fuelAndHealthController);
				fuelAndHealthController.Initialize();
				AddFuelAndHealthPanel(fuelAndHealthController,uav.ID);
				fuelAndHealthController.Begin();
			}
		}
		
		private void AddFuelAndHealthPanel(FuelAndHealthController fuelAndHealthController, int uavID)
		{
			switch (_fuelStatusAndHealthBarPositioning)
			{
				case FuelStatusAndHealthBarVisibleInSeparatePanel:
				{
					var panel = Instantiate(PrefabsManager.Instance.fuelAndHealthPanelPrefab,
						CanvasManager.Instance.fuelAndHealthPanel.transform);
					var panelController= panel.GetComponent<StatusPanelController>();
					panelController.Initialize(fuelAndHealthController);
					_fuelAndHealthPanelControllersDictionary.Add(uavID, panelController);
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