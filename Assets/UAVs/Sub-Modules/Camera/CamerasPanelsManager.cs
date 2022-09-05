using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.UI;

namespace UAVs.Sub_Modules.Camera
{
    public class CamerasPanelsManager : MonoBehaviour
    {
        public bool addAllPanelsDirectly = true;//TODO make not static (get from settings)

        
        [SerializeField] public GameObject camerasContainerPanel;
        [SerializeField] public GameObject cameraAndTargetSelectionPanelPrefab;
         private UavEventChannelSO uavCreatedEventChannel;
         private UavEventChannelSO uavDestroyedEventChannel;
         private List<Uav> _uavs = new List<Uav>(); //automatically updated by listening to the uavCreatedEventChannel
         private List<GameObject> CameraAndTargetSelectionPanels = new List<GameObject>();
        private void OnValidate()
        {
            AssertionHelper.AssertComponentReferencedInEditor(cameraAndTargetSelectionPanelPrefab, this, this.gameObject);
            AssertionHelper.AssertComponentReferencedInEditor(camerasContainerPanel, this, this.gameObject);
        }

        private void Start()
        {
            InitializeChannels();
            SubscribeToChannels();
        }
       
       
        private void OnUavDisabled(Uav uav)
        {
            RemovePanelOfUav( uav);
            _uavs.Remove(uav);
        }

        private void RemovePanelOfUav(Uav uav)
        {
            var panel = CameraAndTargetSelectionPanels.FirstOrDefault(x => x.gameObject.name == "UAVCameraPanel " + uav.ID);
            if (panel != null) 
            {
                CameraAndTargetSelectionPanels.Remove(panel);
                Destroy(panel);
            }

        }
        private void ResetPanelTargetToggle(Uav uav)
        {
            var panel = CameraAndTargetSelectionPanels.FirstOrDefault(x => x.gameObject.name == "UAVCameraPanel " + uav.ID);
            if (panel != null) 
            {
                CameraAndTargetSelectionPanels.Remove(panel);
                Destroy(panel);
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
            var  panelGO = Instantiate(cameraAndTargetSelectionPanelPrefab,  camerasContainerPanel.transform);
            CameraAndTargetSelectionPanels.Add(panelGO); 
            panelGO.name = "UAVCameraPanel " + uav.ID;
            var cameraController =Instantiate(GameManager.Instance.prefabsDatabase.uavCameraPrefab, uav.transform).GetComponent<UavCameraController>();
            cameraController.InitializeCamera(out RenderTexture renderTexture,uav.gameObject.layer);
            panelGO.GetComponentInChildren<RawImage>().texture = renderTexture;
        }
        
        
        private void OnDisable()
        {
            if(uavCreatedEventChannel != null)
                uavCreatedEventChannel.Unsubscribe(OnUavCreated);
            if(uavDestroyedEventChannel != null)
                uavDestroyedEventChannel.Unsubscribe(OnUavDisabled);
        }
        private void InitializeChannels()
        {
            uavCreatedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavCreatedEventChannel;
            uavDestroyedEventChannel= GameManager.Instance.channelsDatabase.uavChannels.uavDestroyedEventChannel;
        }
        private void SubscribeToChannels()
        {
            if(uavCreatedEventChannel != null)
                uavCreatedEventChannel.Subscribe(OnUavCreated);// subscribing to get each uav that is created 
           
            if(uavDestroyedEventChannel != null)
                uavDestroyedEventChannel.Subscribe(OnUavDisabled);
        }
    }
}
