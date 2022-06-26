using System;
using IOHandlers;
using IOHandlers.Records;
using UnityEngine;
using Waypoints;
namespace UAVs
{
    public class Path
    {
        [HideInInspector] public Waypoint StartingWaypoint;
        [HideInInspector] public Waypoint DestinationWaypoint;
        
        [HideInInspector] public int startingWaypointID;
        [HideInInspector] public int destinationWaypointID;
        [HideInInspector] public Uav Uav;
        [HideInInspector] public int id;
        [HideInInspector] public bool isActive;
        [HideInInspector] public bool isUavVisuallyEnabled;
        [HideInInspector] public bool TargetIsPresent;
        [HideInInspector] public bool isFinished;
        [HideInInspector] public float speed=20f;
        
        //fields below are set when the path becomes active and when it is completed.
        [NonSerialized] public DateTime StartTime;
        [NonSerialized] public DateTime EndTime;
        
        

        public Path(int id, Uav uav,Waypoint startingWaypoint, Waypoint destinationWaypoint)
        {
            this.id = id;
            this.Uav = uav;
            this.StartingWaypoint = startingWaypoint;
            this.DestinationWaypoint = destinationWaypoint;
            
            startingWaypointID = startingWaypoint.Id;
            destinationWaypointID = destinationWaypoint.Id;
            
            isActive = false;
        }
        
        public void PathActivated()
        {
            var pathDuration = 0.1f;
           speed = (DestinationWaypoint.Transform.position - StartingWaypoint.Transform.position).magnitude / pathDuration;
            
            StartTime = DateTime.Now;
            isActive = true;
            Uav.isVisuallyEnabled = isUavVisuallyEnabled;
            Uav.SetUavVisuallyEnabled(isUavVisuallyEnabled == true);
           
            Debug.Log("Path " + id + " activated" + "by UAV " + Uav.ID + "from " + StartingWaypoint.Id + " to " + DestinationWaypoint.Id);
            
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