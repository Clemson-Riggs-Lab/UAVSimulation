using UnityEngine;

namespace UAVs.Camera.ScriptableObjects
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
	}
}