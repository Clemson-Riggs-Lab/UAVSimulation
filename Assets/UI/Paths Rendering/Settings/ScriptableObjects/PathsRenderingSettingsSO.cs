using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Paths_Rendering.Settings.ScriptableObjects
{
	[CreateAssetMenu(fileName = "PathRenderingSettings", menuName = "Settings/Path Rendering Settings")]
	public class PathsRenderingSettingsSO:ScriptableObject
	{

		public bool showPathWhenUavIsHidden = false; //if true, the path will be shown even if the uav is hidden
		public bool showWaypointOnMap = true;
		public bool showPathWhenReroutingOnly = true;
		public float pathWidthNormal = 5f; // width of the path when rendering the path(s) the UAV is  following

		public float pathWidthReroutingOption = 8; // width of the path when it is a rendering a rerouting option
		public float updateLineInterval = 0.1f; // refreshes the line every 0.1 seconds

	}
}