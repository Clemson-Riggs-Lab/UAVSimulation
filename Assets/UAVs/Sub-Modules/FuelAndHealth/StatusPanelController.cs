using System;
using System.Collections;
using HelperScripts;
using ScriptableObjects.UAVs.FuelAndHealth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO;

namespace UAVs.Sub_Modules.Fuel
{
    public class StatusPanelController : MonoBehaviour //TODO change to FuelAndHealthStatusPanelController
    {
        // Start is called before the first frame update
        public TextMeshProUGUI idText;
        public TextMeshProUGUI healthButtonText;
        public Button healthButton;
        public Slider fuelSlider;
        public Image fuelSliderImage;
        public Image healthButtonImage;

        private FuelAndHealthController _fuelAndHealthController;
        private FuelAndHealthSettingsSO _fuelAndHealthSettings;
        private FuelAndHealthPanelSettings fuelAndHealthPanelSettings;

        public FuelAndHealthPanelSettings FuelAndHealthPanelSettings
        {
            get => fuelAndHealthPanelSettings;
            set
            {
                fuelAndHealthPanelSettings = value;
                UpdateUIElements();
            }
        }

        private void UpdateUIElements()
        {
            healthButtonText.text = fuelAndHealthPanelSettings.fuelAndHealthPanelVisualSettings.healthButtonText;
            healthButton.interactable = fuelAndHealthPanelSettings.healthButtonInteractable;
            healthButtonText.color = ColorHelper.StringToColor(fuelAndHealthPanelSettings.fuelAndHealthPanelVisualSettings.healthButtonTextColor);
            
            fuelSliderImage.color = ColorHelper.StringToColor(fuelAndHealthPanelSettings.fuelAndHealthPanelVisualSettings.fuelSliderColor);
            healthButtonImage.color = ColorHelper.StringToColor(fuelAndHealthPanelSettings.fuelAndHealthPanelVisualSettings.healthButtonColor);
            if (fuelAndHealthPanelSettings.healthButtonInteractable == true)
            {
                StartCoroutine(ButtonInteractableDurationTimer());
            }
        }

        private IEnumerator ButtonInteractableDurationTimer()
        {
            yield return new WaitForSeconds(_fuelAndHealthSettings.fuelLeakButtonInteractionDurationBeforeFatalLeak);
            healthButton.interactable = false;
        }

        public void Initialize(FuelAndHealthController fuelAndHealthController)
        {
            _fuelAndHealthSettings= GameManager.Instance.settingsDatabase.uavSettingsDatabase.fuelAndHealthSettings;
            
            _fuelAndHealthController = fuelAndHealthController;
            idText.text = _fuelAndHealthController.uav.uavName;
            fuelSlider.maxValue = _fuelAndHealthController.GetStartingFuelLevel();

            // listen to events from fuelAndHealthController
            _fuelAndHealthController.FuelLevelChanged += UpdateFuelLevel;
            _fuelAndHealthController.HealthConditionChanged += UpdateHealthControllerCondition;
            _fuelAndHealthController.FuelConditionChanged += UpdateFuelCondition;
            healthButton.onClick.AddListener(() => _fuelAndHealthController.OnHealthButtonClicked(healthButtonText.text));
            
            UpdateHealthControllerCondition();
        }
        
        public void UpdateFuelLevel()
        {
            fuelSlider.value = _fuelAndHealthController.GetFuelLevel();
        }
        
    
        public void UpdateHealthControllerCondition()
        {
            FuelAndHealthPanelSettings = _fuelAndHealthController.GetUavHealthCondition() switch //set FuelAndHealthPanelSettings based on the current health condition
            {
                UavHealthConditions.Healthy => _fuelAndHealthSettings.healthyUavFuelAndHealthPanelSettings,
                UavHealthConditions.Lost => _fuelAndHealthSettings.uavLostFuelAndHealthPanelSettings,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        public void UpdateFuelCondition()
        {
            if (_fuelAndHealthController.GetUavHealthCondition() != UavHealthConditions.Healthy)
                return;
            else
            {
                FuelAndHealthPanelSettings = _fuelAndHealthController.GetFuelCondition() switch
                {
                    FuelConditions.Normal => _fuelAndHealthSettings.healthyUavFuelAndHealthPanelSettings,
                    FuelConditions.Leaking => _fuelAndHealthSettings.fuelLeakFuelAndHealthPanelSettings,
                    FuelConditions.FatalLeak => _fuelAndHealthSettings.fatalLeakFuelAndHealthPanelSettings,
                    FuelConditions.Empty => _fuelAndHealthSettings.fuelEmptyFuelAndHealthPanelSettings,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

        }
        
    }
}
