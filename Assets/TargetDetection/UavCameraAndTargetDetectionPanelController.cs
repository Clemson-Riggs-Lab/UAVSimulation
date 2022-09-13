using System;
using System.Globalization;
using HelperScripts;
using ScriptableObjects.UAVs.Camera;
using ScriptableObjects.UAVs.Navigation;
using TMPro;
using UAVs;
using UAVs.Sub_Modules.Camera;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;
using UnityEngine.UI;
using static ScriptableObjects.UAVs.Camera.UavCameraPanelSettingsSO.UavVideoArtifacts;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO;

namespace TargetDetection
{
    public class UavCameraAndTargetDetectionPanelController : MonoBehaviour 
    {
        public Uav uav;
        
        private UavCameraPanelSettingsSO uavCameraPanelSettings;
       
        public UavCameraController uavCameraController;
        
        public Toggle targetDetectedToggle;
        public Toggle targetNotDetectedToggle;

        public TextMeshProUGUI centerText;
        public TextMeshProUGUI bottomText;
       
        public Image darkScreenOverlay;
        public Image blackScreenOverlay;
        
        public UavPathEventChannelSO targetDetectedButtonClickedEventChannel;
        public UavPathEventChannelSO targetNotDetectedButtonClickedEventChannel;
    
        private FuelConditions uavFuelCondition = FuelConditions.Normal;
        private UavHealthConditions uavHealthCondition = UavHealthConditions.Healthy;
        private bool UavVisuallyEnabled = true;
        
        private UavCameraPanelConfigs panelConfigs=new();
        
        private void Start()
        {
            uavCameraPanelSettings = GameManager.Instance.settingsDatabase.uavSettingsDatabase.uavCameraPanelSettings;
            InitializeChannels();
        }

        private void InitializeChannels()
        {
            targetDetectedButtonClickedEventChannel = GameManager.Instance.channelsDatabase.targetDetectionChannels.targetDetectedButtonClickedEventChannel;
            targetNotDetectedButtonClickedEventChannel = GameManager.Instance.channelsDatabase.targetDetectionChannels.targetNotDetectedButtonClickedEventChannel;
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

        public void Initialize(Uav uav, UavCameraController cameraController, RenderTexture renderTexture)
        {
            this.uav = uav;
            uavCameraController = cameraController;
            gameObject.name = "UAVCameraPanel " + uav.uavName;
        }


        public void UavStartedNewPath(Uav uav, Path path)
        {
            ResetToggleState();
            UavVisuallyEnabled = path.uavIsVisuallyEnabled;
            UpdatePanelConfigs();
            UpdateUIElements();
        }
        public void UavHealthConditionChanged(Uav uav, UavHealthConditions healthCondition)
        {
            uavHealthCondition = healthCondition;
            UpdatePanelConfigs();
            UpdateUIElements();
        }
        public void UavFuelConditionChanged(Uav uav, FuelConditions fuelCondition)
        {
            uavFuelCondition = fuelCondition;
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
            UpdatePanelConfigs(uavCameraPanelSettings.hoveringUavCameraPanelConfigs); 
            UpdateUIElements();
        }
        
        public void ResetToggleState()
        {
            targetDetectedToggle.isOn = false;
            targetNotDetectedToggle.isOn = true;
        }
        private void UpdatePanelConfigs(UavCameraPanelConfigs configs=null)
        {
            configs ??= uavCameraPanelSettings.healthyUavCameraPanelConfigs;
            
          panelConfigs = configs;
          panelConfigs = new();
            var healthConfigs = uavHealthCondition switch
            {
                UavHealthConditions.Healthy =>  uavCameraPanelSettings.healthyUavCameraPanelConfigs,
                UavHealthConditions.Lost =>  uavCameraPanelSettings.lostUavCameraPanelConfigs,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            var fuelLeakConfigs = uavFuelCondition switch
            {
                FuelConditions.Normal => uavCameraPanelSettings.healthyUavCameraPanelConfigs,
                FuelConditions.Leaking => uavCameraPanelSettings.fuelLeakCameraPanelConfigs,
                FuelConditions.FatalLeak => uavCameraPanelSettings.fatalLeakCameraPanelConfigs,
                FuelConditions.Empty => uavCameraPanelSettings.emptyFuelCameraPanelConfigs,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            
            var visibilityConfigs = UavVisuallyEnabled switch
            {
                true => uavCameraPanelSettings.healthyUavCameraPanelConfigs,
                false => uavCameraPanelSettings.visuallyDisabledUavCameraPanelConfigs,
            };
            
            
             panelConfigs.videoArtifacts = EnumHelpers.GetMaxEnum(new []{healthConfigs.videoArtifacts, fuelLeakConfigs.videoArtifacts, visibilityConfigs.videoArtifacts});
             panelConfigs.bottomText = healthConfigs.bottomText  + fuelLeakConfigs.bottomText +  visibilityConfigs.bottomText;
              panelConfigs.centerText = healthConfigs.centerText  + fuelLeakConfigs.centerText  + visibilityConfigs.centerText;

        }
        
          private void UpdateUIElements()
        {
            switch (panelConfigs.videoArtifacts)
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

            centerText.text = panelConfigs.centerText;
            bottomText.text = panelConfigs.bottomText;
            
            if (panelConfigs.videoArtifacts is BlackScreen or DarkScreen)
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
