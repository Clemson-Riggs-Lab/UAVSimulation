using System;
using IOHandlers.Records;
using UnityEngine;
using WayPoints;

namespace UAVs.Sub_Modules.Navigation
{
    public class Path
    {
        [HideInInspector] public WayPoint StartingWayPoint;
        [HideInInspector] public WayPoint DestinationWayPoint;
        
        [HideInInspector] public int startingWayPointID;
        [HideInInspector] public int destinationWayPointID;
        [HideInInspector] public Uav Uav;
        [HideInInspector] public int id;
        [HideInInspector] public bool isActive;
        [HideInInspector] public bool isUavVisuallyEnabled;
        [HideInInspector] public bool TargetIsPresent;
        [HideInInspector] public bool isFinished;
        [HideInInspector] public float speed=1f;
        
        //fields below are set when the path becomes active and when it is completed.
        [NonSerialized] public DateTime StartTime;
        [NonSerialized] public DateTime EndTime;
        
        

        public Path(int id, Uav uav,WayPoint startingWayPoint, WayPoint destinationWayPoint)
        {
            this.id = id;
            this.Uav = uav;
            this.StartingWayPoint = startingWayPoint;
            this.DestinationWayPoint = destinationWayPoint;
            
            startingWayPointID = startingWayPoint.Id;
            destinationWayPointID = destinationWayPoint.Id;
            
            isActive = false;
        }
        
        public void PathActivated()
        {
            var pathDuration = 10f;
           speed = (DestinationWayPoint.transform.position - StartingWayPoint.transform.position).magnitude / pathDuration;
            
            StartTime = DateTime.Now;
            isActive = true;
            Uav.isVisuallyEnabled = isUavVisuallyEnabled;
            Uav.SetUavVisuallyEnabled(isUavVisuallyEnabled == true);
           
            Debug.Log("Path " + id + " activated" + "by UAV " + Uav.ID + "from " + StartingWayPoint.Id + " to " + DestinationWayPoint.Id);
            
        }   
        
        public void PathCompleted()
        {
            EndTime = DateTime.Now;
            isActive = false;
            isFinished = true;
        }


        public void Initialize(UavPathsRecord.PathRecord record)
        {
           this.isUavVisuallyEnabled= record.UavVisuallyEnabled??false;
           this.TargetIsPresent = record.TargetIsPresent??false;
        }
    }
}