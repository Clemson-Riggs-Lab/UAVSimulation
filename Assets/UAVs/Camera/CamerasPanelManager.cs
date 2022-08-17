using System;
using System.Collections.Generic;
using System.Linq;
using Events.ScriptableObjects;
using HelperScripts;
using UnityEngine;
using UnityEngine.UI;

namespace UAVs.Camera
{
    public class CamerasPanelManager : MonoBehaviour
    {
        public bool addAllPanelsDirectly = true;//TODO make not static (get from settings)

        
        [SerializeField] public GameObject camerasContainerPanel;
        [SerializeField] public GameObject cameraAndTargetSelectionPanelPrefab;
        [SerializeField] private ObjectEventChannelSO uavCreatedChannel = null;
        [SerializeField] private ObjectEventChannelSO uavDisabledChannel = null;
        [NonSerialized] private List<Uav> _uavs = new List<Uav>(); //automatically updated by listening to the uavCreatedChannel
        [NonSerialized] public List<GameObject> CameraAndTargetSelectionPanels = new List<GameObject>();
        private void OnValidate()
        {
            MyDebug.AssertComponentReferencedInEditor(cameraAndTargetSelectionPanelPrefab, this, this.gameObject);
            MyDebug.AssertComponentReferencedInEditor(camerasContainerPanel, this, this.gameObject);
        }

        private void OnEnable()
        {
            if(uavCreatedChannel != null)
                uavCreatedChannel.Subscribe(OnUavCreated);// subscribing to get each uav that is created 
            if(uavDisabledChannel != null)
                uavDisabledChannel.Subscribe(OnUavDisabled);
        }
        private void OnUavDisabled(object uav)
        {
            RemovePanelOfUav((Uav) uav);
            _uavs.Remove((Uav)uav);
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

        private void OnUavCreated(object uav)
        {
            _uavs.Add((Uav)uav);
            if (addAllPanelsDirectly)
            {
                GeneratePanelForUav(uav);
            }
        }

        private void GeneratePanelForUav(object uav)
        {
            var  panelGO = Instantiate(cameraAndTargetSelectionPanelPrefab,  camerasContainerPanel.transform) as GameObject ;
            CameraAndTargetSelectionPanels.Add(panelGO); 
            panelGO.name = "UAVCameraPanel " + ((Uav)uav).ID; 
            
            var uavCameraController=((Uav)uav).gameObject.GetComponentInChildren<UavCameraController>();
            uavCameraController.InitializeCamera(out RenderTexture renderTexture);
            panelGO.GetComponentInChildren<RawImage>().texture = renderTexture;
        }
        
        private void OnDisable()
        {
            if(uavCreatedChannel != null)
                uavCreatedChannel.Unsubscribe(OnUavCreated);
            if(uavDisabledChannel != null)
                uavDisabledChannel.Unsubscribe(OnUavDisabled);
        }
    }
}
