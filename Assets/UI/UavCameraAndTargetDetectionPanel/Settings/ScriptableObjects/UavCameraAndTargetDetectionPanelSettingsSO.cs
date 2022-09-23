using IOHandlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static UI.UavCameraAndTargetDetectionPanel.Settings.ScriptableObjects.UavCameraAndTargetDetectionPanelSettingsSO;
using static UI.UavCameraAndTargetDetectionPanel.Settings.ScriptableObjects.UavCameraAndTargetDetectionPanelSettingsSO.UavCameraAndTargetDetectionPanelState;

namespace UI.UavCameraAndTargetDetectionPanel.Settings.ScriptableObjects
{
	[CreateAssetMenu(fileName = "UavCameraSettings", menuName = "Settings/UavCameraSettings")]
	public class UavCameraAndTargetDetectionPanelSettingsSO:ScriptableObject
	{
	
		public enum UavCameraAndTargetDetectionPanelState //should be in order of priority (i.e., the last one will supersede the previous ones)
	
		{
			None,
			DarkScreen,
			BlackScreen,
			Hide
		}

		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public Range CameraBlankDurationBetweenPaths { get; set; } = new Range() { Min = 0, Max = 0 }; 
		
		[Header("Uav Camera Settings")]
		public int renderTextureWidth = 255;
		public int renderTextureHeight = 255;
		public int renderTextureDepth = 24;
    
		public int cameraDownwardTilt = 10;
		public int cameraFieldOfView = 60 ;
		
		[Space (40)]
		[Header("Uav Camera and Target Detection Panel Settings For Different Uav Conditions")]
		
		[Space (20)]
		public UavCameraPanelConfigs enabledUavCameraAndTargetDetectionPanelConfigs =
			new() { cameraAndTargetDetectionPanelState = None };
		
		[Space (20)]
		public UavCameraPanelConfigs hiddenUavCameraAndTargetDetectionPanelConfigs =
			new() { cameraAndTargetDetectionPanelState = DarkScreen};
		
		[Space (20)]
		public UavCameraPanelConfigs finishedUavCameraAndTargetDetectionPanelConfigs =
			new() { cameraAndTargetDetectionPanelState = DarkScreen, centerText = "Uav Finished Mission"};
		
		[Space (20)]
		public UavCameraPanelConfigs lostUavCameraAndTargetDetectionPanelConfigs =
			new() { cameraAndTargetDetectionPanelState = BlackScreen,centerText= " UAV Lost" };
		
		[Space (20)]
		public UavCameraPanelConfigs hoveringUavCameraAndTargetDetectionPanelConfigs =
			new() { cameraAndTargetDetectionPanelState = DarkScreen, centerText = ""};
		
		[Space (20)]
		public UavCameraPanelConfigs fuelLeakCameraAndTargetDetectionPanelConfigs =
			new() { cameraAndTargetDetectionPanelState = None, centerText = "Fuel Leaking"};
		
		[Space (20)]
		public UavCameraPanelConfigs fatalLeakCameraAndTargetDetectionPanelConfigs =
			new() { cameraAndTargetDetectionPanelState = None, bottomText = "Fatal Fuel Leak"};
		
		[Space (20)]
		public UavCameraPanelConfigs emptyFuelCameraAndTargetDetectionPanelConfigs =
			new() { cameraAndTargetDetectionPanelState = BlackScreen, bottomText = "Fuel Empty"};

	
		
	
		
		
	}
	
	[System.Serializable]
	public class UavCameraPanelConfigs
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public UavCameraAndTargetDetectionPanelState cameraAndTargetDetectionPanelState= None;

		public string centerText = "";
		public string bottomText = "";
	}
}