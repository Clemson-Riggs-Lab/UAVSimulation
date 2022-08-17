using System;
using System.Collections.Generic;
using System.Linq;
using HelperScripts;
using IOHandlers.Records;
using UnityEngine;
using WayPoints;

namespace UAVs.Navigation
{
    public class PathsGenerator : MonoBehaviour
    {

        [SerializeField]public UavsManager uavsManager;
        [SerializeField]public WayPointsManager wayPointsManager;

        private void Start()
        {
            uavsManager= GameManager.Instance.uavsManager;
            wayPointsManager= GameManager.Instance.wayPointsManager;
            MyDebug.AssertObjectReferenceObtainedFromGameManager(uavsManager,this, gameObject);
            MyDebug.AssertObjectReferenceObtainedFromGameManager(wayPointsManager,this, gameObject);
            
        }

        public List<Navigator> GenerateSequentialNavigationPaths(int numberOfIterations)
        { 
            
            // this is the travelling salesman problem, However, for now I have hardcoded the path based on a grid of 4x4 WayPoints.
            //move in the following order: 0,1,2,3,7,11,15,14,13,12,8,9,10,6,5,4,0, repeat
            
            //Temporary code: //TODO make better algorithm
            List<int> wayPointsOrderedList = new List<int>(){ 0, 1, 2, 3, 7, 11, 15, 14, 13, 12, 8, 9, 10, 6, 5, 4 };
            var countOfWayPoints = wayPointsOrderedList.Count;
            
            var navigators= new List<Navigator>();
            foreach (var uav in uavsManager.Uavs)
            {
                var navigator = AddNavigatorScript(uav);
                navigators.Add(navigator);
                
                var uavPaths = new List<Path>();
                WayPoints.WayPoint startingWayPoint, endingWayPoint = null;
                
                startingWayPoint = uav.startingWaypoint; //setting the starting WayPoint to the last WayPoint visited by the uav
                var startingWayPointIDIndex = wayPointsOrderedList.IndexOf(startingWayPoint.Id);
                wayPointsManager.TryGetWayPoint(wayPointsOrderedList[(startingWayPointIDIndex+1)% countOfWayPoints], out  endingWayPoint ); //setting the ending WayPoint to the next WayPoint in the list
               
                var pathIdIterator = 0;
                for (var i = startingWayPointIDIndex; i < numberOfIterations + startingWayPointIDIndex; i++)
                {
                    var path = new Path(pathIdIterator,uav,startingWayPoint, endingWayPoint);
                    wayPointsManager.TryGetWayPoint(wayPointsOrderedList[(i + 1) % countOfWayPoints], out endingWayPoint);
                    uavPaths.Add(path);
                    pathIdIterator++;
                    startingWayPoint = endingWayPoint;
                }
                navigator.paths.AddRange(uavPaths);
                
            }
            return navigators;
        }

       private Navigator AddNavigatorScript(Uav uav)
       {
              var navigator = uav.gameObject.AddComponent<Navigator>();
              return navigator;
       }


       public List<Navigator> GeneratePaths( List<UavPathsRecord> uavPathsRecords)
       {
           var navigators = new List<Navigator>();
              foreach (var uavPathsRecord in uavPathsRecords)
              { 
                var uav = uavsManager.Uavs.FirstOrDefault(x => x.ID == uavPathsRecord.UavId);
                
                if (uav == null)
                {
                    Debug.LogError("UAV not found while initializing paths from uavPathsRecords");
                    continue; //returning because the uav was not found (this should not happen)
                }
                var navigator= AddNavigatorScript(uav);
                
                var paths = new List<Path>();
                var pathID = 0;
                var startingWayPoint = uav.startingWaypoint;
                foreach (var record in uavPathsRecord.PathRecords)
                {
                     
                     wayPointsManager.TryGetWayPoint(record.DestinationWayPointID, out WayPoints.WayPoint endingWayPoint);
                     var path = new Path(pathID, uav, startingWayPoint, endingWayPoint);
                     path.Initialize(record);
                     paths.Add(path);
                     pathID++;
                     startingWayPoint = endingWayPoint;
                }
                navigator.paths.AddRange(paths);
                navigators.Add(navigator);
                
              }
          
           return navigators;
       }
    }
}