using UnityEngine;

namespace ScriptableObjects.UAVs.Camera
{
	[CreateAssetMenu(fileName = "UavCameraSettings", menuName = "Settings/UavCameraSettings")]
	public class UavCameraSettingsSO:ScriptableObject
	{
		public enum UavConditions
		{
			Flying,
			Descending,
			FallAndCrash,
			Disabled
		}
		public enum UavVideoArtifacts
		{
			None,
			Shake,
			ConnectionIssues,
			BlackScreen,
			NoSignal,
			Noise,
			AllNoise,
			Hide
		}
		
		[Header("Uav Camera Settings")]
		public int renderTextureWidth = 255;
		public int renderTextureHeight = 255;
		public int renderTextureDepth = 24;
    
		public int rotationX = 20;
		public int fieldOfView = 60 ;
		public bool disableWhenUavIsNotVisible = true;
	}
}