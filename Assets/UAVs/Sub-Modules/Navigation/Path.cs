using System;
using IOHandlers.Records;
using UnityEngine;
using WayPoints;

namespace UAVs.Sub_Modules.Navigation
{
    public class Path
    {
        [HideInInspector] public WayPoint destinationWayPoint;
        
        [HideInInspector] public int id;
        [HideInInspector] public Uav uav;
        [HideInInspector] public bool uavIsVisuallyEnabled;
        [HideInInspector] public bool targetIsPresent;
        
        public Path previousPath;
        public Path nextPath;
        
        //fields below are set when the path becomes active and when it is completed.
        [NonSerialized] public DateTime startTime;
        [NonSerialized] public DateTime endTime;
        

        public Path(int id, Uav uav, WayPoint destinationWayPoint, bool visuallyEnabled, bool targetIsPresent)
        {
            this.id = id;
            this.uav = uav;
            this.destinationWayPoint = destinationWayPoint;
            this.uavIsVisuallyEnabled = visuallyEnabled;
            this.targetIsPresent = targetIsPresent;
        }

        public Path(Path path)
        {
            this.id = path.id;
            this.uav = path.uav;
            this.destinationWayPoint = path.destinationWayPoint;
            this.uavIsVisuallyEnabled = path.uavIsVisuallyEnabled;
            this.targetIsPresent = path.targetIsPresent;
            this.previousPath = path.previousPath;
            this.nextPath = path.nextPath;
        }
    }
}