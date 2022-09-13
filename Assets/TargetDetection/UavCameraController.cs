using System;
using ScriptableObjects.UAVs.Camera;
using UnityEngine;

namespace UAVs.Sub_Modules.Camera
{
    public class UavCameraController : MonoBehaviour
    {
        
        private UavCameraPanelSettingsSO panelSettings;
        private UnityEngine.Camera uavCamera;
    
        private UavCameraPanelConfigs cameraConfigs;

        public UavCameraPanelConfigs Configs
        {
            get=> cameraConfigs;
            set
            {
                cameraConfigs = value;
                UpdateCamera();
            }
        }
        
        public void InitializeCamera(out RenderTexture renderTexture, int gameObjectLayer)// we send out the render texture so that the panel can use it to display the camera feed
        {
            panelSettings = GameManager.Instance.settingsDatabase.uavSettingsDatabase.uavCameraPanelSettings;
            uavCamera=gameObject.GetComponent<UnityEngine.Camera>();
            uavCamera.cullingMask = (1 << gameObjectLayer)|(1<<LayerMask.NameToLayer("Default")); // Only render this uav's layer and the default layer
            var targetTexture = new RenderTexture(panelSettings.renderTextureWidth, panelSettings.renderTextureHeight, panelSettings.renderTextureDepth);
            targetTexture.Create();
            uavCamera.targetTexture= targetTexture;
            renderTexture = targetTexture;
        
            uavCamera.fieldOfView = panelSettings.cameraFieldOfView;
            uavCamera.transform.localEulerAngles = new Vector3(panelSettings.cameraDownwardTilt, 0, 0);
        }
        
        
        private void UpdateCamera()
        { 
            //TODO 
            switch (cameraConfigs.videoArtifacts)
            {
                case UavCameraPanelSettingsSO.UavVideoArtifacts.None:
                    break;
                case UavCameraPanelSettingsSO.UavVideoArtifacts.BlackScreen:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
