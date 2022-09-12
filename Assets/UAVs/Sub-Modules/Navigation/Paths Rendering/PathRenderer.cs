using System;
using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using ScriptableObjects.UAVs.Navigation.PathsRendering;
using UAVs;
using UAVs.Sub_Modules.Navigation;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    private Uav uav;
    private LineRenderer lineRenderer;
    PathsRenderingSettingsSO pathsRenderingSettings;
    private Color lineColor;
    private Coroutine updateLineRendererCoroutine;
    private Vector3 lineOffset = Vector3.zero;

    public void Initialize(Uav uav)
    {
        pathsRenderingSettings = GameManager.Instance.settingsDatabase.uavSettings.pathsRenderingSettings;

        if (!TryGetComponent(out lineRenderer))
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        this.uav = uav;
        InitializeLineRenderer();
    }

    public void SetLineRenderer(Path uavPath, bool preview=false)
    {
        lineRenderer.SetPosition(0, uav.transform.position);

        if (preview)
            lineRenderer.startWidth = lineRenderer.endWidth = pathsRenderingSettings.pathWidthReroutingOption;
        else
            lineRenderer.startWidth = lineRenderer.endWidth = pathsRenderingSettings.pathWidthNormal;

        var path= uavPath;
        for(var i=1; i<lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, path.destinationWayPoint.transform.position+lineOffset);
            path = path.nextPath;
        }

       
        
    }

    private IEnumerator LineRendererUpdater()
    {
        while (true)
        {
            lineRenderer.SetPosition(0, uav.transform.position + lineOffset);
            yield return new WaitForSeconds(pathsRenderingSettings.updateLineInterval);
        }
    }

    private void InitializeLineRenderer()
    {
        lineRenderer.positionCount = pathsRenderingSettings.numberOfWaypointsAheadToDraw + 1;
        lineRenderer.startWidth = lineRenderer.endWidth = pathsRenderingSettings.pathWidthNormal;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
       
        lineColor = uav.uavColor;
       // make material translucent
        lineColor.a = 0.5f;
        lineRenderer.startColor = lineRenderer.endColor = lineColor;
      
        var offset = (-8 + uav.id);
        lineOffset = new Vector3(offset, 0, 0);
        
        updateLineRendererCoroutine = StartCoroutine(LineRendererUpdater());
    }

    private void OnDestroy()
    {
        if (updateLineRendererCoroutine != null)
            StopCoroutine(updateLineRendererCoroutine);
    }
}
