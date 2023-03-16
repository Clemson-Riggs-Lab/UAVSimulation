using System;
using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace UAVs.Settings.ScriptableObjects
{
	[CreateAssetMenu(fileName = "UavGeneralSettings", menuName = "Settings/Uav General Settings ")]
	public class UavSettingsSO : ScriptableObject
	{
		public int numberOfUaVsToMaintain = 16;
		public bool hideUavInMapWhenHidden =true;
		public bool hideUavInMapWhenLost =true;
		public bool disableCollisionWithNFZWhenHidden = true;
		[JsonConverter(typeof(StringEnumConverter))]
		public UavNamingScheme namingScheme=UavNamingScheme.HashtagNumberOffsetZero;
		public bool colorUavLikePath = true;
		public int maxUavNumberAfterName=9;
		public int waypointsShuffleRandomGeneratorSeed = 1;

		[Space(20)]
		[Header("Logging Settings")]
		public bool logUavCreationEvents= true;
		public bool logUavDestroyEvents= true;
		public bool logUavConditionChangedEvents= true;
	}

	public enum UavNamingScheme
	{
		UavAndNumber,
		UavAndNumberOffsetZero,
		HashtagNumber,
		HashtagNumberOffsetZero,
		Letter,
		NatoName,
		NatoNameWithRandomNumber
	}
}