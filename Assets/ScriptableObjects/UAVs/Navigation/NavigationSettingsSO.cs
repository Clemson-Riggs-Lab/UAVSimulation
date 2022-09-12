using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ScriptableObjects.UAVs.Navigation
{
	[CreateAssetMenu(fileName = "NavigationSettings", menuName = "Settings/NavigationSettings")]
    public class NavigationSettingsSO:ScriptableObject
    {

	    public enum NavigationType 
	    {
		    BasedOnInputFile, BasedOnDefaultInputFile
	    }
	    
	    public enum FollowType
	    {
		    Simple, // Just move object as it is (without any rotation or dumping)
		    Facing, // Roughly face object on current waypoint
		    SmoothFacing, // Face object on current waypoint and adapt path smoothly 
		    SmoothDamping, // Speed will be decreased before each waypoint and accelerate after it 
		    Teleport, // Simply teleport (transfer immediately) object to current waypoint position
	    }
		
	    public enum LoopType
	    {
		    Once, // Only one cycle
		    Cycled, // Infinite amounts of cycles
		    SeveralTimes // Repeat loop several times (specified in numberOfLoops)
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

	    public enum SpeedMode
	    {
		    FixedSpeed, FixedPathDuration
	    }
	    
	    [Space(20)]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public NavigationType navigationType = NavigationType.BasedOnDefaultInputFile;
	   
	    [Space(20)]
	   
	    [Header("Speed Mode (either Fixed Speed or Fixed Path Duration)")]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public SpeedMode speedMode = SpeedMode.FixedPathDuration;
	    [Tooltip("Not used in Fixed Path Duration Mode")]
	    public float fixedSpeed=15;
	    [Tooltip("Not used in Fixed Speed Mode")]
	    public float pathDuration=10;
	    
	    [Space(20)]
	    
	    [Header("How the UAV should follow the path")]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public FollowType followingType= FollowType.SmoothFacing; // Choose one of following type to use
	    [Tooltip("Speed of rotation when following type is set to SmoothFacing")]
	    public float rotationDuration=2;
	   
	    [Space(20)]
	    
	    [Header("How many times should the path be repeated")]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public LoopType loopingType= LoopType.Once ; // Choose one of looping type to use
	    [Tooltip("Neglected unless looping type is set to SeveralTimes")]
	    public int numberOfLoops=0; // How much loops should be performed before stop. Use this parameter if loopingType=SeveralTimes
	    
	    [Space(20)]
	    
	    [Tooltip("Ignore waypoint position on selected axis")]
	    public UsedAxis ignorePositionAtAxis; // Ignore waypoint position along those axis
	    
	    [Space(20)]
	    
	    [Header("How the UAV should hover over waypoints")]
	    [JsonConverter(typeof(StringEnumConverter))]
	    public  WaypointHoveringType waypointHoveringType= WaypointHoveringType.FaceNextWaypoint; // Choose one of hovering type to use
	    [Tooltip("Neglected if hovering type is set to None")]
	    public float hoverDurationOnWaypoint = 4;// Duration of hovering on waypoint (in seconds), set to 0 to disable hovering
	    [Tooltip("Neglected if hovering type not set to UseAngle")]
	    public float hoverAngle = 360; // Angle of hovering (in degrees)
	    [Tooltip("Neglected if hovering type not set to UseSpeed")]
	    public float hoverSpeed = 1; // Speed of hovering (in degrees per second)
	    
    }
}
