using IOHandlers.Records;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static ScriptableObjects.UAVs.Camera.UavCameraPanelSettingsSO;
using static ScriptableObjects.UAVs.Camera.UavCameraPanelSettingsSO.UavVideoArtifacts;
using static ScriptableObjects.UAVs.Camera.UavCameraPanelSettingsSO.TargetDetectionButtonSettings;

namespace ScriptableObjects.UAVs.Camera
{
	[CreateAssetMenu(fileName = "UavCameraSettings", menuName = "Settings/UavCameraSettings")]
	public class UavCameraPanelSettingsSO:ScriptableObject
	{
		public enum UavConditions //should be in order of priority (i.e., the last one will supersede the previous ones)
		{
			Flying,
			Descending,
			FallAndCrash,
			Disabled
		}
		public enum UavVideoArtifacts //should be in order of priority (i.e., the last one will supersede the previous ones)
	
		{
			None,
			DarkScreen,
			BlackScreen,
			Hide
		}

		public enum TargetDetectionButtonSettings //should be in order of priority (i.e., the last one will supersede the previous ones)
		{
			Enabled,
			Disabled,
			Hidden
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
		
		public UavCameraPanelConfigs healthyUavCameraPanelConfigs =
			new()
			{
				videoArtifacts = None
			};
		
		[Space (20)]
		public UavCameraPanelConfigs fuelLeakCameraPanelConfigs =
			new() { videoArtifacts = Hide, centerText = "Fuel Leaking"};
		
		[Space (20)]
		public UavCameraPanelConfigs fatalLeakCameraPanelConfigs =
			new() { videoArtifacts = None, bottomText = "Fatal Fuel Leak"};
		
		[Space (20)]
		public UavCameraPanelConfigs emptyFuelCameraPanelConfigs =
			new() { videoArtifacts = BlackScreen, bottomText = "Fuel Empty"};

		[Space (20)]
		public UavCameraPanelConfigs lostUavCameraPanelConfigs =
			new() { videoArtifacts = BlackScreen,centerText= " UAV Lost" };
		
		public UavCameraPanelConfigs hoveringUavCameraPanelConfigs =
			new() { videoArtifacts = DarkScreen, centerText = ""};
		
		[Space (20)]
		public UavCameraPanelConfigs visuallyDisabledUavCameraPanelConfigs =
			new() { videoArtifacts = DarkScreen};
	}
	
	[System.Serializable]
	public class UavCameraPanelConfigs
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public UavVideoArtifacts videoArtifacts= None;

		public string centerText = "";
		public string bottomText = "";
	}
}