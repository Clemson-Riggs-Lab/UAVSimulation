using System;
using System.Collections;
using HelperScripts;
using Modules.FuelAndHealth.Settings.ScriptableObjects;
using TMPro;
using UAVs;
using UI.FuelAndHealthPanel.Settings.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using static HelperScripts.Enums;
using static Modules.FuelAndHealth.Settings.ScriptableObjects.FuelSettingsSO;
using static Modules.FuelAndHealth.Settings.ScriptableObjects.FuelSettingsSO.FuelComputationType;

namespace UI.FuelAndHealthPanel
{
    public class FuelAndHealthStatusPanelController : MonoBehaviour 
    {
        public TextMeshProUGUI idText;
        public TextMeshProUGUI healthButtonText;
        public Button healthButton;
        public Slider fuelSlider;
        public Image fuelSliderImage;
        public Image healthButtonImage;

        private Uav _uav;
        private FuelSettingsSO _fuelSettings;
        private FuelAndHealthPanelSettingsSO _fuelAndHealthPanelSettings;
        private FuelAndHealthPanelConfigs _panelConfigs;
        private FuelCondition _fuelCondition = FuelCondition.Normal;
        private UavCondition _uavCondition = UavCondition.EnabledForTargetDetectionAndRerouting;

        
        public void Initialize(Uav uav, FuelAndHealthStatusPanelsManager manager)
        {
            _fuelSettings= GameManager.Instance.settingsDatabase.fuelSettings;
            _fuelAndHealthPanelSettings = GameManager.Instance.settingsDatabase.fuelAndHealthPanelSettings;
            
            _uav = uav;
            idText.text = _uav.uavName;
            fuelSlider.maxValue = GetFuelCapacity();
            healthButton.onClick.AddListener(() =>  manager.OnHealthButtonClicked(_uav,healthButtonText.text));
            
            UpdatePanelConfigs();
            UpdateUIElements(); // initialize the UI
        }

        private float GetFuelCapacity()
        {
            return _fuelSettings.fuelComputationType switch
            {
                Duration => _fuelSettings.fuelDuration,
                Consumption => _fuelSettings.fuelCapacity,
                _ => throw new ArgumentOutOfRangeException(nameof(_fuelSettings.fuelComputationType))
            };
        }

        private void UpdatePanelConfigs()
        {
            _panelConfigs= _fuelAndHealthPanelSettings.enabledUavFuelAndHealthPanelConfigs;

            switch (_uavCondition, _fuelCondition)
            {
                case (UavCondition.Lost, _): // if the UAV is lost, then the fuel condition is irrelevant 
                    _panelConfigs = _fuelAndHealthPanelSettings.lostUavFuelAndHealthPanelConfigs;
                    return;
                
                case (UavCondition.Hidden , FuelCondition.Normal): // if the UAV is hidden and the fuel condition is normal, we apply the hidden uav configs
                case (UavCondition.Hidden, _) when !_fuelAndHealthPanelSettings.showFuelLeakConditionWhenUavHidden:  // also if the uav is hidden and the fuel condition is not normal,but the settings say that fuel leaks should not be visible when the uav is hidden, we apply the hidden uav configs
                    _panelConfigs = _fuelAndHealthPanelSettings.hiddenUavFuelAndHealthPanelConfigs;
                    return;
                
                default: // for all other cases ( uav condition is normal, or not normal but showFuelLeakConditionWhenUavHidden is true 
                { 
                    _panelConfigs = _fuelCondition switch
                    {
                        FuelCondition.Normal => _fuelAndHealthPanelSettings.enabledUavFuelAndHealthPanelConfigs,
                        FuelCondition.Leaking => _fuelAndHealthPanelSettings.fuelLeakFuelAndHealthPanelConfigs,
                        FuelCondition.FatalLeak => _fuelAndHealthPanelSettings.fatalLeakFuelAndHealthPanelConfigs,
                        FuelCondition.Empty => _fuelAndHealthPanelSettings.fuelEmptyFuelAndHealthPanelConfigs,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    return;
                }
            }
        }

        private void UpdateUIElements()
        {
            healthButtonText.text = _panelConfigs.healthButtonText;
            healthButton.interactable = _panelConfigs.healthButtonInteractable;
            healthButtonText.color = ColorHelper.StringToColor(_panelConfigs.healthButtonTextColor);
            
            fuelSliderImage.color = ColorHelper.StringToColor(_panelConfigs.fuelSliderColor);
            healthButtonImage.color = ColorHelper.StringToColor(_panelConfigs.healthButtonColor);
            if (_panelConfigs.healthButtonInteractable == true)
            {
                StartCoroutine(ButtonInteractableDurationTimer());
            }
            
            if(_panelConfigs.isVisibile == false)
            {
                gameObject.transform.localScale = Vector3.zero;
                gameObject.GetComponent<LayoutElement>().ignoreLayout = true;
            }
            else
            {
                gameObject.transform.localScale = Vector3.one;
                gameObject.GetComponent<LayoutElement>().ignoreLayout = false;
            }
        }

        private IEnumerator ButtonInteractableDurationTimer()
        {
            yield return new WaitForSeconds(_fuelSettings.fuelLeakButtonInteractionDurationBeforeFatalLeak);
            healthButton.interactable = false;
        }
        
        public void OnFuelLevelChanged(float fuelLevel) => fuelSlider.value = fuelLevel;

        public void OnUavConditionChanged(UavCondition condition)
        {
            _uavCondition = condition;
            UpdatePanelConfigs();
            UpdateUIElements();
        }

        public void OnFuelConditionChanged(FuelCondition condition)
        {
            _fuelCondition = condition;
            UpdatePanelConfigs();
            UpdateUIElements();
        }
    }
}
