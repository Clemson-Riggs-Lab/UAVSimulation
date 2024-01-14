using System.Collections.Generic;
using HelperScripts;
using Modules.FuelAndHealth.Channels.ScriptableObjects;
using Modules.Navigation;
using Modules.Navigation.Channels.ScriptableObjects;
using Modules.TargetDetection;
using TMPro;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UI.UavCameraAndTargetDetectionPanel.Settings.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using static HelperScripts.Enums;
using static Modules.FuelAndHealth.Settings.ScriptableObjects.FuelSettingsSO;

namespace UI.UavCameraAndTargetDetectionPanel
{
    public class CameraAndTargetDetectionPanelsManager : MonoBehaviour
    {
        private UavCameraAndTargetDetectionPanelSettingsSO _uavCameraAndTargetDetectionPanelSettings;
        [SerializeField] public GameObject targetDetectionPanelsContainer;
        [SerializeField] public GameObject cameraAndTargetDetectionPanelPrefab;
         private UavEventChannelSO _uavCreatedEventChannel;
         private UavEventChannelSO _uavDestroyedEventChannel;
         private UavPathEventChannelSO _uavArrivedAtDestinationEventChannel;
         private UavPathEventChannelSO _uavStartedNewPathEventChannel;
         
         private UavFuelConditionEventChannelSO _uavFuelConditionChangedEventChannel;
         private UavConditionEventChannelSO _uavConditionChangedEventChannel;

         public TextMeshProUGUI headerText;
         public GameObject centerPanel;
         
         private Dictionary<Uav,UavCameraAndTargetDetectionPanelController> _uavPanelsDictionary = new ();
       
         private void OnValidate()
        {
            AssertionHelper.AssertComponentReferencedInEditor(cameraAndTargetDetectionPanelPrefab, this, this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(targetDetectionPanelsContainer, this, this.gameObject);
        }

        private void Start()
        {
            GetReferencesFromGameManager();
            SubscribeToChannels();
            ClearPanels();
            
            headerText.text = _uavCameraAndTargetDetectionPanelSettings.headerText;
            headerText.color = ColorHelper.StringToColor(_uavCameraAndTargetDetectionPanelSettings.headerTextColor);
            gameObject.GetComponent<FlexibleGridLayout>().ignoreZeroScaleObjects = _uavCameraAndTargetDetectionPanelSettings.onlyAccountForVisibleUavsButtonsInLayout;

            centerPanel.GetComponent<Image>().color = ColorHelper.StringToColor(headerText.text == "Primary Task" ? _uavCameraAndTargetDetectionPanelSettings.headerTextColor : "Black");
        }

        private void ClearPanels()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void OnUavCreated(Uav uav)
        {
            GeneratePanelForUav(uav);
        }
        private void OnUavDisabled(Uav uav)
        {
            RemovePanelOfUav( uav);
        }
        
        private void GeneratePanelForUav(Uav uav)
        {
            var  panelGo = Instantiate(cameraAndTargetDetectionPanelPrefab,  targetDetectionPanelsContainer.transform);
            var panelController = panelGo.GetComponent<UavCameraAndTargetDetectionPanelController>();
            var cameraController = uav.GetComponentInChildren<UavCameraController>();
            var renderTexture = cameraController.renderTexture;
            panelGo.GetComponentInChildren<RawImage>().texture = renderTexture;
            panelController.Initialize(uav, renderTexture);
            _uavPanelsDictionary.Add(uav,panelController); 
        }
      
        private void RemovePanelOfUav(Uav uav)
        {
            if (_uavPanelsDictionary.ContainsKey(uav))
            {
                Destroy(_uavPanelsDictionary[uav].gameObject); //destroy panel
                _uavPanelsDictionary.Remove(uav); //remove from dictionary
            }
        }
        
        private void OnUavStartedNewPath(Uav uav, Path path)
        {
            if (_uavPanelsDictionary.ContainsKey(uav))
                _uavPanelsDictionary[uav].UavStartedNewPath(uav, path);
        }
        
        private void OnUavHealthConditionChanged(Uav uav, UavCondition uavCondition)
        {
            if (_uavPanelsDictionary.ContainsKey(uav))
                _uavPanelsDictionary[uav].UavConditionChanged(uav, uavCondition);
        }

        private void OnUavFuelConditionChanged(Uav uav, FuelCondition fuelCondition)
        {
            if (_uavPanelsDictionary.ContainsKey(uav))
                _uavPanelsDictionary[uav].UavFuelConditionChanged(uav, fuelCondition);
        }
        
        private void ApplyHoveringConfigs(Uav uav, Path path)
        {
            if (_uavPanelsDictionary.ContainsKey(uav))
                _uavPanelsDictionary[uav].ApplyHoveringConfigs();
        }
        
        private void OnDisable()
        {
           UnsubscribeFromChannels();
        }
        private void GetReferencesFromGameManager()
        {
            _uavCameraAndTargetDetectionPanelSettings = GameManager.Instance.settingsDatabase.uavCameraAndTargetDetectionPanelSettings;
            _uavCreatedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
            _uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
            _uavArrivedAtDestinationEventChannel= GameManager.Instance.channelsDatabase.navigationChannels.uavArrivedAtDestinationEventChannel;
            _uavStartedNewPathEventChannel= GameManager.Instance.channelsDatabase.navigationChannels.uavStartedNewPathEventChannel;
            
            _uavFuelConditionChangedEventChannel= GameManager.Instance.channelsDatabase.fuelChannels.uavFuelConditionChangedEventChannel;
            _uavConditionChangedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;

        }
        private void SubscribeToChannels()
        {
            if(_uavCreatedEventChannel != null)
                _uavCreatedEventChannel.Subscribe(OnUavCreated);// subscribing to get each uav that is created 
            
            if(_uavDestroyedEventChannel != null)
                _uavDestroyedEventChannel.Subscribe(OnUavDisabled);
            
            if(_uavArrivedAtDestinationEventChannel != null)
                _uavArrivedAtDestinationEventChannel.Subscribe(ApplyHoveringConfigs);
            
            if(_uavStartedNewPathEventChannel != null)
                _uavStartedNewPathEventChannel.Subscribe(OnUavStartedNewPath);
            
            if(_uavFuelConditionChangedEventChannel != null)
                _uavFuelConditionChangedEventChannel.Subscribe(OnUavFuelConditionChanged);
            
            if(_uavConditionChangedEventChannel != null)
                _uavConditionChangedEventChannel.Subscribe(OnUavHealthConditionChanged);
        }

        private void UnsubscribeFromChannels()
        {
            if(_uavCreatedEventChannel != null)
                _uavCreatedEventChannel.Unsubscribe(OnUavCreated);
            if(_uavDestroyedEventChannel != null)
                _uavDestroyedEventChannel.Unsubscribe(OnUavDisabled);
            if(_uavArrivedAtDestinationEventChannel != null)
                _uavArrivedAtDestinationEventChannel.Unsubscribe(ApplyHoveringConfigs);
            if(_uavStartedNewPathEventChannel != null)
                _uavStartedNewPathEventChannel.Unsubscribe(OnUavStartedNewPath);
            if(_uavFuelConditionChangedEventChannel != null)
                _uavFuelConditionChangedEventChannel.Unsubscribe(OnUavFuelConditionChanged);
            if(_uavConditionChangedEventChannel != null)
                _uavConditionChangedEventChannel.Unsubscribe(OnUavHealthConditionChanged);
        }
    }
}
