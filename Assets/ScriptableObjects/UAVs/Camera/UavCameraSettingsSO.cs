using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static ScriptableObjects.UAVs.Camera.UavCameraSettingsSO;
using static ScriptableObjects.UAVs.Camera.UavCameraSettingsSO.UavVideoArtifacts;
using static ScriptableObjects.UAVs.Camera.UavCameraSettingsSO.TargetDetectionButtonSettings;

namespace ScriptableObjects.UAVs.Camera
{
	[CreateAssetMenu(fileName = "UavCameraSettings", menuName = "Settings/UavCameraSettings")]
	public class UavCameraSettingsSO:ScriptableObject
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
		
		[Header("Uav Camera Settings")]
		public int renderTextureWidth = 255;
		public int renderTextureHeight = 255;
		public int renderTextureDepth = 24;
    
		public int rotationX = 10;
		public int fieldOfView = 60 ;
		
		[Space (40)]
		[Header("Uav Camera and Target Detection Panel Settings For Different Uav Conditions")]
		
		[Space (20)]
		public UavCameraAndTargetDetectionConfigs healthyUavCameraAndTargetDetectionConfigs =
			new() { videoArtifacts = None};
		
		[Space (20)]
		public UavCameraAndTargetDetectionConfigs fuelLeakCameraAndTargetDetectionConfigs =
			new() { videoArtifacts = Hide, centerText = "Fuel Leaking"};
		
		[Space (20)]
		public UavCameraAndTargetDetectionConfigs fatalLeakCameraAndTargetDetectionConfigs =
			new() { videoArtifacts = None, bottomText = "Fatal Fuel Leak"};
		
		[Space (20)]
		public UavCameraAndTargetDetectionConfigs emptyFuelCameraAndTargetDetectionConfigs =
			new() { videoArtifacts = BlackScreen, bottomText = "Fuel Empty"};

		[Space (20)]
		public UavCameraAndTargetDetectionConfigs lostUavCameraAndTargetDetectionConfigs =
			new() { videoArtifacts = BlackScreen,centerText= " UAV Lost" };
		
		public UavCameraAndTargetDetectionConfigs hoveringUavCameraAndTargetDetectionConfigs =
			new() { videoArtifacts = DarkScreen, centerText = ""};
		
		[Space (20)]
		public UavCameraAndTargetDetectionConfigs visuallyDisabledUavCameraAndTargetDetectionConfigs =
			new() { videoArtifacts = DarkScreen};
	}
	
	[System.Serializable]
	public class UavCameraAndTargetDetectionConfigs
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public UavVideoArtifacts videoArtifacts= None;
		[JsonConverter(typeof(StringEnumConverter))]

		public string centerText = "";
		public string bottomText = "";
	}
}