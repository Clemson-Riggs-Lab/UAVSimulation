using System.Collections;
using HelperScripts;
using Modules.Navigation;
using UAVs;
using UAVs.Channels.ScriptableObjects;
using UI.Paths_Rendering.Settings.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using static HelperScripts.Enums.UavCondition;

namespace UI.Paths_Rendering
{
    public class PathRenderer : MonoBehaviour
    {
        private Uav _uav;
        private LineRenderer _lineRenderer;
        PathsRenderingSettingsSO _pathsRenderingSettings;
        private Color _lineColor;
        private Coroutine _updateLineRendererCoroutine;
        private MeshRenderer _waypointMeshRenderer;
        private UavConditionEventChannelSO _uavConditionChangedEventChannel;

        public void Initialize(Uav uav)
        {
            _uav = uav;
            GetReferencesFromGameManager();
            SubscribeToChannels();
            _lineRenderer = gameObject.GetOrAddComponent<LineRenderer>();
            InitializeLineRenderer();
        }
        private void GetReferencesFromGameManager()
        {
            _pathsRenderingSettings = GameManager.Instance.settingsDatabase.pathsRenderingSettings;
            _uavConditionChangedEventChannel = GameManager.Instance.channelsDatabase.uavChannels.uavConditionChangedEventChannel;
        }

        private void SubscribeToChannels()
        {

            if (_uavConditionChangedEventChannel != null) _uavConditionChangedEventChannel.Subscribe(OnUavConditionChanged);
        }

        private void OnUavConditionChanged(Uav uav, Enums.UavCondition uavCondition)
        {
            if (uavCondition is not Lost ) return;
			
            if (uav.currentPath != null&& uav.currentPath.destinationWayPoint != null)
            {
                uav.currentPath.destinationWayPoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        private void UnsubscribeFromChannels()
        {
            if (_uavConditionChangedEventChannel != null) _uavConditionChangedEventChannel.Unsubscribe(OnUavConditionChanged);

        }


        public void SetLineRenderer(Path path, bool preview=false)
        {
            if(path.destinationWayPoint == null) return;
            
            if(_pathsRenderingSettings.showWaypointOnMap)
            {
                if (_waypointMeshRenderer != null )
                    _waypointMeshRenderer.enabled = false;
                
                if(!preview)
                    _waypointMeshRenderer = path.destinationWayPoint.gameObject.GetComponent<MeshRenderer>();
                
                if( path.uavIsVisuallyEnabledForRerouting)
                {
                    _waypointMeshRenderer.enabled = true;
                    _waypointMeshRenderer.material.color = _uav.uavColor;
                }
                
            }
            
            _lineRenderer.SetPosition(0, _uav.transform.position);

            if (preview)
                _lineRenderer.startWidth = _lineRenderer.endWidth = _pathsRenderingSettings.pathWidthReroutingOption;
            
            else if(_pathsRenderingSettings.showPathWhenReroutingOnly)
                _lineRenderer.startWidth = _lineRenderer.endWidth = 0;
            
            else
                _lineRenderer.startWidth = _lineRenderer.endWidth = _pathsRenderingSettings.pathWidthNormal;

            _lineRenderer.SetPosition(1, path.destinationWayPoint.transform.position+Vector3.zero);
        }

        private IEnumerator LineRendererUpdater()
        {
            while (true)
            {
                _lineRenderer.SetPosition(0, _uav.transform.position +Vector3.zero);
                yield return new WaitForSeconds(_pathsRenderingSettings.updateLineInterval);
            }
        }

        private void InitializeLineRenderer()
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = _lineRenderer.endWidth = _pathsRenderingSettings.pathWidthNormal;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
       
            _lineColor = _uav.uavColor;
            _lineColor.a = 0.5f; // make material translucent
            _lineRenderer.startColor = _lineRenderer.endColor = _lineColor;

            _lineRenderer.SetPosition(0, _uav.transform.position);
            _lineRenderer.SetPosition(1, _uav.transform.position);
            
            _updateLineRendererCoroutine = StartCoroutine(LineRendererUpdater());
        }

        private void OnDestroy()
        {
            if (_updateLineRendererCoroutine != null)
                StopCoroutine(_updateLineRendererCoroutine);

            if (_waypointMeshRenderer != null)
            {
                _waypointMeshRenderer.enabled = false;
            }
            UnsubscribeFromChannels();

        }

        public void ShowLineRenderers(bool  show)
        {
            var color = _lineRenderer.startColor;
            color.a = show ? 0.5f : 0; // make material translucent if show is false and opaque if true
            _lineRenderer.startColor = _lineRenderer.endColor = color;
        }

        
    }
}
