using System;
using ScriptableObjects.UAVs.Camera;
using UnityEngine;

namespace UAVs.Sub_Modules.Camera
{
    public class UavCameraController : MonoBehaviour
    {
    
        public bool DisableWhenUavIsNotVisible = true; //TODO make not static (get from settings)
    
        private UavCameraSettingsSO _settings;
        private UnityEngine.Camera uavCamera;
    
        private UavCameraAndTargetDetectionConfigs cameraConfigs;

        public UavCameraAndTargetDetectionConfigs Configs
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
            _settings = GameManager.Instance.settingsDatabase.uavSettings.uavCameraSettings;
            uavCamera=gameObject.GetComponent<UnityEngine.Camera>();
            uavCamera.cullingMask = (1 << gameObjectLayer)|(1<<LayerMask.NameToLayer("Default")); // Only render this uav's layer and the default layer
            var targetTexture = new RenderTexture(_settings.renderTextureWidth, _settings.renderTextureHeight, _settings.renderTextureDepth);
            targetTexture.Create();
            uavCamera.targetTexture= targetTexture;
            renderTexture = targetTexture;
        
            uavCamera.fieldOfView = _settings.fieldOfView;
            uavCamera.transform.localEulerAngles = new Vector3(_settings.rotationX, 0, 0);
        }
        
        
        private void UpdateCamera()
        { 
            //TODO 
            switch (cameraConfigs.videoArtifacts)
            {
                case UavCameraSettingsSO.UavVideoArtifacts.None:
                    break;
                case UavCameraSettingsSO.UavVideoArtifacts.BlackScreen:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
