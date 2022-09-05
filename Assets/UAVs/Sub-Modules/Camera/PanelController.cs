using System.Collections;
using System.Collections.Generic;
using UAVs;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public Toggle targetDetectedToggle;
    public Toggle targetNotDetectedToggle;
    public Uav uav;
    
    public void ResetToggleState()
    {
        targetDetectedToggle.isOn = false;
        targetNotDetectedToggle.isOn = true;
    }
    
    public void OnTargetDetectedToggleClicked()
    {
        
    }
}
