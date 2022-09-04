using System;
using System.Collections;
using HelperScripts;
using TMPro;
using UAVs.Sub_Modules.Fuel.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using static UAVs.Sub_Modules.Fuel.ScriptableObjects.FuelAndHealthSettingsSO;

namespace UAVs.Sub_Modules.Fuel
{
    public class StatusPanelController : MonoBehaviour
    {
        // Start is called before the first frame update
        public TextMeshProUGUI idText;
        public TextMeshProUGUI healthButtonText;
        public Button healthButton;
        public Slider fuelSlider;
        public Image fuelSliderImage;
        public Image healthButtonImage;

        private FuelAndHealthController _fuelAndHealthController;
        FuelAndHealthSettingsSO _fuelAndHealthSettings;
        private PanelSettings _panelSettings;

        public PanelSettings PanelSettings
        {
            get => _panelSettings;
            set
            {
                _panelSettings = value;
                UpdateUIElements();
            }
        }

        private void UpdateUIElements()
        {
            healthButtonText.text = _panelSettings.panelVisualSettings.healthButtonText;
            healthButton.interactable = _panelSettings.healthButtonInteractable;
            healthButtonText.color = ColorHelper.StringToColor(_panelSettings.panelVisualSettings.healthButtonTextColor);
            
            fuelSliderImage.color = ColorHelper.StringToColor(_panelSettings.panelVisualSettings.fuelSliderColor);
            healthButtonImage.color = ColorHelper.StringToColor(_panelSettings.panelVisualSettings.healthButtonColor);
            if (_panelSettings.healthButtonInteractable == true)
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
            _fuelAndHealthSettings= GameManager.Instance.settingsDatabase.uavSettings.fuelAndHealthSettings;
            
            _fuelAndHealthController = fuelAndHealthController;
            idText.text = _fuelAndHealthController.uavId.ToString();
            fuelSlider.maxValue = _fuelAndHealthController.StartingFuelLevel;

            // listen to events from fuelAndHealthController
            _fuelAndHealthController.FuelLevelChanged += UpdateFuelLevel;
            _fuelAndHealthController.HealthConditionChanged += UpdateHealthControllerCondition;
            _fuelAndHealthController.FuelConditionChanged += UpdateFuelCondition;
            healthButton.onClick.AddListener(() => _fuelAndHealthController.OnHealthButtonClicked(healthButtonText.text));
            
            UpdateHealthControllerCondition();
        }
        
        public void UpdateFuelLevel()
        {
            fuelSlider.value = _fuelAndHealthController.FuelLevel;
        }
        
    
        public void UpdateHealthControllerCondition()
        {
            PanelSettings = _fuelAndHealthController.HealthCondition switch
            {
                HealthConditions.Healthy => _fuelAndHealthSettings.healthyPanelSettings,
                HealthConditions.Unavailable => _fuelAndHealthSettings.uavUnavailablePanelSettings,
                HealthConditions.Lost => _fuelAndHealthSettings.uavLostPanelSettings,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        public void UpdateFuelCondition()
        {
            if (_fuelAndHealthController.HealthCondition != HealthConditions.Healthy)
                return;
            else
            {
                PanelSettings = _fuelAndHealthController.fuelCondition switch
                {
                    FuelConditions.Normal => _fuelAndHealthSettings.healthyPanelSettings,
                    FuelConditions.Leaking => _fuelAndHealthSettings.fuelLeakPanelSettings,
                    FuelConditions.FatalLeak => _fuelAndHealthSettings.fatalLeakPanelSettings,
                    FuelConditions.Empty => _fuelAndHealthSettings.fuelEmptyPanelSettings,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

        }

        private void UpdateSliderAndButtonTextAndColors()
        {
            throw new System.NotImplementedException();
        }
    }
}
