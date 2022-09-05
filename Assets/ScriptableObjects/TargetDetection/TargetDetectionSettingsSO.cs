using TargetDetection;
using UnityEngine;

namespace ScriptableObjects.TargetDetection
{
	[CreateAssetMenu(fileName = "TargetDetectionSettings", menuName = "Settings/TargetDetectionSettings")]
	public class TargetDetectionSettingsSO:ScriptableObject
	{
		public ObjectType defaultTargetType = ObjectType.Box;
	}
}