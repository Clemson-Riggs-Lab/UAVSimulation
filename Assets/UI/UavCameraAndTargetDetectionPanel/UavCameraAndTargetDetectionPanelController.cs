using System;
using System.Linq;
using HelperScripts;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using TMPro;
using UAVs;
using UI.UavCameraAndTargetDetectionPanel.Settings.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using static HelperScripts.Enums;
using static UI.UavCameraAndTargetDetectionPanel.Settings.ScriptableObjects.UavCameraAndTargetDetectionPanelSettingsSO.UavCameraAndTargetDetectionPanelState;
using static Modules.FuelAndHealth.Settings.ScriptableObjects.FuelSettingsSO;

namespace UI.UavCameraAndTargetDetectionPanel
{
    public class UavCameraAndTargetDetectionPanelController : MonoBehaviour 
    {
        public Uav uav;
        
        private UavCameraAndTargetDetectionPanelSettingsSO _uavCameraAndTargetDetectionPanelSettings;
        public Toggle targetDetectedToggle;
        public Toggle targetNotDetectedToggle;

        public TextMeshProUGUI centerText;
        public TextMeshProUGUI bottomText;
       
        public Image darkScreenOverlay;
        public Image blackScreenOverlay;
        
        public UavPathEventChannelSO targetDetectedButtonClickedEventChannel;
        public UavPathEventChannelSO targetNotDetectedButtonClickedEventChannel;
    
        private FuelCondition _uavFuelCondition = FuelCondition.Normal;
        private UavCondition _uavCondition = UavCondition.Enabled;
        
        private UavCameraPanelConfigs _panelConfigs=new();
        
        private void Start()
        {
            GetReferencesFromGameManager();
        }

        private void GetReferencesFromGameManager()
        {
            _uavCameraAndTargetDetectionPanelSettings = GameManager.Instance.settingsDatabase.uavCameraAndTargetDetectionPanelSettings;
            targetDetectedButtonClickedEventChannel = GameManager.Instance.channelsDatabase.targetDetectionChannels.targetDetectedEventChannel;
            targetNotDetectedButtonClickedEventChannel = GameManager.Instance.channelsDatabase.targetDetectionChannels.targetNotDetectedEventChannel;
        }
        
        public void OnTargetDetectedToggleClicked()
        {
            if(targetDetectedButtonClickedEventChannel != null)
                targetDetectedButtonClickedEventChannel.RaiseEvent(uav, uav.currentPath);
        
        }
        public void OnTargetNotDetectedToggleClicked()
        {
            if(targetNotDetectedButtonClickedEventChannel != null)
                targetNotDetectedButtonClickedEventChannel.RaiseEvent(uav, uav.currentPath);
        }

        public void Initialize(Uav uav , RenderTexture renderTexture)
        {
            this.uav = uav;
            gameObject.name = "UAVCameraPanel " + uav.uavName;
        }


        public void UavStartedNewPath(Uav uav, Path path)
        {
            ResetToggleState();
            UpdatePanelConfigs();
            UpdateUIElements();
        }
        public void UavConditionChanged(Uav uav, UavCondition condition)
        {
            _uavCondition = condition;
            UpdatePanelConfigs();
            UpdateUIElements();
        }
        public void UavFuelConditionChanged(Uav uav, FuelCondition fuelCondition)
        {
            _uavFuelCondition = fuelCondition;
            UpdatePanelConfigs();
            UpdateUIElements();
        }
        public void EnablePanel(bool enabled)
        {
            if (enabled)
            {
                gameObject.transform.localScale= Vector3.one;
                gameObject.GetComponent<LayoutElement>().ignoreLayout = false;
            }
            else
            {
                gameObject.transform.localScale= Vector3.zero;
                gameObject.GetComponent<LayoutElement>().ignoreLayout = true;
            }
        }
        public void ApplyHoveringConfigs() // this is called to set the configs when the uav is hovering over the waypoint (rotating to get to the destination)
        {
            UpdatePanelConfigs(_uavCameraAndTargetDetectionPanelSettings.hoveringUavCameraAndTargetDetectionPanelConfigs); 
            UpdateUIElements();
        }
        
