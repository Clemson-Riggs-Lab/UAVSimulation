using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Waypoints;

namespace UAVs
{
	public class Uav : MonoBehaviour
	{
		private int _id = 999;
		[NonSerialized] public string codeName;
		[NonSerialized] public string abbrvName;


		[NonSerialized] public List<Path> Paths = new List<Path>();
		[NonSerialized] public Path CurrentPath;
		[NonSerialized] public Waypoint LastWaypointVisited;
		[NonSerialized] public bool isActive;

		public int ID
		{
			get => _id;
			set => SetIDandNames(value);
		}

		public void Initialize(int id, Waypoint waypoint)
		{
			this.ID = id;
			transform.localPosition =
				waypoint.transform.position; // TODO: check if just position is enough instead of local position
			LastWaypointVisited = waypoint;
			this.isActive = false;
		}


		private void SetIDandNames(int value)
		{
			_id = value;
			abbrvName = NatoAlphabetConverter.IntToLetters(value);
			codeName = NatoAlphabetConverter.LettersToName(abbrvName);
		}

		public void Navigate()
		{
			isActive = true;

			if (CurrentPath == null)
			{
				if (Paths.Count == 0)
				{
					Debug.LogError("No paths found");
					return;
				}
				else
				{
					CurrentPath = Paths[0];
					CurrentPath.PathActivated();
				}
			}
		}

		//while paths are not empty, check if current path is finished, if it is, then start the next path
		public void Update()
		{
			if (!isActive || Paths.Count == 0) return;

			//move uav towards next waypoint
			if (CurrentPath != null)
			{
				transform.position = Vector3.MoveTowards(transform.position,
					CurrentPath.DestinationWaypoint.Transform.position, Time.deltaTime * CurrentPath.speed);

				//check if uav is close to waypoint
				if (Vector3.Distance(transform.position, CurrentPath.DestinationWaypoint.Transform.position) < 0.1f)
				{
					LastWaypointVisited = CurrentPath.DestinationWaypoint;
					CurrentPath.PathCompleted();
					Paths.Remove(CurrentPath);
					if (Paths.Count > 0)
					{
						CurrentPath = Paths[0];
						CurrentPath.PathActivated();
					}
					else
					{
						CurrentPath = null;
						isActive = false;
					}
				}
			}


		}
	}
}