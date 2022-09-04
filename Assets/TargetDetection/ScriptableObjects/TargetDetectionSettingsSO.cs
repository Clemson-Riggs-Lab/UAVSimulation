using UnityEngine;

namespace TargetDetection.ScriptableObjects
{
	[CreateAssetMenu(fileName = "TargetDetectionSettings", menuName = "Settings/TargetDetectionSettings")]
	public class TargetDetectionSettingsSO:ScriptableObject
	{
		public ObjectType DefaultTargetType { get; set; } = ObjectType.TargetBox;
		public ObjectType DefaultNonTargetType { get; set; } = ObjectType.NonTargetBox;
	}
}