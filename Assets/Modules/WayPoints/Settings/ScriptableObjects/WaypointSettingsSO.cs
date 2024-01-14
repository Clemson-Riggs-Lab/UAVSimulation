using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using static HelperScripts.Enums;

namespace WayPoints.Settings.ScriptableObjects
{
	[CreateAssetMenu(fileName = "WaypointSettings", menuName = "Settings/Waypoint Settings ")]
	public class WaypointSettingsSO : ScriptableObject
	{
		public int numberOfWaypoints = 100;
		public int waypointHeightFromTerrain = 150;
	}
}
 