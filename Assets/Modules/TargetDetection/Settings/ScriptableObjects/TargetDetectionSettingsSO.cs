using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Serialization;
using Range = IOHandlers.Range;

namespace Modules.TargetDetection.Settings.ScriptableObjects
{
	[CreateAssetMenu(fileName = "TargetDetectionSettings", menuName = "Settings/TargetDetectionSettings")]
	public class TargetDetectionSettingsSO:ScriptableObject
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public ObjectType defaultTargetType = ObjectType.Box;
		
		public Range targetSpawnBufferRangeInSeconds = new Range(){ Min = 5, Max = 10};
		public Range targetRandomDeviationFromCenter = new Range(){ Min = 0, Max = 20};

		[Space(10)] [Header("Target Detection Default Frequency Settings")]
		public float ratioOfActiveFeedsWithTarget = .5f;
		public float ratioOfActiveFeedsWithNonTarget = .5f;
		public int numberOfActiveUavsForTargetDetection = 4;
		
		[Space(10)]
		[Header("Target Detection Log Settings")]
		public bool logTargetDetection = true;
		public bool logTargetDetectionCorrectness= true;
		public bool logTimeSinceStartOfPathWhenTargetDetectionOccured= true;
		public bool logIfNonTargetIsPresent = true;
		public string targetColor= "darkBlue";
		public string nonTargetColor= "lightBlue";
		public int targetPositioningRandomGeneratorSeed = 1;
		public int targetDistanceRandomGeneratorSeed = 1;
	}
}