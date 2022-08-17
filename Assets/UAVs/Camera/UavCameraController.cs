using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UavCameraController : MonoBehaviour
{
    public int renderTextureWidth = 255;//TODO make not static (get from settings)
    public int renderTextureHeight = 255;//TODO make not static (get from settings)
    public int renderTextureDepth = 24;//TODO make not static (get from settings)
    
    public int rotationX = 75;//TODO make not static (get from settings)
    public int fieldOfView = 60 ;//TODO make not static (get from settings)
    
    public bool DisableWhenUavIsNotVisible = true; //TODO make not static (get from settings)
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeCamera(out RenderTexture renderTexture)
    {
        var uavCamera=gameObject.GetComponent<Camera>();
        var targetTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, renderTextureDepth);
        targetTexture.Create();
        uavCamera.targetTexture= targetTexture;
        renderTexture = targetTexture;
        
        uavCamera.fieldOfView = fieldOfView;
        uavCamera.transform.localEulerAngles = new Vector3(rotationX, 0, 0);
    }
}
