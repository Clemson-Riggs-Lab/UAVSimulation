using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Waypoints;

namespace UAVs
{
    public class PathsGenerator : MonoBehaviour
    {

        [SerializeField]public UavsManager uavsManager;

       public List<Path> GenerateSequentialNavigationPaths(WaypointsManager waypointsManager,int numberOfIterations, float speed)
        { // this is the travelling salesman problem, However, for now I have hardcoded the path based on a grid of 4x4 waypoints.
            //move in the following order: 0,1,2,3,7,11,15,14,13,12,8,9,10,6,5,4,0, repeat
            List<int> waypointsOrderedList = new List<int>(){ 0, 1, 2, 3, 7, 11, 15, 14, 13, 12, 8, 9, 10, 6, 5, 4 };
            var countOfWaypoints = waypointsOrderedList.Count;
            
            var allPaths = new List<Path>();
 
            foreach (var uav in uavsManager.Uavs)
            {
                var uavPaths = new List<Path>();
                Waypoint startingWaypoint, endingWaypoint = null;
                
                startingWaypoint = uav.LastWaypointVisited; //setting the starting waypoint to the last waypoint visited by the uav
                var startingWaypointIDIndex = waypointsOrderedList.IndexOf(startingWaypoint.ID);
                waypointsManager.TryGetWaypoint(waypointsOrderedList[(startingWaypointIDIndex+1)% countOfWaypoints], out  endingWaypoint ); //setting the ending waypoint to the next waypoint in the list
               
                var pathID = 0;
                for (var i = startingWaypointIDIndex; i < numberOfIterations + startingWaypointIDIndex; i++)
                {
                    var path = new Path(pathID,uav,startingWaypoint, endingWaypoint, speed);
                    startingWaypoint = endingWaypoint;
                    waypointsManager.TryGetWaypoint(waypointsOrderedList[(i + 1) % countOfWaypoints], out endingWaypoint);
                    uavPaths.Add(path);
                    pathID++;
                }
                uav.Paths.AddRange(uavPaths);
                allPaths.AddRange(uavPaths);
            }
            return allPaths;
        }
        


       

        
    }
}