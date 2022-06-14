using System;
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
        [HideInInspector] public bool isFinished;
        [HideInInspector] public float speed;
        
        //fields below are set when the path becomes active and when it is completed.
        [NonSerialized] public DateTime StartTime;
        [NonSerialized] public DateTime EndTime;
        
        

        public Path(int id, Uav uav,Waypoint startingWaypoint, Waypoint destinationWaypoint,float speed)
        {
            this.id = id;
            this.Uav = uav;
            this.StartingWaypoint = startingWaypoint;
            this.DestinationWaypoint = destinationWaypoint;
            
            startingWaypointID = startingWaypoint.ID;
            destinationWaypointID = destinationWaypoint.ID;
            
            isActive = false;
            this.speed= speed;
        }
        
        public void PathActivated()
        {
            StartTime = DateTime.Now;
            isActive = true;
            
        }   
        
        public void PathCompleted()
        {
            EndTime = DateTime.Now;
            isActive = false;
            isFinished = true;
        }
   
       
        
    
    }
}