using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ScriptableObjects.UAVs;
using UnityEngine;
using static HelperScripts.Enums;

[CreateAssetMenu(fileName = "UavGeneralSettings", menuName = "Settings/Uav General Settings ")]
public class UavGeneralSettingsSO : ScriptableObject
{
	[JsonConverter(typeof(StringEnumConverter))]
	public InputRecordsSource uavRecordsSource = InputRecordsSource.FromInputFile;
	[JsonConverter(typeof(StringEnumConverter))]
	public UavSettingsDatabaseSO.UavNamingScheme namingScheme=UavSettingsDatabaseSO.UavNamingScheme.HashtagNumberOffsetZero;
	public bool colorUavLikePath = true;
}
