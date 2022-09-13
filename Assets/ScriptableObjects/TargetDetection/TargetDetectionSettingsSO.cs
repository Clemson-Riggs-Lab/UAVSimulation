using IOHandlers.Records;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TargetDetection;
using UnityEngine;

namespace ScriptableObjects.TargetDetection
{
	[CreateAssetMenu(fileName = "TargetDetectionSettings", menuName = "Settings/TargetDetectionSettings")]
	public class TargetDetectionSettingsSO:ScriptableObject
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public ObjectType defaultTargetType = ObjectType.Box;
		
		[JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
		public Range TargetSpawnBufferRange { get; set; } = new Range(){ Min = 0, Max = 0 };

	}
}