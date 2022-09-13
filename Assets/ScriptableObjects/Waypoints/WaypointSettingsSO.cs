using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static HelperScripts.Enums;

namespace ScriptableObjects.Waypoints
{
	[CreateAssetMenu(fileName = "WaypointSettings", menuName = "Settings/Waypoint Settings ")]
	public class WaypointSettingsSO : ScriptableObject
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public InputRecordsSource waypointsRecordsSource = InputRecordsSource.FromInputFile;
	}
}
