using UI.UavCameraAndTargetDetectionPanel.Settings.ScriptableObjects;
using UnityEngine;

namespace Modules.TargetDetection
{
    public class UavCameraController : MonoBehaviour
    {
        
        private UavCameraAndTargetDetectionPanelSettingsSO _andTargetDetectionPanelSettings;
        private Camera _uavCamera;
        public RenderTexture renderTexture;

        
        
        public void InitializeCamera( int gameObjectLayer)// we send out the render texture so that the panel can use it to display the camera feed
        {
            _andTargetDetectionPanelSettings = GameManager.Instance.settingsDatabase.uavCameraAndTargetDetectionPanelSettings;
            _uavCamera=gameObject.GetComponent<UnityEngine.Camera>();
            _uavCamera.cullingMask = (1 << gameObjectLayer)|(1<<LayerMask.NameToLayer("Default")); // Only render this uav's layer and the default layer
            var targetTexture = new RenderTexture(_andTargetDetectionPanelSettings.renderTextureWidth, _andTargetDetectionPanelSettings.renderTextureHeight, _andTargetDetectionPanelSettings.renderTextureDepth);
            targetTexture.Create();
            _uavCamera.targetTexture= targetTexture;
            renderTexture = targetTexture;
        
            _uavCamera.fieldOfView = _andTargetDetectionPanelSettings.cameraFieldOfView;
            _uavCamera.transform.localEulerAngles = new Vector3(_andTargetDetectionPanelSettings.cameraDownwardTilt, 0, 0);
        }
    }
}
