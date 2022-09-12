using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.UAVs.Navigation.PathsRendering
{
	[CreateAssetMenu(fileName = "Path Rendering Settings", menuName = "Settings/Uav/Navigation/Path Rendering Settings")]
	public class PathsRenderingSettingsSO:ScriptableObject
	{
		

		public float pathWidthNormal = 5f;

		public float pathWidthReroutingOption = 8;
		
		public int numberOfWaypointsAheadToDraw = 2;

		public float updateLineInterval = 0.1f; // refreshes the line every 0.1 seconds

	}
}