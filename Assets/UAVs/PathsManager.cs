using System;
using System.Collections.Generic;
using Helper_Scripts;
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
		[SerializeField]public float speed = 20f;
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
				AllPaths= pathsGenerator.GenerateSequentialNavigationPaths(waypointsManager, numberOfSteps,speed );
			}
			else
			{
				throw new System.NotImplementedException();
			}
		}
	}
}