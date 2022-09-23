using System.Collections;
using HelperScripts;
using Modules.Navigation;
using UAVs;
using UI.Paths_Rendering.Settings.ScriptableObjects;
using UnityEngine;

namespace UI.Paths_Rendering
{
    public class PathRenderer : MonoBehaviour
    {
        private Uav _uav;
        private LineRenderer _lineRenderer;
        PathsRenderingSettingsSO _pathsRenderingSettings;
        private Color _lineColor;
        private Coroutine _updateLineRendererCoroutine;
        private Vector3 _lineOffset = Vector3.zero;

        public void Initialize(Uav uav)
        {
            _uav = uav;
            GetReferencesFromGameManager();
            _lineRenderer = gameObject.GetOrAddComponent<LineRenderer>();
            InitializeLineRenderer();
        }
        private void GetReferencesFromGameManager()
        {
            _pathsRenderingSettings = GameManager.Instance.settingsDatabase.pathsRenderingSettings;
        }

  

   
        public void SetLineRenderer(Path path, bool preview=false)
        {
            _lineRenderer.SetPosition(0, _uav.transform.position);

            if (preview)
                _lineRenderer.startWidth = _lineRenderer.endWidth = _pathsRenderingSettings.pathWidthReroutingOption;
            else
                _lineRenderer.startWidth = _lineRenderer.endWidth = _pathsRenderingSettings.pathWidthNormal;
        
            for(var i=1; i<_lineRenderer.positionCount; i++)
            {
                _lineRenderer.SetPosition(i, path.destinationWayPoint.transform.position+_lineOffset);
                path = path.nextPath;
            }

       
        
        }

        private IEnumerator LineRendererUpdater()
        {
            while (true)
            {
                _lineRenderer.SetPosition(0, _uav.transform.position + _lineOffset);
                yield return new WaitForSeconds(_pathsRenderingSettings.updateLineInterval);
            }
        }

        private void InitializeLineRenderer()
        {
            _lineRenderer.positionCount = _pathsRenderingSettings.numberOfWaypointsAheadToDraw + 1;
            _lineRenderer.startWidth = _lineRenderer.endWidth = _pathsRenderingSettings.pathWidthNormal;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
       
            _lineColor = _uav.uavColor;
            _lineColor.a = 0.5f; // make material translucent
            _lineRenderer.startColor = _lineRenderer.endColor = _lineColor;
      
            var offset = (-8 + _uav.id);
            _lineOffset = new Vector3(offset, 0, 0);
        
            _updateLineRendererCoroutine = StartCoroutine(LineRendererUpdater());
        }

        private void OnDestroy()
        {
            if (_updateLineRendererCoroutine != null)
                StopCoroutine(_updateLineRendererCoroutine);
        }

        public void ShowLineRenderers(bool  show)
        {
            var color = _lineRenderer.startColor;
            color.a = show ? 0.5f : 0; // make material translucent if show is false and opaque if true
            _lineRenderer.startColor = _lineRenderer.endColor = color;
        }

    }
}
