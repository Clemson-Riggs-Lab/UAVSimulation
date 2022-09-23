using System;
using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace UAVs.Settings.ScriptableObjects
{
	[CreateAssetMenu(fileName = "UavGeneralSettings", menuName = "Settings/Uav General Settings ")]
	public class UavSettingsSO : ScriptableObject
	{
		public bool hideUavInMapWhenHidden =true;
		public bool hideUavInMapWhenLostOrFinished =true;
		public bool disableCollisionWithNFZWhenHidden = true;
		[JsonConverter(typeof(StringEnumConverter))]
		public Enums.InputRecordsSource uavRecordsSource = Enums.InputRecordsSource.FromInputFile;
		[JsonConverter(typeof(StringEnumConverter))]
		public UavNamingScheme namingScheme=UavNamingScheme.HashtagNumberOffsetZero;
		public bool colorUavLikePath = true;
	
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
		NatoName
	}
}