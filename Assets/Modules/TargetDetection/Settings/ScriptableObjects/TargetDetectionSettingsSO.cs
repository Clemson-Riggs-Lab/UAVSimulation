using IOHandlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Modules.TargetDetection.Settings.ScriptableObjects
{
	[CreateAssetMenu(fileName = "TargetDetectionSettings", menuName = "Settings/TargetDetectionSettings")]
	public class TargetDetectionSettingsSO:ScriptableObject
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public ObjectType defaultTargetType = ObjectType.Box;
		
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public Range targetSpawnBufferRange = new Range(){ Min = 0, Max = 0 };

		
		[Space(10)]
		[Header("Target Detection Log Settings")]
		public bool logTargetDetection = true;
		public bool logTargetDetectionCorrectness= true;
		public bool logTimeSinceStartOfPathWhenTargetDetectionOccured= true;
	}
}