using System;
using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using IOHandlers;
using IOHandlers.Records;
using UnityEngine;
using Waypoints;

namespace UAVs
{
	public class PathsManager: MonoBehaviour
	{
		public enum NavType
		{
			InSequence,
			Other
		}
		
		[SerializeField] public const int NumberOfRounds = 10;
		[NonSerialized] public List<Path> AllPaths = new List<Path>();
		
		[SerializeField] public PathsGenerator pathsGenerator ;
		[SerializeField] public WaypointsManager waypointsManager;

		private void OnValidate()
		{
			MyDebug.AssertComponentReferencedInEditor(waypointsManager, this,this.gameObject);
			MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(pathsGenerator, this,this.gameObject);
		}

		public void GeneratePaths(NavType navType)
		{
			if (navType == NavType.InSequence)
			{
				var numberOfSteps = waypointsManager.waypoints.Count * NumberOfRounds;
				AllPaths= pathsGenerator.GenerateSequentialNavigationPaths(waypointsManager, numberOfSteps );
			}
			else
			{
				throw new System.NotImplementedException();
			}
		}

		public void GeneratePaths(List<UavPathsRecord> uavPathsRecords)
		{
			AllPaths= pathsGenerator.GeneratePaths(waypointsManager,uavPathsRecords);
		}
	}
}