        public void ResetToggleState()
        {
            targetDetectedToggle.isOn = false;
            targetNotDetectedToggle.isOn = true;
        }
        private void UpdatePanelConfigs(UavCameraPanelConfigs configs=null)
        {
            configs ??= _uavCameraAndTargetDetectionPanelSettings.enabledUavCameraAndTargetDetectionPanelConfigs;
            
            var uavConditionConfigs = _uavCondition switch
            {
                UavCondition.Enabled =>  _uavCameraAndTargetDetectionPanelSettings.enabledUavCameraAndTargetDetectionPanelConfigs,
                UavCondition.Lost =>  _uavCameraAndTargetDetectionPanelSettings.lostUavCameraAndTargetDetectionPanelConfigs,
                UavCondition.Hidden => _uavCameraAndTargetDetectionPanelSettings.hiddenUavCameraAndTargetDetectionPanelConfigs,
                UavCondition.Finished => _uavCameraAndTargetDetectionPanelSettings.finishedUavCameraAndTargetDetectionPanelConfigs,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            var fuelLeakConfigs = _uavFuelCondition switch
            {
                FuelCondition.Normal => _uavCameraAndTargetDetectionPanelSettings.enabledUavCameraAndTargetDetectionPanelConfigs,
                FuelCondition.Leaking => _uavCameraAndTargetDetectionPanelSettings.fuelLeakCameraAndTargetDetectionPanelConfigs,
                FuelCondition.FatalLeak => _uavCameraAndTargetDetectionPanelSettings.fatalLeakCameraAndTargetDetectionPanelConfigs,
                FuelCondition.Empty => _uavCameraAndTargetDetectionPanelSettings.emptyFuelCameraAndTargetDetectionPanelConfigs,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            
            _panelConfigs.cameraAndTargetDetectionPanelState = EnumHelpers.GetMaxEnum(new []{configs.cameraAndTargetDetectionPanelState, uavConditionConfigs.cameraAndTargetDetectionPanelState, fuelLeakConfigs.cameraAndTargetDetectionPanelState});
           
            var bottomTextArray= new []{configs.bottomText, uavConditionConfigs.bottomText, fuelLeakConfigs.bottomText};
            _panelConfigs.bottomText= string.Join(" | ",bottomTextArray.Where(s=> !string.IsNullOrEmpty(s)));
            
            var centerTextArray= new []{configs.centerText, uavConditionConfigs.centerText, fuelLeakConfigs.centerText};
            _panelConfigs.centerText= string.Join(" | ",centerTextArray.Where(s=> !string.IsNullOrEmpty(s)));



        }
        
          private void UpdateUIElements()
        {
            switch (_panelConfigs.cameraAndTargetDetectionPanelState)
            {
                case None:
                   EnablePanel(true);
                    darkScreenOverlay.gameObject.SetActive(false);
                    blackScreenOverlay.gameObject.SetActive(false);
                    break;
                case BlackScreen:
                    EnablePanel(true);
                    blackScreenOverlay.gameObject.SetActive(true);
                    darkScreenOverlay.gameObject.SetActive(false);
                    break;
                case DarkScreen:
                    EnablePanel(true);
                    darkScreenOverlay.gameObject.SetActive(true);
                    blackScreenOverlay.gameObject.SetActive(false);
                    break;
                case Hide:
                    EnablePanel(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            centerText.text = _panelConfigs.centerText;
            bottomText.text = _panelConfigs.bottomText;
            
            if (_panelConfigs.cameraAndTargetDetectionPanelState is BlackScreen or DarkScreen)
            {
                centerText.color  = Color.white;
                bottomText.color =  Color.white;
            }
            else
            {
                centerText.color  = Color.black;
                bottomText.color =  Color.black;
            }
        }


         
    }
}
