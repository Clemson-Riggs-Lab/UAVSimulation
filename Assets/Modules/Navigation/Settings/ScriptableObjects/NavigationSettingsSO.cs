using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Serialization;
using static HelperScripts.Enums;
using static HelperScripts.Enums.InputRecordsSource;
using static Modules.Navigation.Settings.ScriptableObjects.NavigationSettingsSO.FollowType;
using static Modules.Navigation.Settings.ScriptableObjects.NavigationSettingsSO.WaypointHoveringType;

namespace Modules.Navigation.Settings.ScriptableObjects
{
	[CreateAssetMenu(fileName = "NavigationSettings", menuName = "Settings/NavigationSettings")]
    public class NavigationSettingsSO:ScriptableObject
    {

	  
	    
	    public enum FollowType
	    {
		    Simple, // Just move object as it is (without any rotation or dumping)
		    Facing, // Roughly face object on current waypoint
		    SmoothFacing, // Face object on current waypoint and adapt path smoothly 
		    SmoothDamping, // Speed will be decreased before each waypoint and accelerate after it 
		    Teleport, // Simply teleport (transfer immediately) object to current waypoint position
	    }

	    [Serializable]
	    public struct UsedAxis
	    {
		    public bool x;
		    public bool y;
		    public bool z;
	    }

	    public enum WaypointHoveringType
	    {
		    None, UseAngle, UseSpeed, FaceNextWaypoint
	    }
	    
	     public int numberOfActiveUavsForRerouting = 10;
	    [Space(20)]
	    
	    public float fixedSpeed=15;

	    public float maxPathDuration = 35; // Max duration of path (in seconds), set to 0 to disable
	    public float minPathDuration = 15; // Min duration of path (in seconds), set to 0 to disable
	    public double minDistanceFromNFZInDuration= 7; // Min distance from NFZ in duration (in seconds), set to 0 to disable
	    [Space(20)]
	    
	    [Header("How the UAV should follow the path")]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public FollowType followingType= SmoothFacing; // Choose one of following type to use
	    [Tooltip("Speed of rotation when following type is set to SmoothFacing")]
	    public float rotationDuration=2;

	    [Space(20)]
	    [Tooltip("Ignore waypoint position on selected axis")]
	    public UsedAxis ignoreWaypointPositionOnAxis; // Ignore waypoint position along those axis
	    
	    [Space(20)]
	    
	    [Header("How the UAV should hover over waypoints")]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public  WaypointHoveringType waypointHoveringType= FaceNextWaypoint; // Choose one of hovering type to use
	    [Tooltip("Neglected if hovering type is set to None")]
	    public float hoverDurationOnWaypoint = 2;// Duration of hovering on waypoint (in seconds), set to 0 to disable hovering
	    [Tooltip("Neglected if hovering type not set to UseAngle")]
	    public float hoverAngle = 360; // Angle of hovering (in degrees)
	    [Tooltip("Neglected if hovering type not set to UseSpeed")]
	    public float hoverSpeed = 1; // Speed of hovering (in degrees per second)

	    public int shufflingPathsRandomGeneratorSeed = 1;
	    public int targetAndNFZProbabilityRandomGeneratorSeed = 1;
	    public int uavSelectionShuffleRandomNumberGeneratorSeed = 1;
	   
    }
}
