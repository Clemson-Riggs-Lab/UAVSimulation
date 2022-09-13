using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using ScriptableObjects.EventChannels;
using ScriptableObjects.UAVs.Camera;
using ScriptableObjects.UAVs.Navigation;
using TargetDetection;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static ScriptableObjects.UAVs.FuelAndHealth.FuelAndHealthSettingsSO;

namespace UAVs.Sub_Modules.Camera
{
    public class CameraAndTargetDetectionPanelsManager : MonoBehaviour
    {
        public bool addAllPanelsDirectly = true;//TODO make not static (get from settings)

        private UavCameraPanelSettingsSO uavCameraPanelSettings;
        [SerializeField] public GameObject camerasContainerPanel;
        [SerializeField] public GameObject cameraAndTargetDetectionPanelPrefab;
         private UavEventChannelSO uavCreatedEventChannel;
         private UavEventChannelSO uavDestroyedEventChannel;
         private UavPathEventChannelSO uavArrivedAtDestinationEventChannel;
         private UavPathEventChannelSO uavStartedNewPathEventChannel;
         
         private Uav_FuelConditionEventChannelSO uavFuelConditionChangedEventChannel;
         private Uav_UavHealthConditionEventChannelSO uavHealthConditionChangedEventChannel;

        
         
         private List<Uav> _uavs = new List<Uav>(); //automatically updated by listening to the uavCreatedEventChannel
         private List<UavCameraAndTargetDetectionPanelController> cameraAndTargetDetectionPanels = new ();
        private void OnValidate()
        {
            AssertionHelper.AssertComponentReferencedInEditor(cameraAndTargetDetectionPanelPrefab, this, this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(camerasContainerPanel, this, this.gameObject);
        }

        private void Start()
        {
            InitializeChannels();
            SubscribeToChannels();
            ClearPanels();
        }

        private void ClearPanels()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }


        private void OnUavDisabled(Uav uav)
        {
            RemovePanelOfUav( uav);
            _uavs.Remove(uav);
        }

        private void RemovePanelOfUav(Uav uav)
        {
            var panel = cameraAndTargetDetectionPanels.FirstOrDefault(x => x.gameObject.name == "UAVCameraPanel " + uav.uavName);
            if (panel != null) 
            {
                cameraAndTargetDetectionPanels.Remove(panel);
                Destroy(panel.gameObject);
            }

        }
        private void OnUavStartedNewPath(Uav uav, Path path)
        {
            var panelController = cameraAndTargetDetectionPanels.FirstOrDefault(x => x.gameObject.name == "UAVCameraPanel " + uav.uavName);
            if (panelController != null) 
            {
                panelController.UavStartedNewPath(uav, path);
            }
        }
        private void OnUavHealthConditionChanged(Uav uav, UavHealthConditions healthCondition)
        {
            var panelController = cameraAndTargetDetectionPanels.FirstOrDefault(x => x.gameObject.name == "UAVCameraPanel " + uav.uavName);
            if (panelController != null)
            {
                panelController.UavHealthConditionChanged(uav, healthCondition);
            }
        }

        private void OnUavFuelConditionChanged(Uav uav, FuelConditions fuelCondition)
        {
            var panelController = cameraAndTargetDetectionPanels.FirstOrDefault(x => x.gameObject.name == "UAVCameraPanel " + uav.uavName);
            if (panelController != null)
            {
                panelController.UavFuelConditionChanged(uav, fuelCondition);
            }
        }
        private void ApplyHoveringConfigs(Uav uav, Path path)
        {
            var panelController = cameraAndTargetDetectionPanels.FirstOrDefault(x => x.gameObject.name == "UAVCameraPanel " + uav.uavName);
            if (panelController != null) 
            {
                panelController.ApplyHoveringConfigs();
            }
        }
        private void OnUavCreated(Uav uav)
        {
            _uavs.Add(uav);
            if (addAllPanelsDirectly)
            {
                GeneratePanelForUav(uav);
            }
        }

        private void GeneratePanelForUav(Uav uav)
        {
            var  panelGO = Instantiate(cameraAndTargetDetectionPanelPrefab,  camerasContainerPanel.transform);
            var panelController = panelGO.GetComponent<UavCameraAndTargetDetectionPanelController>();
            cameraAndTargetDetectionPanels.Add(panelController); 
            
            var cameraController =Instantiate(GameManager.Instance.prefabsDatabase.uavCameraPrefab, uav.uavBody.transform).GetComponent<UavCameraController>();
            cameraController.InitializeCamera(out RenderTexture renderTexture,uav.gameObject.layer);
           
            panelGO.GetComponentInChildren<RawImage>().texture = renderTexture;
            panelController.Initialize(uav, cameraController, renderTexture);
        }
        
        private void OnDisable()
        {
           UnsubscribeFromChannels();
        }
        private void InitializeChannels()
        {
            uavCameraPanelSettings = GameManager.Instance.settingsDatabase.uavSettingsDatabase.uavCameraPanelSettings;
            uavCreatedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
            uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
            uavArrivedAtDestinationEventChannel= GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavArrivedAtDestinationEventChannel;
            uavStartedNewPathEventChannel= GameManager.Instance.channelsDatabase.uavChannels.navigationChannels.uavStartedNewPathEventChannel;
            
            uavFuelConditionChangedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavFuelConditionChangedEventChannel;
            uavHealthConditionChangedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.fuelAndHealthChannels.uavHealthConditionChangedEventChannel;

        }
        private void SubscribeToChannels()
        {
            if(uavCreatedEventChannel != null)
                uavCreatedEventChannel.Subscribe(OnUavCreated);// subscribing to get each uav that is created 
            
            if(uavDestroyedEventChannel != null)
                uavDestroyedEventChannel.Subscribe(OnUavDisabled);
            
            if(uavArrivedAtDestinationEventChannel != null)
                uavArrivedAtDestinationEventChannel.Subscribe(ApplyHoveringConfigs);
            
            if(uavStartedNewPathEventChannel != null)
                uavStartedNewPathEventChannel.Subscribe(OnUavStartedNewPath);
            
            if(uavFuelConditionChangedEventChannel != null)
                uavFuelConditionChangedEventChannel.Subscribe(OnUavFuelConditionChanged);
            
            if(uavHealthConditionChangedEventChannel != null)
                uavHealthConditionChangedEventChannel.Subscribe(OnUavHealthConditionChanged);
        }

        private void UnsubscribeFromChannels()
        {
            if(uavCreatedEventChannel != null)
                uavCreatedEventChannel.Unsubscribe(OnUavCreated);
            if(uavDestroyedEventChannel != null)
                uavDestroyedEventChannel.Unsubscribe(OnUavDisabled);
            if(uavArrivedAtDestinationEventChannel != null)
                uavArrivedAtDestinationEventChannel.Unsubscribe(OnUavStartedNewPath);
            if(uavStartedNewPathEventChannel != null)
                uavStartedNewPathEventChannel.Unsubscribe(OnUavStartedNewPath);
            if(uavFuelConditionChangedEventChannel != null)
                uavFuelConditionChangedEventChannel.Unsubscribe(OnUavFuelConditionChanged);
            if(uavHealthConditionChangedEventChannel != null)
                uavHealthConditionChangedEventChannel.Unsubscribe(OnUavHealthConditionChanged);
        
        }
    }
}
